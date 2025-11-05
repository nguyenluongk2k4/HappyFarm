using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chicken : MonoBehaviour
{
    [Header("Growth Settings")]
    public List<Sprite> growthSprites; // Danh sách sprite (nhỏ -> to)
    public float timeToGrow = 20f;     // Tổng thời gian lớn lên (giây)
    private float growTimer = 0f;
    private int currentGrowthStage = 0;
    private bool isAdult = false;      // Đánh dấu gà đã trưởng thành
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

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPos = transform.position;
        SetNextLayTime();

        if (growthSprites.Count > 0)
        {
            spriteRenderer.sprite = growthSprites[0];
        }
    }

    void Update()
    {
        // 1️⃣ Quá trình lớn lên
        if (!isAdult)
        {
            Grow();
          
        }

        // 2️⃣ Khi đã trưởng thành: di chuyển và đẻ trứng
        if (!isLayingEgg)
        {
            Move();
        }

        if (isAdult&&Time.time >= nextLayTime && !isLayingEgg)
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

        // Khi đủ lớn, chuyển sang trạng thái trưởng thành
        if (growTimer >= timeToGrow)
        {
            animator.SetBool("bool_HasGrowth",true);
            isAdult = true;
        }
    }

    // Di chuyển qua lại
    void Move()
    {
        float direction = moveLeft ? -1 : 1;
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= moveDistance)
        {
            moveLeft = !moveLeft;
            animator.SetBool("bool_MoveLeft", moveLeft);
            startPos = transform.position;
            transform.localScale = new Vector3(moveLeft ? 1 : -1, 1, 1);
        }
    }

    // Đẻ trứng
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
}
