using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish")]
public class FishData : ItemData
{
    
   

    [Header("Fishing Info")]

    public string fishName = "";
    [Tooltip("Xác suất (weight) dùng để random xuất hiện. Không phải phần trăm. Ví dụ 0.2, 0.5...")]
    public float rarity = 0.1f;

    [Tooltip("Độ khó: giá trị càng lớn => càng khó bắt (dùng trong công thức bắt).")]
    [Range(1f, 10f)]
    public float difficulty = 5f;

    [Header("Kinh tế")]
    public int sellPrice = 50;

    private void OnEnable()
    {
        // Đảm bảo luôn nhận diện đúng loại trong switch-case
        type = ItemType.Fish;
    }
}
