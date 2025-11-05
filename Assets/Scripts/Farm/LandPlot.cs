using UnityEngine;

public class LandPlot : MonoBehaviour, IInteractable
{
    public enum LandState { Empty, Plowed, Planted, Ready, Withered }

    [Header("State")]
    public LandState currentState = LandState.Empty;

    [Header("Data")]
    private CropData currentCrop;
    private float growTimer;
    private float witherTimer;
    private int currentGrowthStage = 0;

    [Header("References")]
    public SpriteRenderer plotSpriteRenderer;
    public Sprite emptySprite;
    public Sprite plowedSprite;

    void Start()
    {
        currentState = LandState.Empty;
        UpdateVisuals();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        switch (currentState)
        {
            case LandState.Planted:
                growTimer += Time.deltaTime;
                int stage = GetCurrentGrowthStage();
                if (stage != currentGrowthStage)
                {
                    currentGrowthStage = stage;
                    UpdateVisuals();
                }

                if (growTimer >= currentCrop.growTimeInSeconds)
                {
                    currentState = LandState.Ready;
                    growTimer = 0;
                    witherTimer = 0;
                    Debug.Log(currentCrop.cropName + " đã chín!");
                    UpdateVisuals();
                }
                break;

            case LandState.Ready:
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
                plotSpriteRenderer.sprite = currentCrop.growthSprites[currentGrowthStage];
                break;
            case LandState.Ready:
                plotSpriteRenderer.sprite = currentCrop.growthSprites[^1];
                break;
            case LandState.Withered:
                plotSpriteRenderer.sprite = currentCrop.witheredSprite;
                break;
        }
    }

    private int GetCurrentGrowthStage()
    {
        if (currentCrop == null || currentCrop.growthSprites.Length == 0) return 0;
        float progress = growTimer / currentCrop.growTimeInSeconds;
        int totalStages = currentCrop.growthSprites.Length - 1;
        int stage = Mathf.FloorToInt(progress * totalStages);
        return Mathf.Clamp(stage, 0, totalStages);
    }

    // Tương tác thông qua IInteractable
    public void Interact(PlayerInteraction interactor)
    {
        switch (interactor.CurrentTool)
        {
            case PlayerInteraction.ToolType.Hand:
                Harvest();
                ClearWithered();
                break;
            case PlayerInteraction.ToolType.Hoe:
                Plow();
                break;
            case PlayerInteraction.ToolType.Seed:
                Plant(interactor.testTomatoSeed);
                break;
        }
    }

    public void Plow()
    {
        if (currentState == LandState.Empty)
        {
            currentState = LandState.Plowed;
            UpdateVisuals();
            Debug.Log("Đã cày đất!");
        }
    }

    public void Plant(CropData crop)
    {
        if (currentState == LandState.Plowed)
        {
            Debug.Log($"Đã gieo {crop.seedItem.itemName}");
            currentCrop = crop;
            currentState = LandState.Planted;
            growTimer = 0;
            currentGrowthStage = 0;
            UpdateVisuals();
        }
    }

    public void Harvest()
    {
        if (currentState != LandState.Ready) return;

        if (!InventoryManager.Instance.CanAdd(currentCrop.harvestItem, currentCrop.harvestItemAmount))
        {
            Debug.LogWarning("Kho đầy! Không thể thu hoạch " + currentCrop.harvestItem.itemName);
            return;
        }

        InventoryManager.Instance.Add(currentCrop.harvestItem, currentCrop.harvestItemAmount);
        Debug.Log("Thu hoạch thành công: " + currentCrop.harvestItem.itemName);

        currentState = LandState.Empty;
        currentCrop = null;
        UpdateVisuals();
    }

    public void ClearWithered()
    {
        if (currentState == LandState.Withered)
        {
            Debug.Log("Dọn cây héo (Mất 5 xu)");
            currentState = LandState.Empty;
            currentCrop = null;
            UpdateVisuals();
        }
    }
}
