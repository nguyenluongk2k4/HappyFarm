// 30/10/2025
// Quản lý trạng thái và logic của một ô đất trồng trọt
// Đã cập nhật để kiểm tra kho đồ (InventoryManager) khi thu hoạch.

using UnityEngine;

public class LandPlot : MonoBehaviour,IInteractable
{
    // Enum định nghĩa các trạng thái của ô đất (như trong Use Case)
    public enum LandState
    {
        Empty,   // Đất trống
        Plowed,  // Đã cày
        Planted, // Đã gieo hạt
        Ready,   // Sẵn sàng thu hoạch
        Withered // Bị héo
    }

    [Header("State")]
    public LandState currentState = LandState.Empty;

    [Header("Data")]
    private CropData currentCrop;   // Loại cây đang trồng
    private float growTimer;        // Đếm giờ sinh trưởng
    private float witherTimer;      // Đếm giờ bị héo
    private int currentGrowthStage = 0;

    [Header("References")]
    public SpriteRenderer plotSpriteRenderer; // Sprite của chính ô đất
    public Sprite emptySprite;                // Sprite đất trống
    public Sprite plowedSprite;               // Sprite đất đã cày

    void Start()
    {
        // Bắt đầu với trạng thái Trống
        currentState = LandState.Empty;
        UpdateVisuals();
    }

    void Update()
    {
        // Chỉ chạy logic khi game không bị pause
        if (Time.timeScale == 0) return;

        switch (currentState)
        {
            case LandState.Planted:
                growTimer += Time.deltaTime;

                // Cập nhật sprite theo từng giai đoạn
                int stage = GetCurrentGrowthStage();
                if (stage != currentGrowthStage)
                {
                    currentGrowthStage = stage;
                    UpdateVisuals();
                }

                // Kiểm tra nếu đã chín (UC-1.2)
                if (growTimer >= currentCrop.growTimeInSeconds)
                {
                    currentState = LandState.Ready;
                    growTimer = 0;
                    witherTimer = 0; // Bắt đầu đếm giờ héo
                    Debug.Log(currentCrop.cropName + " đã chín!");
                    UpdateVisuals();
                }
                break;

            case LandState.Ready:
                // Bắt đầu đếm giờ héo (UC-1.4)
                witherTimer += Time.deltaTime;
                if (witherTimer >= currentCrop.witherTimeInSeconds)
                {
                    currentState = LandState.Withered;
                    witherTimer = 0;
                    Debug.Log(currentCrop.cropName + " đã bị héo!");
                    UpdateVisuals();
                }
                break;
        }
    }

    // Cập nhật hình ảnh của ô đất dựa trên trạng thái
    private void UpdateVisuals()
    {
        switch (currentState)
        {
            case LandState.Empty:
                plotSpriteRenderer.sprite = emptySprite;
                break;
            case LandState.Plowed:
                plotSpriteRenderer.sprite = plowedSprite;
                break;
            case LandState.Planted:
                // Hiển thị sprite mầm cây
                plotSpriteRenderer.sprite = currentCrop.growthSprites[currentGrowthStage];
                break;
            case LandState.Ready:
                // Hiển thị sprite cây đã chín (sprite cuối cùng trong mảng)
                plotSpriteRenderer.sprite = currentCrop.growthSprites[currentCrop.growthSprites.Length - 1];
                break;
            case LandState.Withered:
                plotSpriteRenderer.sprite = currentCrop.witheredSprite;
                break;
        }
    }

    // Tính toán giai đoạn sinh trưởng để hiển thị sprite
    private int GetCurrentGrowthStage()
    {
        if (currentCrop == null || currentCrop.growthSprites.Length == 0) return 0;

        float progress = growTimer / currentCrop.growTimeInSeconds;
        // Trừ 1 vì mảng cuối là 'Ready' (sẽ được xử lý riêng)
        int totalStages = currentCrop.growthSprites.Length - 1;
        int stage = Mathf.FloorToInt(progress * totalStages);

        return Mathf.Clamp(stage, 0, totalStages);
    }

    // --- CÁC HÀM TƯƠNG TÁC (Do PlayerInteraction gọi) ---

    // UC-1.1: Cày đất
    public void Plow()
    {
        if (currentState == LandState.Empty)
        {
            currentState = LandState.Plowed;
            UpdateVisuals();
            Debug.Log("Đã cày đất!");
            // TODO: Phát âm thanh "cày đất" (UC-1.1)
        }
    }

    // UC-1.2: Gieo hạt
    public void Plant(CropData cropToPlant)
    {
        if (currentState == LandState.Plowed)
        {
            // TODO: Trừ 1 HạtGiống (cropToPlant.seedItem) khỏi Kho (UC-1.2)
            // (Hiện tại, chúng ta giả lập là luôn có hạt)
            Debug.Log("Đã gieo " + cropToPlant.seedItem.itemName);

            currentCrop = cropToPlant;
            currentState = LandState.Planted;
            growTimer = 0; // Bắt đầu tính giờ
            currentGrowthStage = 0;
            UpdateVisuals(); // Hiển thị mầm cây
        }
    }

    // UC-1.3: Thu hoạch (ĐÃ CẬP NHẬT)
    public void Harvest()
    {
        if (currentState == LandState.Ready)
        {
            // --- LOGIC MỚI: KIỂM TRA KHO ---
            // Gọi InventoryManager để kiểm tra kho
            // (Giả định InventoryManager.Instance đã tồn tại trong Scene)
            if (InventoryManager.Instance.CheckForSpace(currentCrop.harvestItem, currentCrop.harvestItemAmount) == false)
            {
                // Luồng phụ (Kho đầy) (UC-1.3)
                Debug.LogWarning("Kho đã đầy! Không thể thu hoạch " + currentCrop.harvestItem.itemName);

                // Dừng hàm tại đây, ô đất vẫn ở trạng thái Ready
                return;
            }
            // --- HẾT LOGIC MỚI ---

            // Nếu code chạy đến đây, nghĩa là kho CÒN CHỖ

            // 1. Thêm Nông sản vào Kho (UC-1.3)
            InventoryManager.Instance.AddItem(currentCrop.harvestItem, currentCrop.harvestItemAmount);

            // 2. Cộng XP (UC-1.3)
            Debug.Log("Nhận được " + currentCrop.xpGained + " XP");
            // TODO: PlayerStats.Instance.AddXP(currentCrop.xpGained);

            // 3. Phát âm thanh "thu hoạch" (UC-1.3)

            // 4. Reset ô đất
            currentState = LandState.Empty;
            currentCrop = null;
            UpdateVisuals();
        }
    }

    // UC-1.4: Dọn cây héo
    public void ClearWithered()
    {
        if (currentState == LandState.Withered)
        {
            // TODO: Hiển thị hộp thoại xác nhận (UC-1.4)

            // TODO: Trừ 5 Xu (UC-1.4)
            Debug.Log("Dọn cây héo (Mất 5 Xu)");

            currentState = LandState.Empty;
            currentCrop = null;
            UpdateVisuals();
        }
    }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }
}