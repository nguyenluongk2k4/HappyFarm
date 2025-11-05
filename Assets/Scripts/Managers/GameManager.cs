using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    // ✨ Thêm mới - Thông tin spawn khi đổi scene
    private bool useCustomSpawnPosition = false;
    private Vector3 nextSpawnPosition;
    private string nextSpawnPointName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ GameManager sẽ tồn tại qua các scene
            gameObject.name = "[GameManager - DontDestroyOnLoad]"; // Dễ nhận biết

            // ✨ Đăng ký sự kiện khi load scene xong
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("✓ GameManager khởi tạo và set DontDestroyOnLoad");
        }
        else
        {
            Debug.LogWarning("⚠️ GameManager đã tồn tại, destroy bản sao này");
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (useCustomSpawnPosition)
        {
            player.transform.position = nextSpawnPosition;
            Debug.Log($"Spawned player at custom position: {nextSpawnPosition}");
        }
        else if (!string.IsNullOrEmpty(nextSpawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(nextSpawnPointName);
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"Spawned player at point: {nextSpawnPointName}");
            }
        }
    }
}
