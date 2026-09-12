using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("=== THÔNG SỐ CƠ BẢN ===")]
    [SerializeField] private int maxHealth = 60;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float knockbackForce = 2.5f;

    [Header("=== TẦM NHÌN (RAYCAST 2D & LAYER MASK) ===")]
    [SerializeField] private float visionRange = 6f;
    [SerializeField] private LayerMask playerLayer;

    [Header("=== PHẠM VI MẶT ĐƯỜNG (2.5D) ===")]
    [SerializeField] private float minX = -9f;
    [SerializeField] private float maxX = 9f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = -3.5f;

    [Header("=== ZOOM XA GẦN (DEPTH SCALE) ===")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.2f;

    private int currentHealth;
    private Transform player;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isAttacking = false;
    private float lastAttackTime = -99f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // Tìm Player theo Tag đúng theo bài học
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (player == null) return;

        // 1. Kiểm tra tầm nhìn bằng Raycast 2D và LayerMask
        if (CanSeePlayer() && !isAttacking)
        {
            // 2. Di chuyển tiếp cận Player
            MoveTowardsPlayer();
        }

        // 3. Giữ quái trên mặt đường bằng Mathf.Clamp
        ClampToRoadArea();

        // 4. Zoom xa gần bằng Mathf.Lerp
        UpdateDepthScale();
    }

    // Bắn tia Raycast 2D về phía Player sử dụng LayerMask
    private bool CanSeePlayer()
    {
        Vector2 direction = (player.position - transform.position);
        float distance = direction.magnitude;

        if (distance <= visionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, visionRange, playerLayer);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Quay mặt theo hướng trái/phải
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void ClampToRoadArea()
    {
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void UpdateDepthScale()
    {
        float t = Mathf.InverseLerp(minY, maxY, transform.position.y);
        float currentScale = Mathf.Lerp(maxScale, minScale, t);
        float signX = Mathf.Sign(transform.localScale.x);
        transform.localScale = new Vector3(signX * currentScale, currentScale, 1f);
    }

    // Tấn công khi chạm vào Player bằng Trigger 2D
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Phát sáng nhẹ 0.5s bằng cách đổi màu sáng
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 0.2f, 1f);
        }

        // Đẩy lùi Player theo hướng ngược lại
        Vector2 knockbackDir = (player.position - transform.position).normalized;
        if (playerController != null)
        {
            playerController.ApplyKnockback(knockbackDir, knockbackForce);
            Debug.Log("Quái tấn công gây " + attackDamage + " sát thương!");
        }

        yield return new WaitForSeconds(0.5f);

        // Trở về màu ban đầu
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isAttacking = false;
    }

    // Nhận sát thương khi trúng đạn
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Quái dính đạn, máu còn: " + currentHealth);

        StartCoroutine(FlashWhiteRoutine());

        // Hết máu -> Xóa quái (Destroy)
        if (currentHealth <= 0)
        {
            Debug.Log("Quái bị tiêu diệt!");
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashWhiteRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    // Vẽ tầm nhìn bằng Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
