using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FishingSpot : MonoBehaviour
{
    [Header("Thông tin khu vực câu cá")]
    public string spotName = "Sea Spot";
    public FishData[] availableFish;

    [Header("Thời gian chờ cá cắn câu (giây)")]
    public float biteTimeMin = 2f;
    public float biteTimeMax = 5f;

    [Tooltip("Tăng hoặc giảm cơ hội cá hiếm ở khu vực này (1 = bình thường)")]
    public float spotRarityModifier = 1f;

    private Collider2D fishingCollider;

    private void Reset()
    {
        fishingCollider = GetComponent<Collider2D>();
        fishingCollider.isTrigger = true; // Trigger để detect player
    }

    private void Awake()
    {
        fishingCollider = GetComponent<Collider2D>();
    }

    // Lấy cá ngẫu nhiên
    public FishData GetRandomFish(FishingRodData rod)
    {
        if (availableFish == null || availableFish.Length == 0) return null;

        float rodPower = rod != null ? rod.catchPower : 1f;
        float totalWeight = 0f;
        float[] weights = new float[availableFish.Length];

        for (int i = 0; i < availableFish.Length; i++)
        {
            FishData fish = availableFish[i];

            // base rarity
            float w = Mathf.Max(0.0001f, fish.rarity) * spotRarityModifier;

            // --- XÁC ĐỊNH NHÓM ĐỘ KHÓ THEO CẦN CÂU ---
            // nhóm trung tâm theo rodPower
            float targetMin = 1f, targetMax = 4f;
            if (rodPower >= 3.5f)
            {
                targetMin = 8f; targetMax = 10f;
            }
            else if (rodPower >= 2f)
            {
                targetMin = 4f; targetMax = 7f;
            }

            // tính factor dựa vào khoảng cách đến trung tâm nhóm
            float groupCenter = (targetMin + targetMax) / 2f;
            float distance = Mathf.Abs(fish.difficulty - groupCenter);

            // weight giảm dần khi xa nhóm, factor 0.5–1 tùy distance
            float maxDistance = (targetMax - targetMin) / 2f;
            float factor = Mathf.Clamp01(1f - (distance / maxDistance)); // 0..1
            factor = Mathf.Lerp(0.1f, 1f, factor); // tránh 0 hoàn toàn, luôn có chance nhỏ

            w *= factor;
            weights[i] = w;
            totalWeight += w;
        }

        // Random dựa trên weights
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < availableFish.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative) return availableFish[i];
        }

        return availableFish[availableFish.Length - 1];
    }


    // Thời gian chờ cá cắn
    public float GetRandomBiteTime(FishingRodData rod)
    {
        float baseTime = Random.Range(biteTimeMin, biteTimeMax);
        if (rod != null) baseTime *= rod.biteTimeModifier;
        return baseTime;
    }

    // Kiểm tra player có trong vùng câu hay không
    public bool IsPlayerInRange(Transform player)
    {
        return fishingCollider.OverlapPoint(player.position);
    }

    // Vẽ vùng câu cá trong Scene (tuỳ chọn)
    private void OnDrawGizmosSelected()
    {
        if (fishingCollider is PolygonCollider2D poly)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < poly.points.Length; i++)
            {
                Vector2 p1 = transform.TransformPoint(poly.points[i]);
                Vector2 p2 = transform.TransformPoint(poly.points[(i + 1) % poly.points.Length]);
                Gizmos.DrawLine(p1, p2);
            }
        }
    }
}
