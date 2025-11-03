using UnityEngine;

public class SoftFollowCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Kéo Player vào đây

    [Header("Dead Zone Settings")]
    public float smoothTime = 0.3f;
    public Vector2 deadZoneSize; // Kích thước của "vùng chết"
    public float zOffset = -10f; // Giữ khoảng cách Z

    // --- THÊM MỚI: Cài đặt giới hạn (Limits) ---
    [Header("Camera Limits")]
    public bool useLimits = true; // Tick để bật giới hạn
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    // --- Kết thúc thêm mới ---

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
                Debug.Log("Camera đã tự động tìm thấy và theo dõi Player!");
            }
            else
            {
                Debug.LogWarning("Camera không tìm thấy đối tượng nào có tag 'Player' trong Scene!");
            }
        }
    }
    private void OnDrawGizmos()
    {
        // 1. Vẽ vùng Dead Zone (Màu đỏ)
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));

        // 2. Vẽ vùng giới hạn camera (Màu xanh) - THÊM MỚI
        if (useLimits)
        {
            Gizmos.color = new Color(0, 0, 1, 0.3f); // Màu xanh
            float sizeX = maxX - minX;
            float sizeY = maxY - minY;
            float centerX = minX + sizeX / 2f;
            float centerY = minY + sizeY / 2f;
            Gizmos.DrawWireCube(new Vector3(centerX, centerY, zOffset), new Vector3(sizeX, sizeY, 0));
        }
    }

    void LateUpdate()
    {
        // Nếu target bằng null, tìm object có tag "Player"
        if (target == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
                Debug.Log("Camera đã tìm thấy Player: " + target.name);
            }
            else
            {
                return; // Không tìm thấy Player, thoát
            }
        }

        Vector3 cameraPos = transform.position;
        Vector3 targetPos = target.position;
        Vector3 desiredPosition = cameraPos;

        // --- 1-4. Logic Cốt Lõi Của Dead Zone (như cũ) ---
        float deltaX = targetPos.x - cameraPos.x;
        float deltaY = targetPos.y - cameraPos.y;
        float halfDeadZoneX = deadZoneSize.x / 2f;
        float halfDeadZoneY = deadZoneSize.y / 2f;

        if (deltaX > halfDeadZoneX)
            desiredPosition.x = targetPos.x - halfDeadZoneX;
        else if (deltaX < -halfDeadZoneX)
            desiredPosition.x = targetPos.x + halfDeadZoneX;

        if (deltaY > halfDeadZoneY)
            desiredPosition.y = targetPos.y - halfDeadZoneY;
        else if (deltaY < -halfDeadZoneY)
            desiredPosition.y = targetPos.y + halfDeadZoneY;

        // --- 5. Áp dụng Giới Hạn (THÊM MỚI) ---
        // Kẹp (Clamp) vị trí mong muốn (desiredPosition) 
        // vào bên trong các mốc giới hạn.
        if (useLimits)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // --- 6. Đặt Z Offset (như cũ) ---
        desiredPosition.z = zOffset;

        // --- 7. Di chuyển Camera (như cũ) ---
        // Vị trí mới sẽ là vị trí đã được kẹp (clamped)
        transform.position = Vector3.SmoothDamp(cameraPos, desiredPosition, ref velocity, smoothTime);
    }
}