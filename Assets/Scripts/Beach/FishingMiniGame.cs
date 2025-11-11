using System;
using UnityEngine;
using UnityEngine.UI;

public class FishingMiniGame : MonoBehaviour
{
    [Header("References")]
    public RectTransform fishIcon;
    public RectTransform catchZone;
    public Slider progressBar;

    [Header("Gameplay Settings")]
    public float baseFishSpeed = 100f;     // cơ sở tốc độ (pixels/s)
    public float baseBarSpeed = 220f;
    public float baseZoneHeight = 80f;
    public float catchTarget = 100f;
    public float baseGainRate = 30f;
    public float baseLossRate = 15f;

    // Internal
    private float catchProgress = 25f;
    private float barY;
    private float fishY;
    private float fishTargetY;
    private float fishTimer;
    private float nextTargetTime;
    private float oscillationTimer;

    private float minY = -200f, maxY = 200f;

    // movement
    private float fishVelocity = 0f;

    // difficulty / rod
    private float fishDifficultyFactor; // 0..1 (1 = hardest)
    private float rodPowerFactor;       // 0..1 (1 = best)

    private float currentFishSpeed;

    private Action<bool> onFinish;
    private bool finished = false;

    private FishData fishData;
    private FishingRodData rodData;

    private void Start()
    {
        barY = 0f;
        fishY = UnityEngine.Random.Range(minY * 0.3f, maxY * 0.3f);
        fishTargetY = fishY;
        progressBar.value = catchProgress / catchTarget;
    }

    public void StartMiniGame(FishData fish, FishingRodData rod, Action<bool> callback)
    {
        onFinish = callback;
        fishData = fish;
        rodData = rod;

        // compute difficulty and rod factors
        float rawDifficulty = Mathf.Clamp(fish.difficulty, 1f, 20f);
        float rodReduction = (rod != null) ? rod.difficultyReduction + (rod.catchPower * 0.12f) : 0f;
        float effectiveDifficulty = Mathf.Clamp(rawDifficulty - rodReduction, 0.5f, 20f);
        fishDifficultyFactor = Mathf.Clamp01((effectiveDifficulty - 1f) / 19f); // 0..1

        rodPowerFactor = (rod != null) ? Mathf.Clamp01(rod.catchPower / 10f) : 0f;

        // currentFishSpeed: higher difficulty => faster; rod reduces speed but not fully
        float speedFromDifficulty = Mathf.Lerp(80f, 280f, fishDifficultyFactor); // px/s mapping
        float rodSlowFactor = Mathf.Lerp(1.0f, 0.6f, rodPowerFactor); // rod can slow up to 40%
        currentFishSpeed = baseFishSpeed * (speedFromDifficulty / 100f) * rodSlowFactor;

        // bar speed and zone size by rod
        baseBarSpeed = baseBarSpeed * (1f + rodPowerFactor * 0.12f);
        float zoneBoost = Mathf.Lerp(0f, 60f, rodPowerFactor); // up to +60px
        float finalZone = Mathf.Clamp(baseZoneHeight + zoneBoost, 40f, 260f);
        catchZone.sizeDelta = new Vector2(catchZone.sizeDelta.x, finalZone);

        // reset internal state, set starting positions closer to center
        catchProgress = Mathf.Clamp(catchProgress, 0f, catchTarget);
        fishVelocity = Mathf.Sign(UnityEngine.Random.value - 0.5f) * UnityEngine.Random.Range(30f, 80f); // non-zero start
        fishY = UnityEngine.Random.Range(minY * 0.4f, maxY * 0.4f);
        barY = fishY;
        fishTargetY = fishY;
        fishTimer = 0f;
        nextTargetTime = 0.05f; // force immediate target pick
        oscillationTimer = UnityEngine.Random.Range(0f, 10f);
        finished = false;
        progressBar.value = catchProgress / catchTarget;
    }

    private void Update()
    {
        if (finished) return;

        HandlePlayerInput();
        HandleFishMovement();
        UpdateProgress();
    }

    private void HandlePlayerInput()
    {
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            barY += baseBarSpeed * Time.deltaTime;
        else
            barY -= baseBarSpeed * Time.deltaTime * 1.6f;

        barY = Mathf.Clamp(barY, minY, maxY);
        catchZone.anchoredPosition = new Vector2(catchZone.anchoredPosition.x, barY);
    }

