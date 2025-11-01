// 30/10/2025
// Script đã được viết lại để sử dụng rb.linearVelocity
// thay vì rb.MovePosition để di chuyển.

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movementInput; // Sẽ lưu input từ Update
    private Vector2 lastMovementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Rất quan trọng cho game top-down
        rb.gravityScale = 0;
        rb.linearDamping = 0f; // Đảm bảo không có lực cản không mong muốn
        rb.angularDamping = 0f;

        lastMovementDirection = new Vector2(0, -1f);
    }

    void Update()
    {
        // --- 1. Lấy Input (Chỉ lấy, không xử lý vật lý) ---
        // Lấy input trong Update để đảm bảo không bỏ lỡ phím nào
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // --- 2. Xử lý lật Sprite ---
        if (movementInput.x > 0.01f)
        {
            spriteRenderer.flipX = false; // Nhìn sang phải
        }
        else if (movementInput.x < -0.01f)
        {
            spriteRenderer.flipX = true; // Nhìn sang trái (lật sprite)
        }

        // --- 3. Cập nhật thông số Animator ---
        // (Animator vẫn chạy trong Update)
        float speed = movementInput.sqrMagnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            // Chỉ cập nhật hướng khi đang di chuyển
            animator.SetFloat("Horizontal", movementInput.x);
            animator.SetFloat("Vertical", movementInput.y);
            lastMovementDirection = movementInput.normalized;
        }

        // Luôn cập nhật hướng nhìn cuối cùng cho trạng thái Idle
        animator.SetFloat("LastMoveX", lastMovementDirection.x);
        animator.SetFloat("LastMoveY", lastMovementDirection.y);
    }

    void FixedUpdate()
    {
        // --- 4. Áp dụng di chuyển vật lý ---
        // (Tất cả vật lý nên nằm trong FixedUpdate)

        // Chuẩn hóa vector input để đi chéo không nhanh hơn
        Vector2 moveDirection = movementInput.normalized;

        // Gán vận tốc (velocity) trực tiếp cho Rigidbody
        rb.linearVelocity = moveDirection * moveSpeed;

        // Bây giờ Debug.Log cũ của bạn sẽ hoạt động
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            // Bạn sẽ thấy Log này nếu moveSpeed > 0
            Debug.Log("Player đang di chuyển với vận tốc: " + rb.linearVelocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Player đã va chạm với một vật cản!");
        }
    }
}