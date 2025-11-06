using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public enum ToolType { Hand, Hoe, Seed }
    public ToolType CurrentTool { get; private set; } = ToolType.Hand;

    [Header("Interaction")]
    public Transform interactionPoint;
    public float interactionRadius = 0.5f;
    public LayerMask interactableLayer;

    private Vector2 lastMoveDir = Vector2.down;

    private void Start()
    {
        // Lắng nghe khi hotbar đổi slot
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.OnSelectedChanged.AddListener(OnHotbarSelectedChanged);
    }

    private void OnDestroy()
    {
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.OnSelectedChanged.RemoveListener(OnHotbarSelectedChanged);
    }

    private void Update()
    {
        HandleInteractionPoint();
        HandleInteract();
    }

    void HandleInteractionPoint()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0)
            lastMoveDir = new Vector2(moveX, moveY).normalized;

        if (interactionPoint != null)
            interactionPoint.position = transform.position + (Vector3)lastMoveDir * 0.5f;
    }

    void HandleInteract()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        Collider2D hit = Physics2D.OverlapCircle(interactionPoint.position, interactionRadius, interactableLayer);
        if (hit == null) return;

        IInteractable interactable = hit.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(this);
        }
    }

    void OnHotbarSelectedChanged(int index)
    {
        var stack = HotbarManager.Instance.GetSelectedStack();

        if (stack == null || stack.IsEmpty)
        {
            CurrentTool = ToolType.Hand;
            return;
        }

        // Chuyển ItemType → ToolType
        switch (stack.item.type)
        {
            case ItemType.Tool_Hoe:
                CurrentTool = ToolType.Hoe;
                break;
            case ItemType.Seed:
                CurrentTool = ToolType.Seed;
                break;
            default:
                CurrentTool = ToolType.Hand;
                break;
        }

        Debug.Log($"🔧 Tool hiện tại: {CurrentTool}");
    }
}
