// 30/10/2025
// Xử lý logic tương tác của Player với môi trường (ô đất, NPC, v.v.)

using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // Giả lập việc chọn công cụ
    private enum SelectedTool
    {
        Hand, // Dùng để thu hoạch, dọn dẹp
        Hoe,  // Cuốc (UC-1.1)
        Seed  // Hạt giống (UC-1.2)
    }

    private SelectedTool currentTool = SelectedTool.Hand;

    [Header("Interaction")]
    public Transform interactionPoint; // Một Empty Object con của Player
    public float interactionRadius = 0.5f;
    public LayerMask interactableLayer; // Layer của các ô đất
    private Vector2 lastMoveDir = new Vector2(0, -1f); // Mặc định nhìn xuống

    [Header("Tool Simulation")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode toolKey_Hand = KeyCode.Alpha1;
    public KeyCode toolKey_Hoe = KeyCode.Alpha2;
    public KeyCode toolKey_Seed = KeyCode.Alpha3;

    // Giả lập có hạt giống cà chua để trồng
    public CropData testTomatoSeed;

    void Update()
    {
        // --- 1. Mô phỏng chọn công cụ ---
        if (Input.GetKeyDown(toolKey_Hand))
        {
            currentTool = SelectedTool.Hand;
            Debug.Log("Đã chọn: Tay (Thu hoạch)");
        }
        if (Input.GetKeyDown(toolKey_Hoe))
        {
            currentTool = SelectedTool.Hoe;
            Debug.Log("Đã chọn: Cuốc (Cày đất)");
        }
        if (Input.GetKeyDown(toolKey_Seed))
        {
            currentTool = SelectedTool.Seed;
            Debug.Log("Đã chọn: Hạt giống (Gieo hạt)");
        }

        // --- 2. Cập nhật vị trí điểm tương tác ---
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0)
        {
            // Cập nhật hướng nhìn (dùng cho điểm tương tác)
            lastMoveDir = new Vector2(moveX, moveY).normalized;
        }
        // Di chuyển điểm tương tác ra trước mặt player
        interactionPoint.position = transform.position + (Vector3)lastMoveDir * 0.5f; // 0.5f là offset


        // --- 3. Xử lý Tương tác (Nhấn phím) ---
        if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Collider2D hit = Physics2D.OverlapCircle(interactionPoint.position, interactionRadius, interactableLayer);

        if (hit == null) return;

        // 1️⃣ Nếu vật thể có IInteractable → gọi trực tiếp
        IInteractable interactable = hit.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Nếu là ô đất, ta vẫn dùng logic cũ (có tool)
            LandPlot plot = hit.GetComponent<LandPlot>();
            if (plot != null)
            {
                switch (currentTool)
                {
                    case SelectedTool.Hand:
                        plot.Harvest();
                        plot.ClearWithered();
                        break;
                    case SelectedTool.Hoe:
                        plot.Plow();
                        break;
                    case SelectedTool.Seed:
                        plot.Plant(testTomatoSeed);
                        break;
                }
            }
            else
            {
                // Nếu là vật thể nhặt được (PickupItem, v.v.)
                interactable.Interact();
            }

            return;
        }

        // 2️⃣ Nếu không có IInteractable, thử xem đây có phải item nằm trên đất không
        string baseName = hit.gameObject.name;
        ItemData item = ItemDataList.Instance.GetItemByName(baseName);

        if (item != null)
        {
            // Nếu tìm thấy item trùng tên trong danh sách
            if (!InventoryManager.Instance.CheckForSpace(item, 1))
            {
                Debug.LogWarning("Kho đầy, không thể nhặt " + item.itemName);
                return;
            }

            InventoryManager.Instance.AddItem(item, 1);
            Debug.Log("Nhặt được " + item.itemName);

            Destroy(hit.gameObject);
        }
    }

    // (Optional) Vẽ vòng tròn tương tác trong Scene view để dễ debug
    private void OnDrawGizmos()
    {
        if (interactionPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}