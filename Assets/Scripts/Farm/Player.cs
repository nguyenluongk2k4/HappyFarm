using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("Thông tin cơ bản")]
    [SerializeField] private int level = 1;
    [SerializeField] private int xp = 0;
    [SerializeField] private int coins = 0;

    [Header("Thiết lập tăng cấp")]
    public int baseXPToLevelUp = 100;
    public float xpGrowthRate = 1.25f; // tăng dần theo cấp

    [Header("Sự kiện cập nhật UI")]
    public UnityEvent<int> OnXPChanged = new UnityEvent<int>();
    public UnityEvent<int> OnCoinChanged = new UnityEvent<int>();
    public UnityEvent<int> OnLevelChanged = new UnityEvent<int>();

    [Header("Thiết lập ban đầu")]
    public bool startWithDefaultInventory = true; // ✅ Bật/tắt khởi tạo kho ban đầu

    private bool inventoryInitialized = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        StartCoroutine(InitWithDelay());
    }

    private IEnumerator InitWithDelay()
    {
        // Đợi InventoryManager khởi tạo
        yield return new WaitUntil(() => InventoryManager.Instance != null);

        // Sau khi InventoryManager sẵn sàng thì mới init
        if (startWithDefaultInventory && !inventoryInitialized)
        {
            InitializeStartingInventory();
            inventoryInitialized = true;
        }
    }

    private void InitializeStartingInventory()
    {
        Debug.Log("🎒 Khởi tạo kho đồ ban đầu...");

        // ✅ Gán số tiền ban đầu và gọi event để update UI
        coins = 50;
        OnCoinChanged.Invoke(coins);
        Debug.Log("💰 Người chơi bắt đầu với 50 xu.");

        // ✅ Thêm vật phẩm khởi đầu
        var hoe = ItemDataList.Instance.GetItemByName("Hoe");
        var seeds = ItemDataList.Instance.GetItemByName("TomatoSeed");
        var waterCan = ItemDataList.Instance.GetItemByName("WaterCan");
        var quoc = ItemDataList.Instance.GetItemByName("Quoc");
        if (hoe != null)
        {
            InventoryManager.Instance.Add(hoe, 1);
            Debug.Log("🪓 Đã thêm 1 Cuốc vào kho.");
        }

        if (seeds != null)
        {
            InventoryManager.Instance.Add(seeds, 10);
            Debug.Log("🌱 Đã thêm 10 hạt giống Cà Chua vào kho.");
        }
        if (waterCan != null)
        {
            InventoryManager.Instance.Add(waterCan, 1);
            Debug.Log(" Đã thêm water can vào kho.");
        }
        if (quoc != null)
        {
            InventoryManager.Instance.Add(quoc, 1);
            Debug.Log(" Đã thêm water can vào kho.");
        }

        // ✅ Cập nhật XP và Level ban đầu
        OnXPChanged.Invoke(xp);
        OnLevelChanged.Invoke(level);
    }

    // ======================
    // 📊 XP & Level
    // ======================
    public void AddXP(int amount)
    {
        xp += amount;
        Debug.Log($"XP hiện tại: {xp}");

        OnXPChanged.Invoke(xp);

        int xpToNext = GetXPRequiredForNextLevel();
        if (xp >= xpToNext)
        {
            xp -= xpToNext;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        OnLevelChanged.Invoke(level);
        Debug.Log($"🎉 Level Up! Cấp hiện tại: {level}");
    }

    public int GetXPRequiredForNextLevel()
    {
        // Ví dụ: Level 1 cần 100 XP, Level 2 cần 125 XP, Level 3 cần 156 XP, v.v.
        return Mathf.RoundToInt(baseXPToLevelUp * Mathf.Pow(xpGrowthRate, level - 1));
    }

    // ======================
    // 💰 Coins
    // ======================
    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins < 0) coins = 0;
        OnCoinChanged.Invoke(coins);
        Debug.Log($"💰 Coins hiện tại: {coins}");
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount)
        {
            Debug.LogWarning("Không đủ tiền!");
            return false;
        }

        coins -= amount;
        OnCoinChanged.Invoke(coins);
        Debug.Log($"💸 Đã chi {amount} coin. Còn lại: {coins}");
        return true;
    }

    // ======================
    // Getter nhanh
    // ======================
    public int GetXP() => xp;
    public int GetCoins() => coins;
    public int GetLevel() => level;

    [System.Serializable]
    public class PlayerSaveData
    {
        public int xp;
        public int coin;
        public Vector3 position;

        public void Save()
        {
            xp = Player.instance.xp;
            coin = Player.instance.coins;
            position = Player.instance.transform.position;
        }

        public void Load()
        {
            Player.instance.xp = xp;
            Player.instance.coins = coin;
            Player.instance.transform.position = position;
            //PlayerHUD.Instance.Refresh(); // nếu bạn có HUD
        }
    }
}
