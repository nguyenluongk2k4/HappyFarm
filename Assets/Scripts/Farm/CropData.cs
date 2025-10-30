// 30/10/2025
// ScriptableObject để định nghĩa dữ liệu cho một loại nông sản

using UnityEngine;

// Dòng này cho phép bạn tạo asset mới từ menu (Chuột phải -> Create -> Farming -> Crop)
[CreateAssetMenu(fileName = "New Crop", menuName = "Farming/Crop")]
public class CropData : ScriptableObject
{
    [Header("Settings")]
    public string cropName = "New Crop";
    public float growTimeInSeconds = 60f; // Thời gian lớn (tính bằng giây)
    public float witherTimeInSeconds = 30f; // Thời gian bị héo (sau khi chín)

    [Header("Items")]
    public string seedItemName = "Seed"; // Tên hạt giống (để check inventory)
    public string harvestItemName = "Harvested Crop"; // Tên vật phẩm thu hoạch
    public int harvestItemAmount = 1;
    public int xpGained = 10;

    [Header("Sprites")]
    // Sprite cho các giai đoạn
    // 0 = Mầm (Planted)
    // 1...N = Các giai đoạn lớn
    // Cuối cùng = Chín (Ready)
    public Sprite[] growthSprites;
    public Sprite witheredSprite; // Sprite khi bị héo
}