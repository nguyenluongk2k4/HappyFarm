// Assets/Scripts/Data/FishingRodData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewFishingRod", menuName = "Fishing/Rod")]
public class FishingRodData : ScriptableObject
{
    public string rodName = "Basic Rod";
    [Header("Mechanics")]
    [Tooltip("Sức mạnh khi bắt: càng lớn => tăng chance bắt")]
    public float catchPower = 1f;

    [Tooltip("Nhân tố giảm thời gian chờ cá cắn (1 = không thay đổi, <1 = nhanh hơn)")]
    public float biteTimeModifier = 1f;

    [Tooltip("Giảm difficulty của fish khi dùng cần này (subtractive). Ví dụ 1.5 sẽ giảm effective difficulty.")]
    public float difficultyReduction = 0f;

    [Header("Game meta")]
    public int price = 0;
    public Sprite icon;
}
