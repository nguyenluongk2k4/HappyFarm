using static InventoryManager;
using static Player;
using System;
using UnityEngine;
using static LandPlot;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneData
{
    public int sceneIndex;                    // Scene người chơi đang đứng
    public InventorySaveData inventory;
    public HotbarSaveData hotbar;
    public LandPlotSaveData landPlots;
    public PlayerSaveData player;            // bạn tự tạo nếu chưa có
}

public class SaveSystem
{
    public static SceneData saveData = new SceneData();

    private static string SaveFilePath()
    {
        return Application.persistentDataPath + "/save.json";
    }

    // =================== SAVE ===================
    public static void Save()
    {
        HandleSaveData();

        string json = JsonUtility.ToJson(saveData, true);
        string encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        File.WriteAllText(SaveFilePath(), encoded);

        Debug.Log("✅ Game Saved!");
    }

    private static void HandleSaveData()
    {
        saveData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        saveData.inventory = new InventorySaveData();
        saveData.inventory.Save();

        saveData.hotbar = new HotbarSaveData();
        saveData.hotbar.Save();

        saveData.landPlots = new LandPlotSaveData();
        saveData.landPlots.Save();

        saveData.player = new PlayerSaveData();
        saveData.player.Save();
    }

    // =================== LOAD ===================
    public static void Load()
    {
        if (!File.Exists(SaveFilePath()))
        {
            Debug.LogWarning("❌ Không tìm thấy file save!");
            return;
        }

        string encoded = File.ReadAllText(SaveFilePath());
        string json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        saveData = JsonUtility.FromJson<SceneData>(json);

        Debug.Log("⏳ Load scene trước...");
        LoadDataAsync();
    }

    private static async void LoadDataAsync()
    {
        // 1. Load Scene trước
        await SceneManager.LoadSceneAsync(saveData.sceneIndex);

        // 2. Đợi 1 frame để GameObject + Manager Awake xong
        await Task.Delay(100);

        // 3. Load dữ liệu vào game
        saveData.inventory.Load();
        saveData.hotbar.Load();
        saveData.landPlots.Load();
        saveData.player.Load();
        // AnimalData.Load();

        Debug.Log("✅ LoadGame thành công!");
    }
}
