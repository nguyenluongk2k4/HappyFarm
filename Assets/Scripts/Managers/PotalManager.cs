using UnityEngine;

public class PotalManager : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex = 2; // Index của scene muốn chuyển đến

    // ✨ Thêm mới — Thông tin spawn
    [Header("Spawn Settings")]
    [SerializeField] private bool useCustomSpawnPosition = false;
    [SerializeField] private Vector3 customSpawnPosition;
    [SerializeField] private string spawnPointName = "";

    void Start()
    {
        // Đảm bảo object có Collider và được set là Trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Nếu là Collider2D
        Collider2D col2D = GetComponent<Collider2D>();
        if (col2D != null)
        {
            col2D.isTrigger = true;
        }
    }

    // Phát hiện va chạm với player (3D)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeScene();
        }
    }

    // Phát hiện va chạm với player (2D)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        // Kiểm tra GameManager có tồn tại không
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found! Please add GameManager to the scene.");
            return;
        }

        // ✨ Gửi thông tin spawn sang GameManager
        GameManager.Instance.SetNextSpawnInfo(useCustomSpawnPosition, customSpawnPosition, spawnPointName);

        // Chuyển scene bằng index
        GameManager.Instance.LoadSceneByIndex(targetSceneIndex);
        Debug.Log($"Portal: Loading scene at index {targetSceneIndex}");
    }

    /// <summary>
    /// Hàm công khai để load scene theo index từ bên ngoài
    /// </summary>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (GameManager.Instance != null)
        {
            // ✨ Gửi thông tin spawn khi load từ code khác
            GameManager.Instance.SetNextSpawnInfo(useCustomSpawnPosition, customSpawnPosition, spawnPointName);

            GameManager.Instance.LoadSceneByIndex(sceneIndex);
            Debug.Log($"Portal: Loading scene at index {sceneIndex}");
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }
}
