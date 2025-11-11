using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    // Spawn info khi đổi scene
    private bool useCustomSpawnPosition = false;
    private Vector3 nextSpawnPosition;
    private string nextSpawnPointName;

    [Header("Prefabs")]
    public GameObject landPlotPrefab;

    [Header("UI References")]
    public GameObject hudCanvas;
    public GameObject inventoryUI;
    public GameObject hotbarUI;

    [Header("Animal Prefabs (Kéo tất cả prefab gà vào đây)")]
    public List<GameObject> animalPrefabs = new List<GameObject>();
    public Dictionary<string, GameObject> animalPrefabDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.name = "[GameManager - DontDestroyOnLoad]";

            // Build prefab dictionary
            foreach (var prefab in animalPrefabs)
            {
                if (!animalPrefabDict.ContainsKey(prefab.name))
                    animalPrefabDict.Add(prefab.name, prefab);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("✓ GameManager initialized & DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnableGameplayUI()
    {
        if (hudCanvas) hudCanvas.SetActive(true);
        if (hotbarUI) hotbarUI.SetActive(true);
        if (inventoryUI) inventoryUI.SetActive(true); // ẩn mặc định
        Debug.Log("✅ HUD + Hotbar enabled");
    }

    private void DisableAllGameplayUI()
    {
        if (hudCanvas) hudCanvas.SetActive(false);
        if (hotbarUI) hotbarUI.SetActive(false);
        if (inventoryUI) inventoryUI.SetActive(false);
        Debug.Log("🚫 Gameplay UI Disabled (Boot Scene)");
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Scene index out of range!");
            return;
        }
        SceneManager.LoadScene(sceneIndex);
    }

    public void SetNextSpawnInfo(bool useCustom, Vector3 pos, string pointName)
    {
        useCustomSpawnPosition = useCustom;
        nextSpawnPosition = pos;
        nextSpawnPointName = pointName;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Bật UI gameplay ở những scene có chơi
        if (scene.name == "Farm" || scene.name == "Beach" || scene.name == "Market")
        {
            EnableGameplayUI();

            // ✅ Nếu có data gà -> spawn
            if (AnimalData.memory.Count > 0)
                AnimalData.SpawnFromMemory();
        }
        else
        {
            // Scene boot: tắt gameplay UI
            DisableAllGameplayUI();
        }


        // Spawn Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            if (useCustomSpawnPosition)
                player.transform.position = nextSpawnPosition;
            else if (!string.IsNullOrEmpty(nextSpawnPointName))
            {
                GameObject sp = GameObject.Find(nextSpawnPointName);
                if (sp) player.transform.position = sp.transform.position;
            }
        }
    }

    // ================= SAVE / LOAD =================

    public void SaveGame()
    {
        SaveSystem.Save();
        AnimalData.Save(); // ✅ Lưu gà
        Debug.Log("✅ Lưu game + gà thành công!");
    }

    public void LoadGame()
    {
        SaveSystem.Load();
        // AnimalData.Load();
        AnimalData.LoadToMemory();
        Debug.Log("📥 LoadGame invoked!");
    }
}