    private void HandleFishMovement()
    {
        oscillationTimer += Time.deltaTime;
        fishTimer += Time.deltaTime;

        // --- ĐỔI TARGET NGẪU NHIÊN ---
        float changeInterval = Mathf.Lerp(0.3f, 0.15f, fishDifficultyFactor); // khó hơn => đổi nhanh hơn
        if (fishTimer >= nextTargetTime)
        {
            float rodMod = Mathf.Lerp(1.1f, 0.7f, rodPowerFactor);
            nextTargetTime = changeInterval * rodMod;
            fishTimer = 0f;

            // Quãng đường di chuyển lớn hơn và min move range cao hơn
            float minMove = 240f; // tăng min range
            float maxMove = Mathf.Lerp(300f, 450f, fishDifficultyFactor); // max range difficulty 10
            float moveRange = UnityEngine.Random.Range(minMove, maxMove);
            float rodRangeMod = Mathf.Lerp(1.2f, 0.6f, rodPowerFactor);
            fishTargetY = Mathf.Clamp(fishY + UnityEngine.Random.Range(-moveRange, moveRange) * rodRangeMod, minY, maxY);
        }

        // --- TỐC ĐỘ DI CHUYỂN ---
        float toTarget = fishTargetY - fishY;
        float dir = Mathf.Sign(toTarget);

        float minSpeed = 300f; // min speed
        float maxSpeed = 700f; // max speed
        float desiredSpeed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Clamp01(Mathf.Abs(toTarget) / 200f));
        desiredSpeed *= Mathf.Lerp(1f, 0.65f, rodPowerFactor);

        float accel = Mathf.Lerp(500f, 2200f, fishDifficultyFactor) * Mathf.Lerp(1f, 0.6f, rodPowerFactor);
        fishVelocity = Mathf.MoveTowards(fishVelocity, desiredSpeed * dir, accel * Time.deltaTime);

        // --- JITTER + SIN ---
        float perlin = (Mathf.PerlinNoise(Time.time * (2f + fishDifficultyFactor * 4f), fishY * 0.01f) - 0.5f) * (10f + fishDifficultyFactor * 50f);
        float sinJitter = Mathf.Sin(oscillationTimer * (2f + fishDifficultyFactor * 5f)) * (4f + fishDifficultyFactor * 12f);
        float jitterReduce = Mathf.Lerp(1f, 0.5f, rodPowerFactor);
        perlin *= jitterReduce;
        sinJitter *= jitterReduce;

        // --- CẬP NHẬT VỊ TRÍ ---
        fishY += fishVelocity * Time.deltaTime + perlin * Time.deltaTime + sinJitter * Time.deltaTime;

        if (fishY < minY) { fishY = minY; fishVelocity = Mathf.Abs(fishVelocity); }
        if (fishY > maxY) { fishY = maxY; fishVelocity = -Mathf.Abs(fishVelocity); }

        fishIcon.anchoredPosition = new Vector2(fishIcon.anchoredPosition.x, fishY);

        // thêm trong cuối hàm
        if (UnityEngine.Random.value < 0.002f * fishDifficultyFactor)
        {
            fishVelocity += UnityEngine.Random.Range(-1200f, 1200f);
        }

        fishTargetY += (fishY - barY) * 0.2f * fishDifficultyFactor;

    }






    private void UpdateProgress()
    {
        // giảm harsh hơn, tăng cơ hội câu cá
        float gainMod = Mathf.Lerp(1.0f, 0.9f, fishDifficultyFactor); // giảm ít hơn
        float lossMod = Mathf.Lerp(1.0f, 1.1f, fishDifficultyFactor); // giảm bớt harsh

        float rodGainBoost = Mathf.Lerp(1f, 1.3f, rodPowerFactor);
        float rodLossReduce = Mathf.Lerp(1f, 0.9f, rodPowerFactor);

        float difficultyGap = Mathf.Clamp(fishData.difficulty - rodData.catchPower, 0f, 10f);
        float gapMultiplier = 1f + (difficultyGap / 8f); // giảm penalty hơn nữa

        float gain = baseGainRate * gainMod * rodGainBoost / gapMultiplier;
        float loss = baseLossRate * lossMod * rodLossReduce * gapMultiplier * 1.3f;

        // --- GIỚI HẠN LOSS THEO KHOẢNG CÁC VỊ TRÍ ---
        float diff = Mathf.Abs(fishY - barY);
        float halfZone = catchZone.sizeDelta.y * 0.5f;

        if (diff < halfZone)
        {
            catchProgress += gain * Time.deltaTime;
        }
        else
        {
            // giảm loss khi lệch ít, tăng dần khi lệch nhiều
            float factor = Mathf.Clamp01((diff - halfZone) / (halfZone * 2f)); // scale diff/zone *2 để tụt chậm hơn
            catchProgress -= loss * factor * Time.deltaTime;
        }

        catchProgress = Mathf.Clamp(catchProgress, 0f, catchTarget);
        progressBar.value = catchProgress / catchTarget;

        if (catchProgress >= catchTarget)
            Finish(true);
        else if (catchProgress <= 0f)
            Finish(false);
    }






    private void Finish(bool success)
    {
        finished = true;
        onFinish?.Invoke(success);
    }
}
