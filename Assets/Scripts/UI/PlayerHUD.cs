using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;

    private IEnumerator Start()
{
    // ⏳ Chờ Player khởi tạo xong
    while (Player.instance == null)
        yield return null;

    // ✅ Kết nối event
    Player.instance.OnCoinChanged.AddListener(UpdateCoins);
    Player.instance.OnXPChanged.AddListener(UpdateXP);
    Player.instance.OnLevelChanged.AddListener(UpdateLevel);

    Debug.Log("✅ PlayerHUD đã kết nối event thành công!");

    // 🟢 Cập nhật thủ công dữ liệu hiện tại (phòng trường hợp event bắn trước)
    UpdateCoins(Player.instance.GetCoins());
    UpdateXP(Player.instance.GetXP());
    UpdateLevel(Player.instance.GetLevel());
}


    void UpdateCoins(int value) => coinText.text = $"Coin {value}";
    void UpdateXP(int value) => xpText.text = $"XP: {value}/{Player.instance.GetXPRequiredForNextLevel()}";
    void UpdateLevel(int value) => levelText.text = $"Lv {value}";
}
