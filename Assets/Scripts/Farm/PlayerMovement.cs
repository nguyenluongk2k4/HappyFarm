using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Cấu hình di chuyển")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movementInput;
    private Vector2 lastMovementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ép chắc chắn Dynamic nếu ai đó đặt nhầm
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        lastMovementDirection = new Vector2(0, -1f);

        //Debug.Log("Rigidbody2D bodyType = " + rb.bodyType);
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
       
        if (movementInput.x > 0.01f) spriteRenderer.flipX = false;
        else if (movementInput.x < -0.01f) spriteRenderer.flipX = true;

        float speed = movementInput.sqrMagnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            animator.SetFloat("Horizontal", movementInput.x);
            animator.SetFloat("Vertical", movementInput.y);
            lastMovementDirection = movementInput.normalized;
        }

        animator.SetFloat("LastMoveX", lastMovementDirection.x);
        animator.SetFloat("LastMoveY", lastMovementDirection.y);
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = movementInput.normalized;
        rb.linearVelocity = moveDirection * moveSpeed;

        //if (rb.linearVelocity.sqrMagnitude > 0.01f) ;
            //Debug.Log("Player đang di chuyển với vận tốc: " + rb.linearVelocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Obstacle"))
            //Debug.Log("Player đã va chạm với vật cản: " + collision.gameObject.name);
    }
}
