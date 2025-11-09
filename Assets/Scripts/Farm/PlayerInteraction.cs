using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public enum ToolType { Hand, Hoe, Seed }
    public ToolType CurrentTool { get; private set; } = ToolType.Hand;

    [Header("Interaction")]
    public float interactionRadius = 0.5f;
    public LayerMask interactableLayer;

    [Header("Hoe Settings")]
    public GameObject landPlotPrefab;
    public int hoeHitsNeeded = 3;
    private int hoeHitCount = 0;
    private Vector2 lastHoePosition;
    private bool willSpawnLand = false;

    private Animator animator;
    private Vector2 facingDirection = Vector2.down; // hướng nhìn của player

    private void Start()
    {
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.OnSelectedChanged.AddListener(OnHotbarSelectedChanged);

        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.OnSelectedChanged.RemoveListener(OnHotbarSelectedChanged);
    }

    private void Update()
    {
        UpdateFacingDirection();
        HandleInteract();
    }

    // ✅ Lưu hướng nhìn player khi di chuyển
    void UpdateFacingDirection()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
            facingDirection = new Vector2(moveX, moveY).normalized;
    }

    void HandleInteract()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        // === CUỐC ĐẤT ===
        if (CurrentTool == ToolType.Hoe)
        {
            Vector2 spawnPos = (Vector2)transform.position + facingDirection * 1f;
            spawnPos = RoundToGrid(spawnPos);

            // Nếu cuốc vị trí mới -> reset counter
            if (spawnPos != lastHoePosition)
            {
                hoeHitCount = 0;
                lastHoePosition = spawnPos;
            }

            hoeHitCount++;
            Debug.Log($"⛏ Cuốc đất {hoeHitCount}/{hoeHitsNeeded} tại {spawnPos}");

            if (hoeHitCount >= hoeHitsNeeded)
            {
                willSpawnLand = true;
                hoeHitCount = 0;
            }

            animator.SetTrigger("trig_Hoe"); // phát animation cuốc
            return;
        }

        // === TƯƠNG TÁC THƯỜNG ===
        Collider2D hit = Physics2D.OverlapCircle(transform.position + (Vector3)facingDirection * 0.5f, interactionRadius, interactableLayer);
        hit?.GetComponent<IInteractable>()?.Interact(this);
    }

    // ✅ Được gọi từ Animation Event ở cuối animation cuốc
    public void OnHoeAnimationEnd()
    {
        if (willSpawnLand)
        {
            SpawnLandPlot(lastHoePosition);
            willSpawnLand = false;
        }
    }

    void OnHotbarSelectedChanged(int index)
    {
        var stack = HotbarManager.Instance.GetSelectedStack();

        if (stack == null || stack.IsEmpty)
        {
            SetTool(ToolType.Hand);
            return;
        }

        switch (stack.item.type)
        {
            case ItemType.Tool_Hoe:
                SetTool(ToolType.Hoe);
                break;
            case ItemType.Seed:
                SetTool(ToolType.Seed);
                break;
            default:
                SetTool(ToolType.Hand);
                break;
        }

        Debug.Log($"🔧 Tool hiện tại: {CurrentTool}");
    }

    void SetTool(ToolType tool)
    {
        CurrentTool = tool;
        animator.SetBool("bool_Hoe", tool == ToolType.Hoe);
    }

    void SpawnLandPlot(Vector2 position)
    {
        Collider2D existing = Physics2D.OverlapCircle(position, 0.2f, interactableLayer);
        if (existing != null && existing.GetComponent<LandPlot>())
        {
            Debug.Log("⚠ Ở đây đã có đất rồi!");
            return;
        }

        Instantiate(landPlotPrefab, position, Quaternion.identity);
        Debug.Log("🌱 Đất mới đã được tạo!");
    }

    Vector2 RoundToGrid(Vector2 pos)
    {
        return new Vector2(
            Mathf.Round(pos.x),
            Mathf.Round(pos.y)
        );
    }
}
