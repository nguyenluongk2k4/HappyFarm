// Assets/Scripts/Fishing/FishingSpot.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FishingSpot : MonoBehaviour
{
    public string spotName = "Sea Spot";
    public FishDatabase fishDatabase;
    public float biteTimeMin = 2f;
    public float biteTimeMax = 5f;

    // có thể thêm biến cho vùng: deep/shallow, special modifier...
    public float spotRarityModifier = 1f; // nhân vào rarity nếu muốn

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Hàm để lấy 1 con ngẫu nhiên từ database (bỏ qua modifier cho đơn giản)
    public FishData GetRandomFish()
    {
        if (fishDatabase == null) return null;
        return fishDatabase.GetRandomFish();
    }

    public float GetRandomBiteTime()
    {
        return Random.Range(biteTimeMin, biteTimeMax);
    }
}
