// Assets/Scripts/Fishing/FishDatabase.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewFishDatabase", menuName = "Fishing/Fish Database")]
public class FishDatabase : ScriptableObject
{
    public FishData[] fishList;

    // Lấy 1 con cá random theo rarity (weight)
    public FishData GetRandomFish()
    {
        if (fishList == null || fishList.Length == 0) return null;
        float total = 0f;
        foreach (var f in fishList) total += Mathf.Max(0.0001f, f.rarity);

        float r = Random.Range(0f, total);
        float cum = 0f;
        foreach (var f in fishList)
        {
            cum += Mathf.Max(0.0001f, f.rarity);
            if (r <= cum) return f;
        }
        return fishList[fishList.Length - 1];
    }
}
