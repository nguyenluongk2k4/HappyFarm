using System.Collections.Generic;
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

    public void UpdateVisuals()
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

    // =============== INTERACTION =============== //
    public void Interact(PlayerInteraction interactor)
    {
        switch (interactor.CurrentTool)
        {
            case PlayerInteraction.ToolType.Hand:
                Harvest();
                ClearWithered();
                break;
            // CycleStateWithFarmTool();
            // case PlayerInteraction.ToolType.Hoe:
            //     Plow();
            //     break;
            case PlayerInteraction.ToolType.FarmLand:
                Plow();
                break;
            case PlayerInteraction.ToolType.Seed:
                var selected = HotbarManager.Instance.GetSelectedStack();

                if (selected != null && selected.item.type == ItemType.Seed)
                {
                    if (currentState != LandState.Plowed)
                    {
                        Debug.LogWarning("🌾 Đất chưa được cày nên không thể gieo hạt!");
                        break;
                    }

                    // 🔍 tìm CropData có seedItem trùng với item đang cầm
                    CropData foundCrop = FindCropBySeed(selected.item);
                    if (foundCrop == null)
                    {
                        Debug.LogWarning("❌ Không tìm thấy CropData cho hạt này!");
                        break;
                    }

                    if (!Plant(foundCrop))
                    {
                        Debug.LogWarning("❌ Không thể gieo hạt trên mảnh đất hiện tại!");
                        break;
                    }

                    // Ưu tiên trừ trực tiếp trên hotbar; nếu thất bại thì trừ trong inventory như dự phòng
                    if (!HotbarManager.Instance.ConsumeSelected(1))
                    {
                        int removed = InventoryManager.Instance.Remove(selected.item, 1);
                        if (removed <= 0)
                        {
                            Debug.LogWarning("⚠️ Không thể trừ hạt giống khỏi Hotbar hoặc Inventory.");
                        }
                    }
                }
                break;
        }
    }

    private CropData FindCropBySeed(ItemData seedItem)
    {
        CropData[] allCrops = Resources.LoadAll<CropData>("Crops");
        Debug.Log($"Có {allCrops.Length} CropData được load.");

        foreach (var crop in allCrops)
        {
            Debug.Log($"🌱 Crop: {crop.name}, seedItem = {crop.seedItem?.name}");
            if (crop.seedItem == seedItem)
            {
                Debug.Log($"✅ Match tìm thấy: {crop.cropName}");
                return crop;
            }
        }

        Debug.LogWarning($"❌ Không tìm thấy CropData cho {seedItem?.name}");
        return null;
    }


    // =============== ACTIONS =============== //
    void CycleStateWithFarmTool()
    {
        switch (currentState)
        {
            case LandState.Empty:
                currentState = LandState.Plowed;
                break;
            case LandState.Plowed:
                if (!EnsureDebugCrop()) return;
                currentState = LandState.Planted;
                growTimer = 0f;
                currentGrowthStage = 0;
                break;
            case LandState.Planted:
                if (!EnsureDebugCrop()) return;
                currentState = LandState.Ready;
                growTimer = 0f;
                witherTimer = 0f;
                currentGrowthStage = currentCrop.growthSprites.Length > 0 ? currentCrop.growthSprites.Length - 1 : 0;
                break;
            case LandState.Ready:
                if (!EnsureDebugCrop()) return;
                currentState = LandState.Withered;
                witherTimer = 0f;
                break;
            case LandState.Withered:
                currentState = LandState.Empty;
                currentCrop = null;
                growTimer = 0f;
                witherTimer = 0f;
                currentGrowthStage = 0;
                break;
        }

        UpdateVisuals();
        Debug.Log($"🌾 Farm tool chuyển sang trạng thái {currentState}");
    }

    bool EnsureDebugCrop()
    {
        if (currentCrop != null) return true;

        CropData[] crops = Resources.LoadAll<CropData>("Crops");
        if (crops.Length == 0)
        {
            Debug.LogWarning("⚠️ Không có CropData nào trong Resources/Crops để dùng cho FarmLand tool.");
            return false;
        }

        currentCrop = crops[0];
        currentGrowthStage = 0;
        return currentCrop.growthSprites != null && currentCrop.growthSprites.Length > 0;
    }

    // =============== ACTIONS =============== //
    public void Plow()
    {
        if (currentState == LandState.Empty)
        {
            currentState = LandState.Plowed;
            UpdateVisuals();
            Debug.Log("Đã cày đất!");
        }
    }

    public bool Plant(CropData crop)
    {
        if (currentState == LandState.Plowed)
        {
            Debug.Log($"Đã gieo {crop.seedItem.itemName}");
            currentCrop = crop;
            currentState = LandState.Planted;
            growTimer = 0;
            currentGrowthStage = 0;
            UpdateVisuals();
            return true;
        }

        return false;
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

        // ✅ Cộng XP khi thu hoạch thành công
        if (Player.instance != null)
        {
            Player.instance.AddXP(5);
            Debug.Log("+5 XP từ việc thu hoạch!");
        }

        currentState = LandState.Empty;
        currentCrop = null;
        UpdateVisuals();
    }

    public void ClearWithered()
    {
        if (currentState == LandState.Withered)
        {
            if (Player.instance != null)
            {
                // ✅ Kiểm tra đủ tiền không
                bool paid = Player.instance.SpendCoins(5);
                if (!paid)
                {
                    Debug.LogWarning("❌ Không đủ 5 xu để dọn cây héo!");
                    return;
                }

                Debug.Log("🧹 Dọn cây héo (Mất 5 xu)");
            }

            currentState = LandState.Empty;
            currentCrop = null;
            UpdateVisuals();
        }
    }


    [System.Serializable]
    public class LandPlotSaveData
    {
        public List<LandSave> lands = new();

        [System.Serializable]
        public class LandSave
        {
            public Vector3 position;
            public int state;
            public string cropName;
            public float growTimer;
            public float witherTimer;
            public int growthStage;
        }

        public void Save()
        {
            lands.Clear();
            foreach (var land in GameObject.FindObjectsOfType<LandPlot>())
            {
                lands.Add(new LandSave
                {
                    position = land.transform.position,
                    state = (int)land.currentState,
                    cropName = land.currentCrop != null ? land.currentCrop.name : "",
                    growTimer = land.currentState == LandPlot.LandState.Planted ? land.growTimer : 0,
                    witherTimer = land.currentState == LandPlot.LandState.Ready ? land.witherTimer : 0,
                    growthStage = land.currentGrowthStage
                });
            }

            Debug.Log($"💾 Đã lưu {lands.Count} mảnh đất");
        }

        public void Load()
        {
            var allPlots = new List<LandPlot>(GameObject.FindObjectsOfType<LandPlot>());
            int loaded = 0;
            int spawned = 0;

            foreach (var data in lands)
            {
                LandPlot plot = FindPlotByPosition(allPlots.ToArray(), data.position);

                // ✅ Nếu không tìm thấy -> spawn mới
                if (plot == null)
                {
                    if (GameManager.Instance.landPlotPrefab != null)
                    {
                        GameObject newPlot = GameObject.Instantiate(
                            GameManager.Instance.landPlotPrefab,
                            data.position,
                            Quaternion.identity
                        );

                        plot = newPlot.GetComponent<LandPlot>();
                        allPlots.Add(plot);
                        spawned++;

                        Debug.Log($"🆕 Spawn mới ô đất tại {data.position}");
                    }
                    else
                    {
                        Debug.LogError("❌ landPlotPrefab chưa được gán trong GameManager!");
                        continue;
                    }
                }

                // ✅ Khôi phục dữ liệu đất
                plot.currentState = (LandPlot.LandState)data.state;
                plot.currentCrop = string.IsNullOrEmpty(data.cropName)
                    ? null
                    : Resources.Load<CropData>("Crops/" + data.cropName);

                plot.growTimer = data.growTimer;
                plot.witherTimer = data.witherTimer;
                plot.currentGrowthStage = data.growthStage;

                plot.UpdateVisuals();
                loaded++;
            }

            Debug.Log($"✅ Load xong {loaded}/{lands.Count} ô đất (Spawn mới {spawned})");
        }


        // Tìm đất gần đúng vị trí (sai số 0.1f)
        private LandPlot FindPlotByPosition(LandPlot[] plots, Vector3 pos)
        {
            foreach (var plot in plots)
            {
                if (Vector3.Distance(plot.transform.position, pos) < 0.1f)
                    return plot;
            }
            return null;
        }
    }

}

