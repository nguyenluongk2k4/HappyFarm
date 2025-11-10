// Assets/Scripts/Fishing/FishingManager.cs
using System.Collections;
using UnityEngine;
using System;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Bắt đầu quá trình câu cá
    /// </summary>
    /// <param name="spot">Điểm câu (FishingSpot)</param>
    /// <param name="rod">Cần câu (FishingRodData)</param>
    /// <param name="resultCallback">Gọi lại khi câu xong: (FishData, caught?)</param>
    public IEnumerator StartFishingRoutine(FishingSpot spot, FishingRodData rod, Action<FishData, bool> resultCallback)
    {
        if (spot == null || rod == null)
        {
            resultCallback?.Invoke(null, false);
            yield break;
        }

        // 1️⃣ Chọn cá ngẫu nhiên tại điểm câu
        FishData fish = spot.GetRandomFish();
        if (fish == null)
        {
            resultCallback?.Invoke(null, false);
            yield break;
        }

        // 2️⃣ Tính thời gian chờ cá cắn câu
        // base bite time * rarity * modifier => cá hiếm cắn lâu hơn
        float baseBite = spot.GetRandomBiteTime();
        float finalBiteTime = baseBite * Mathf.Clamp(fish.rarity * 1.0f + fish.difficulty * 0.02f, 0.5f, 6f);
        finalBiteTime *= rod.biteTimeModifier;

        Debug.Log($"[Fishing] Waiting {finalBiteTime:F2}s for {fish.fishName}");
        yield return new WaitForSeconds(finalBiteTime);

        // 3️⃣ Tính xác suất bắt được cá
        float effectiveDifficulty = Mathf.Max(1f, fish.difficulty - rod.difficultyReduction);
        float randomFactor = UnityEngine.Random.Range(0.85f, 1.15f); // chút ngẫu nhiên
        float rawChance = (rod.catchPower * randomFactor) / effectiveDifficulty;
        float catchChance = Mathf.Clamp01(rawChance / 1.2f); // chuẩn hóa về 0..1

        Debug.Log($"[Fishing] Fish: {fish.fishName}, Difficulty: {fish.difficulty}, Rod Power: {rod.catchPower}, Chance: {catchChance:F2}");

        // 4️⃣ Kiểm tra kết quả
        bool caught = UnityEngine.Random.value <= catchChance;

        // 5️⃣ Gọi callback trả về kết quả
        resultCallback?.Invoke(fish, caught);
    }
}
