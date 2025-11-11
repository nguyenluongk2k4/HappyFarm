// Assets/Scripts/Data/FishingRodData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewFishingRod", menuName = "Fishing/Rod")]
public class FishingRodData : ItemData
{
    [Header("Rod Info")]
    public string rodName = "Basic Rod";

    [Header("Mechanics")]
    [Tooltip("Sức mạnh khi bắt: càng lớn => tăng khả năng bắt thành công")]
    public float catchPower = 1f;

    [Tooltip("Nhân tố giảm thời gian chờ cá cắn (1 = không thay đổi, <1 = nhanh hơn)")]
    public float biteTimeModifier = 1f;

    [Tooltip("Giảm độ khó khi bắt cá (ví dụ 1.5 sẽ giúp dễ bắt hơn)")]
    public float difficultyReduction = 0f;

    [Header("Game Meta")]
    [Tooltip("Giá bán hoặc giá mua của cần câu này")]
    public int price = 0;

    private void OnEnable()
    {
        // Đảm bảo luôn nhận diện đúng loại trong switch-case
        type = ItemType.Rod;
    }
}
