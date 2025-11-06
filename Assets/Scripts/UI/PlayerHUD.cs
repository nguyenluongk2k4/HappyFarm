using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Start()
    {
        UpdateCoins(Player.instance.GetCoins());
        UpdateXP(Player.instance.GetXP());
        UpdateLevel(Player.instance.GetLevel());

        Player.instance.OnCoinChanged.AddListener(UpdateCoins);
        Player.instance.OnXPChanged.AddListener(UpdateXP);
        Player.instance.OnLevelChanged.AddListener(UpdateLevel);
    }

    void UpdateCoins(int value) => coinText.text = $"Coin {value}";
    void UpdateXP(int value) => xpText.text = $"XP: {value}/{Player.instance.GetXPRequiredForNextLevel()}";
    void UpdateLevel(int value) => levelText.text = $"Lv {value}";
}
