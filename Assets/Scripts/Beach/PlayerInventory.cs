// Assets/Scripts/Fishing/PlayerInventory.cs
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<FishData> caughtFish = new List<FishData>();

    public void AddFish(FishData fish)
    {
        if (fish == null) return;
        caughtFish.Add(fish);
        // có thể spawn UI popup hiển thị fish.icon, tên, giá
    }

    public int SellAll()
    {
        int total = 0;
        foreach (var f in caughtFish) total += f.sellPrice;
        caughtFish.Clear();
        Debug.Log($"Bạn bán hết cá, nhận {total} vàng.");
        return total;
    }
}
