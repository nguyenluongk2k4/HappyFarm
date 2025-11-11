using System;
using System.Collections;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;
    public FishingMiniGame miniGamePrefab; // gán trong Inspector
    public Canvas fishingCanvas;            // Canvas chứa UI minigame

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator StartFishingRoutine(FishingSpot spot, FishingRodData rod, Action<FishData, bool> resultCallback)
    {
        if (spot == null || rod == null)
        {
            resultCallback?.Invoke(null, false);
            yield break;
        }

        // 1️⃣ Chọn cá ngẫu nhiên dựa vào cần câu
        FishData fish = spot.GetRandomFish(rod);
        if (fish == null)
        {
            resultCallback?.Invoke(null, false);
            yield break;
        }

        // 2️⃣ Thời gian chờ cá cắn câu (cần tốt → nhanh hơn)
        float baseBite = spot.GetRandomBiteTime(rod);
        Debug.Log($"[Fishing] Waiting {baseBite:F2}s for {fish.fishName}");
        yield return new WaitForSeconds(baseBite);

        // 3️⃣ Cá cắn câu → bật mini-game
        Transform parent = fishingCanvas != null ? fishingCanvas.transform : null;
        FishingMiniGame mini = Instantiate(miniGamePrefab, parent);

        bool caught = false;
        bool done = false;

        mini.StartMiniGame(fish, rod, (success) =>
        {
            caught = success;
            done = true;
        });

        yield return new WaitUntil(() => done);

        resultCallback?.Invoke(fish, caught);
        Destroy(mini.gameObject);
    }
}
