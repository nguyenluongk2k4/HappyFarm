using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public enum ToolType { Hand, Hoe, Seed }
    public ToolType CurrentTool { get; private set; } = ToolType.Hand;

    [Header("Interaction")]
    public Transform interactionPoint;
    public float interactionRadius = 0.5f;
    public LayerMask interactableLayer;

    [Header("Input Keys")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode handKey = KeyCode.Alpha1;
    public KeyCode hoeKey = KeyCode.Alpha2;
    public KeyCode seedKey = KeyCode.Alpha3;

    [Header("Testing")]
    public CropData testTomatoSeed;

    private Vector2 lastMoveDir = Vector2.down;

    void Update()
    {
        HandleToolSelect();
        HandleInteractionPoint();
        HandleInteract();
    }

    void HandleToolSelect()
    {
        if (Input.GetKeyDown(handKey)) { CurrentTool = ToolType.Hand; Debug.Log("Chọn tay"); }
        if (Input.GetKeyDown(hoeKey)) { CurrentTool = ToolType.Hoe; Debug.Log("Chọn cuốc"); }
        if (Input.GetKeyDown(seedKey)) { CurrentTool = ToolType.Seed; Debug.Log("Chọn hạt giống"); }
    }

    void HandleInteractionPoint()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0)
            lastMoveDir = new Vector2(moveX, moveY).normalized;

        interactionPoint.position = transform.position + (Vector3)lastMoveDir * 0.5f;
    }

    void HandleInteract()
    {
        if (!Input.GetKeyDown(interactKey)) return;

        Collider2D hit = Physics2D.OverlapCircle(interactionPoint.position, interactionRadius, interactableLayer);
        if (hit == null) return;

        IInteractable interactable = hit.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact(this);
        }
        else
        {
            Debug.Log("Không thể tương tác với " + hit.name);
        }
    }

    private void OnDrawGizmos()
    {
        if (interactionPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}
