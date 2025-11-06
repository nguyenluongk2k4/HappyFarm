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
}
