using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameData gameData;


    // ✨ Thêm mới - Thông tin spawn khi đổi scene
    private bool useCustomSpawnPosition = false;
    private Vector3 nextSpawnPosition;
    private string nextSpawnPointName;

    private string saveFileName = "savegame.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    public GameData CurrentGameData { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ✨ Đăng ký sự kiện khi load scene xong
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("GameManager initialized");
    }

    void Update()
    {
        // Game logic here
    }

    /// <summary>
    /// Load scene theo index
    /// </summary>
    /// 
    public void LoadGameAndStart()
    {
        // Thử load
        gameData = SaveSystem.Load();

        if (gameData == null)
        {
            Debug.Log("Không có save → tạo game mới");
            gameData = new GameData();
            gameData.InitializeNewGame();
        }

        // Load map
        SceneManager.LoadScene(gameData.currentMap);
    }

    public void SaveGame()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" ||
       SceneManager.GetActiveScene().name == "PlayerPref")
        {
            Debug.Log("Skip saving on MainMenu/PlayerPref");
            return;
        }

        if (gameData == null)
            gameData = new GameData();
        gameData.currentMap = SceneManager.GetActiveScene().name;


        // Thu thập dữ liệu
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            gameData.playerPosition = player.transform.position;

        gameData.currentMap = SceneManager.GetActiveScene().name;

        // TODO: Thu thập inventory, tiles... (tuỳ bạn đã code)

        SaveSystem.Save(gameData);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Scene index {sceneIndex} is out of range!");
            return;
        }

        Debug.Log($"Loading scene at index: {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }

    // ✨ Thêm mới — Set thông tin spawn trước khi load scene
    public void SetNextSpawnInfo(bool useCustom, Vector3 pos, string pointName)
    {
        useCustomSpawnPosition = useCustom;
        nextSpawnPosition = pos;
        nextSpawnPointName = pointName;
    }

    // ✨ Thêm mới — Khi scene mới load xong thì set vị trí Player
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //if (player == null) return;

        //if (useCustomSpawnPosition)
        //{
        //    player.transform.position = nextSpawnPosition;
        //    Debug.Log($"Spawned player at custom position: {nextSpawnPosition}");
        //}
        //else if (!string.IsNullOrEmpty(nextSpawnPointName))
        //{
        //    GameObject spawnPoint = GameObject.Find(nextSpawnPointName);
        //    if (spawnPoint != null)
        //    {
        //        player.transform.position = spawnPoint.transform.position;
        //        Debug.Log($"Spawned player at point: {nextSpawnPointName}");
        //    }
        //}
        if (gameData != null)
        {
            GameObject playerLoaded = GameObject.FindGameObjectWithTag("Player");
            if (playerLoaded != null)
            {
                playerLoaded.transform.position = gameData.playerPosition;
                Debug.Log("Player loaded to saved position " + gameData.playerPosition);
            }
        }
        SaveGame();

    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }

}
