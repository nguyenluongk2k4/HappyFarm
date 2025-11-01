// 30/10/2025
// ScriptableObject để định nghĩa dữ liệu cho một loại nông sản (ĐÃ CẬP NHẬT)

using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Farming/Crop")]
public class CropData : ScriptableObject
{
    [Header("Settings")]
    public string cropName = "New Crop";
    public float growTimeInSeconds = 60f;
    public float witherTimeInSeconds = 30f;

    [Header("Items")]
    // --- THAY ĐỔI Ở ĐÂY ---
    public ItemData seedItem;    // Tên hạt giống (để check inventory)
    public ItemData harvestItem; // Tên vật phẩm thu hoạch
                                 // --- HẾT THAY ĐỔI ---

    public int harvestItemAmount = 1;
    public int xpGained = 10;

    [Header("Sprites")]
    public Sprite[] growthSprites;
    public Sprite witheredSprite;
}