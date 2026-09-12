using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("setting")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 25;
    [SerializeField] private float lifeTime = 3f; // tu bien mat sau 3s
    [SerializeField] private LayerMask enemyLayer;

    private float direction = 1f; // 1: bay sang phải, -1: bay sang trái

    // Hàm khởi tạo hướng bay khi đạn được sinh ra (Instantiate)
    public void Setup(float shootDirection, LayerMask targetLayer)
    {
        direction = Mathf.Sign(shootDirection);
        enemyLayer = targetLayer;

        if (direction < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        Vector2 moveVector = new Vector2(direction * moveDistance, 0f);

        // raycasst quet enemy
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(direction, 0f), moveDistance, enemyLayer);

        if (hit.collider != null)
        {
            // check xem raycast co cham vao dung enemy hay cai khac
            if (hit.collider.TryGetComponent<EnemyAI>(out var enemy))
            {
                // gay dame
                enemy.TakeDamage(damage);
                Debug.Log("Bạn đã gây " + damage + " sát thương lên quái vật!");
                Destroy(gameObject);
                return;
            }
        }

        // dan di chuyen ve truoc 
        transform.position += (Vector3)moveVector;
    }

    // ve tia raycasst trong screen
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(direction * 0.3f, 0, 0));
    }
}
