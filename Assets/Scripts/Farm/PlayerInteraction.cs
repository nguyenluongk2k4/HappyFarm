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
        // Tìm kiếm tất cả đối tượng trong vòng tròn tương tác
        Collider2D hit = Physics2D.OverlapCircle(interactionPoint.position, interactionRadius, interactableLayer);

        if (hit != null)
        {
            // Đã tìm thấy một ô đất, thử lấy script LandPlot
            LandPlot plot = hit.GetComponent<LandPlot>();
            if (plot != null)
            {
                // Gửi lệnh tương tác đến ô đất
                // Ô đất sẽ tự quyết định làm gì (UC-1.1 -> 1.4)
                switch (currentTool)
                {
                    case SelectedTool.Hand:
                        // Nếu là tay, thử Thu hoạch (UC-1.3)
                        plot.Harvest();
                        // Nếu không thu hoạch được, thử Dọn dẹp (UC-1.4)
                        plot.ClearWithered();
                        break;

                    case SelectedTool.Hoe:
                        // Nếu là cuốc, thử Cày (UC-1.1)
                        plot.Plow();
                        break;

                    case SelectedTool.Seed:
                        // Nếu là hạt giống, thử Gieo hạt (UC-1.2)
                        // (Gửi kèm data của hạt giống đang cầm)
                        plot.Plant(testTomatoSeed);
                        break;
                }
            }
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