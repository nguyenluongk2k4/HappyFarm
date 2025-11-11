using UnityEngine;
using System.Collections;

public class PlayerFishing : MonoBehaviour
{
    [Header("References")]
    public FishingRodData equippedRod;       // Cần câu hiện đang đeo      // (Không cần thiết nếu dùng InventoryManager)
    public PlayerInteraction playerInteraction;

    private FishingSpot currentSpot;
    private bool inRange = false;
    private bool isFishing = false;

    void Start()
    {
        // Nếu chưa gán trong Inspector thì tự tìm trên Player
        if (playerInteraction == null)
            playerInteraction = GetComponent<PlayerInteraction>();
    }

    public void SetRod(ItemData item)
    {
        if (item == null)
        {
            equippedRod = null;
            MessageUI.Instance.ShowMessage(" Không có cần câu nào được chọn!");
            return;
        }

        if (item is FishingRodData rodData)
        {
            equippedRod = rodData;
            MessageUI.Instance.ShowMessage($" Trang bị cần câu: {equippedRod.rodName}");
        }
        else
        {
            equippedRod = null;
            MessageUI.Instance.ShowMessage($" {item.itemName} không phải là loại cần câu hợp lệ!");
        }
    }

    void Update()
    {
        if (!inRange || isFishing) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInteraction == null || playerInteraction.CurrentTool != PlayerInteraction.ToolType.Rod)
            {
                MessageUI.Instance.ShowMessage(" Bạn chưa trang bị cần câu!");
                return;
            }

            if (equippedRod == null)
            {
                MessageUI.Instance.ShowMessage(" Bạn chưa chọn loại cần câu cụ thể!");
                return;
            }

            StartCoroutine(DoFishing());
        }
    }

    IEnumerator DoFishing()
    {
        isFishing = true;
        MessageUI.Instance.ShowMessage(" Bắt đầu câu cá...");

        yield return StartCoroutine(FishingManager.Instance.StartFishingRoutine(currentSpot, equippedRod, OnFishResult));

        yield return new WaitForSeconds(0.3f);
        isFishing = false;
    }

    void OnFishResult(FishData fish, bool caught)
    {
        if (fish == null)
        {
            MessageUI.Instance.ShowMessage("Không có cá ở đây.");
            return;
        }

        if (caught)
        {
            MessageUI.Instance.ShowMessage($" Bắt được {fish.fishName} (Giá {fish.sellPrice})");

            // ✅ Thêm cá vào kho người chơi
            var fishItem = ItemDataList.Instance.GetItemByName(fish.fishName);
            if (fishItem != null)
            {
                InventoryManager.Instance.Add(fishItem, 1);
                MessageUI.Instance.ShowMessage($" Đã thêm {fish.fishName} vào kho!");
            }
            else
            {
                MessageUI.Instance.ShowMessage($" Không tìm thấy vật phẩm tương ứng với {fish.fishName} trong ItemDataList!");
            }
        }
        else
        {
            MessageUI.Instance.ShowMessage($" {fish.fishName} sổng mất!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out FishingSpot spot))
        {
            currentSpot = spot;
            inRange = true;
            MessageUI.Instance.ShowMessage("Đến khu vực câu cá (bấm E để câu)");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out FishingSpot spot) && spot == currentSpot)
        {
            currentSpot = null;
            inRange = false;
        }
    }
}
