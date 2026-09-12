using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("=== KHUÔN MẪU QUÁI (PREFAB) ===")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("=== THÔNG SỐ CHU KỲ SINH QUÁI ===")]
    [Tooltip("Chu kỳ n giây sinh quái một lần")]
    [SerializeField] private float spawnInterval = 3f;

    [Tooltip("Số lượng m con sinh ra trong mỗi đợt")]
    [SerializeField] private int spawnCountPerBatch = 1;

    [Tooltip("Số quái tối đa cùng xuất hiện trên Scene (đủ sẽ dừng sinh)")]
    [SerializeField] private int maxEnemies = 5;

    [Header("=== VÙNG SINH QUÁI (SPAWN AREA) ===")]
    [SerializeField] private Vector2 spawnAreaCenter = new Vector2(5f, -3.5f);
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(3f, 1.5f);

    private float timer = 0f;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Update()
    {
        // 1. Dọn dẹp danh sách các quái đã bị tiêu diệt (bị Destroy)
        activeEnemies.RemoveAll(enemy => enemy == null);

        // 2. Nếu đã đủ số lượng tối đa thì không sinh thêm quái nữa
        if (activeEnemies.Count >= maxEnemies)
        {
            return;
        }

        // 3. Đếm thời gian theo chu kỳ n giây
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBatch();
        }
    }

    private void SpawnBatch()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Chưa gán Enemy Prefab vào EnemySpawner!");
            return;
        }

        for (int i = 0; i < spawnCountPerBatch; i++)
        {
            // Kiểm tra lại số lượng để không vượt quá maxEnemies
            if (activeEnemies.Count >= maxEnemies) break;

            // Tính toán vị trí ngẫu nhiên trong vùng sinh quái
            float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
            float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2f, spawnAreaCenter.y + spawnAreaSize.y / 2f);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            // Nhân bản quái từ Prefab (Instantiate theo bài học)
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(newEnemy);

            Debug.Log("Spawner đã sinh ra: " + newEnemy.name + " (Hiện có: " + activeEnemies.Count + "/" + maxEnemies + ")");
        }
    }

    // Vẽ vùng sinh quái màu vàng trên Scene View
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));
    }
}
