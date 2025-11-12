using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chicken : MonoBehaviour
{
    [Header("Growth Settings")]
    public List<Sprite> growthSprites;
    public float timeToGrow = 20f;
    private float growTimer = 0f;
    private int currentGrowthStage = 0;
    private bool isAdult = false;
    private SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    public bool moveLeft = true;

    [Header("Egg Settings")]
    public GameObject eggPrefab;
    public Transform eggSpawnPoint;
    public float minLayDelay = 5f;
    public float maxLayDelay = 10f;

    private Vector3 startPos;
    private float nextLayTime;
    private bool isLayingEgg = false;
    private Animator animator;

    // ✅ NEW: Dữ liệu lưu trạng thái
    [System.Serializable]
    public class ChickenSaveData
    {
        public Vector3 position;
        public float growTimer;
        public int currentGrowthStage;
        public bool isAdult;
        public bool moveLeft;
        public float nextLayTime;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPos = transform.position;
        SetNextLayTime();

        if (growthSprites.Count > 0)
            spriteRenderer.sprite = growthSprites[0];
    }

    void Update()
    {
        if (!isAdult)
        {
            Grow();
        }

        if (!isLayingEgg)
        {
            Move();
        }

        if (isAdult && Time.time >= nextLayTime && !isLayingEgg)
        {
            StartCoroutine(LayEggRoutine());
        }
    }

    // Giai đoạn phát triển
    void Grow()
    {
        if (growthSprites.Count == 0) return;

        growTimer += Time.deltaTime;
        float stageDuration = timeToGrow / growthSprites.Count;
        int newStage = Mathf.FloorToInt(growTimer / stageDuration);

        if (newStage != currentGrowthStage && newStage < growthSprites.Count)
        {
            currentGrowthStage = newStage;
            spriteRenderer.sprite = growthSprites[currentGrowthStage];
        }

        if (growTimer >= timeToGrow)
        {
            animator.SetBool("bool_HasGrowth", true);
            isAdult = true;
        }
    }

    void Move()
    {
        float direction = moveLeft ? -1 : 1;
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= moveDistance)
        {
            moveLeft = !moveLeft;
            startPos = transform.position;
            transform.localScale = new Vector3(moveLeft ? 1 : -1, 1, 1);
        }
    }

    private IEnumerator LayEggRoutine()
    {
        isLayingEgg = true;
        animator.SetBool("bool_LayEgg", true);

        float stopTime = Random.Range(1f, 3f);
        yield return new WaitForSeconds(stopTime);

        if (eggPrefab != null && eggSpawnPoint != null)
        {
            Instantiate(eggPrefab, eggSpawnPoint.position, Quaternion.identity);
            Debug.Log("🥚 Gà đã đẻ trứng!");
        }

        SetNextLayTime();
        isLayingEgg = false;
        animator.SetBool("bool_LayEgg", false);
    }

    void SetNextLayTime()
    {
        nextLayTime = Time.time + Random.Range(minLayDelay, maxLayDelay);
    }

    // ======================== SAVE / LOAD ========================

    public ChickenSaveData SaveState()
    {
        ChickenSaveData data = new ChickenSaveData();
        data.position = transform.position;
        data.growTimer = growTimer;
        data.currentGrowthStage = currentGrowthStage;
        data.isAdult = isAdult;
        data.moveLeft = moveLeft;
        data.nextLayTime = nextLayTime;
        return data;
    }

    public void LoadState(ChickenSaveData data)
    {
        transform.position = data.position;
        growTimer = data.growTimer;
        currentGrowthStage = data.currentGrowthStage;
        isAdult = data.isAdult;
        moveLeft = data.moveLeft;
        nextLayTime = data.nextLayTime;

        // Cập nhật sprite tương ứng với stage
        if (growthSprites.Count > 0 && currentGrowthStage < growthSprites.Count)
            spriteRenderer.sprite = growthSprites[currentGrowthStage];

        // Nếu đã trưởng thành → bật animation trưởng thành
        animator.SetBool("bool_HasGrowth", isAdult);

        // Cập nhật hướng di chuyển
        transform.localScale = new Vector3(moveLeft ? 1 : -1, 1, 1);

        Debug.Log($"✅ Đã load lại trạng thái gà (stage {currentGrowthStage}, adult={isAdult})");
    }
}
