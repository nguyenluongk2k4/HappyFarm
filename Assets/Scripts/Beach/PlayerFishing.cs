// Assets/Scripts/Fishing/PlayerFishing.cs
using UnityEngine;
using System.Collections;

public class PlayerFishing : MonoBehaviour
{
    public FishingRodData currentRod;
    public PlayerInventory inventory;

    private FishingSpot currentSpot;
    private bool inRange = false;
    private bool isFishing = false;

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && !isFishing)
        {
            StartCoroutine(DoFishing());
        }
    }

    IEnumerator DoFishing()
    {
        isFishing = true;
        Debug.Log("Đã bắt đầu câu...");

        yield return StartCoroutine(FishingManager.Instance.StartFishingRoutine(currentSpot, currentRod, OnFishResult));

        // chờ một chút trước khi cho người chơi move hoặc tiếp tục
        yield return new WaitForSeconds(0.3f);
        isFishing = false;
    }

    void OnFishResult(FishData fish, bool caught)
    {
        if (fish == null)
        {
            Debug.Log("Không có cá.");
            return;
        }

        if (caught)
        {
            Debug.Log($"🎣 Bắt được {fish.fishName} (Giá {fish.sellPrice})");
            inventory?.AddFish(fish);
            // Hiện UI, chơi animation, âm thanh...
        }
        else
        {
            Debug.Log($"🐟 {fish.fishName} sổng mất!");
            // Hiện UI "sổng"
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out FishingSpot spot))
        {
            currentSpot = spot;
            inRange = true;
            // show prompt "Bấm E để câu"
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out FishingSpot spot) && spot == currentSpot)
        {
            currentSpot = null;
            inRange = false;
            // hide prompt
        }
    }
}
