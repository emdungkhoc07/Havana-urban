using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("speeeed")]
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private InputSystem_Actions inputActions;

    [Header("mat duong")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4.5f;
    [SerializeField] private float maxY = -2.5f;


    [Header("zoom xa gan")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.2f;

    [Header("nhay")]
    [SerializeField] private float jumpPower = 8f; 
    [SerializeField] private float gravity = 20f; 
    private float verticalVelocity = 0f;
    private float jumpHeight = 0f;
    private bool isGrounded = true;
    private Vector2 groundPosition; // vi tri chan tren mat duong

    [Header("=== BẮN ĐẠN (CHUỘT PHẢI) ===")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float shootCooldown = 0.25f;
    private float lastShootTime = -99f;
    private float facingDirection = 1f; // 1: phải (D), -1: trái (A)
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new InputSystem_Actions();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        groundPosition = transform.position;
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // 1. Ghi nhận hướng A hay D được bấm sau cùng
        if (moveInput.x > 0.05f)
        {
            facingDirection = 1f; // D bấm sau cùng -> quay sang phải
        }
        else if (moveInput.x < -0.05f)
        {
            facingDirection = -1f; // A bấm sau cùng -> quay sang trái
        }

        // 2. Di chuyển chân trên mặt đường
        groundPosition += moveInput * moveSpeed * Time.deltaTime;
        groundPosition.x = Mathf.Clamp(groundPosition.x, minX, maxX);
        groundPosition.y = Mathf.Clamp(groundPosition.y, minY, maxY);

        // 3. Nhảy
        if (inputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            verticalVelocity = jumpPower; 
            isGrounded = false;  
        }

        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;     // Giảm dần vận tốc
            jumpHeight += verticalVelocity * Time.deltaTime; // Thay đổi độ cao
            
            // Khi độ cao rơi về 0 -> Tiếp đất!
            if (jumpHeight <= 0f)
            {
                jumpHeight = 0f;
                isGrounded = true;
            }
        }

        // 4. Bắn đạn bằng Chuột Phải
        if (inputActions.Player.Attack.WasPressedThisFrame() && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
        }

        transform.position = new Vector3(groundPosition.x, groundPosition.y + jumpHeight, transform.position.z);
        UpdateDepthScale(); // ham tinh phong to thu nho nhan vat
    }

    private void Shoot()
    {
        lastShootTime = Time.time;

        // Sinh viên đạn từ Prefab (Instantiate)
        if (bulletPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(facingDirection * 0.6f, 0f, 0f);
            GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            if (bulletObj.TryGetComponent<Bullet>(out var bullet))
            {
                bullet.Setup(facingDirection, enemyLayer);
            }
        }

        // Nháy đỏ nhân vật khi bắn (0.12s)
        StartCoroutine(FlashRedRoutine());
    }

    private IEnumerator FlashRedRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.12f);
            spriteRenderer.color = originalColor;
        }
    }

    // Nhận lực đẩy lùi từ quái vật khi bị tấn công
    public void ApplyKnockback(Vector2 direction, float force)
    {
        groundPosition += direction * force;
        groundPosition.x = Mathf.Clamp(groundPosition.x, minX, maxX);
        groundPosition.y = Mathf.Clamp(groundPosition.y, minY, maxY);
        transform.position = new Vector3(groundPosition.x, groundPosition.y + jumpHeight, transform.position.z);

        Debug.Log("Player bị quái tấn công đẩy lùi!");
    }

    private void UpdateDepthScale()
    {
        // Tính theo vị trí chân trên mặt đường:
        float t = Mathf.InverseLerp(minY, maxY, groundPosition.y);
        float currentScale = Mathf.Lerp(maxScale, minScale, t);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }

    private void OnDrawGizmos() // ve duong
    {
        Gizmos.color = Color.blue;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}
