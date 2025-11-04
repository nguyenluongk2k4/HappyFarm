// Assets/Scripts/Data/FishData.cs
using UnityEngine;

[System.Serializable]
public class FishData
{
    public string fishName;
    public Sprite icon;
    public int sellPrice = 50;

    [Tooltip("Xác suất (weight) dùng để random xuất hiện. Không phải phần trăm. Ví dụ 0.2, 0.5...")]
    public float rarity = 0.1f;

    [Tooltip("Độ khó: giá trị càng lớn => càng khó bắt (dùng trong công thức bắt).")]
    [Range(1f, 20f)]
    public float difficulty = 5f;

    // tùy chọn: thời gian xuất hiện / mùa / weather... có thể thêm sau
}
