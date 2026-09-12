# Prefab và Instantiate — Khái niệm, tác dụng và ứng dụng trong Unity

## Prefab và Instantiate là gì?

Đây là **cặp khái niệm đi cùng nhau**, giải quyết một vấn đề cơ bản: *làm sao tạo ra nhiều bản sao của một object khi game đang chạy?*

```
Prefab      = "khuôn mẫu" (bản thiết kế) — lưu trong Project, không nằm trong Scene
Instantiate = "đúc" (tạo bản sao từ khuôn) — chạy khi game đang hoạt động
```

---

## 1. Prefab — khuôn mẫu tái sử dụng

### Khái niệm kỹ thuật

Prefab là file lưu **toàn bộ cấu trúc** của một GameObject (và các con của nó): Transform, SpriteRenderer, Collider2D, Rigidbody2D, script, giá trị đã chỉnh... — đóng gói thành **1 asset có thể tái sử dụng vô hạn lần**.

Nói cách khác: Prefab áp dụng nguyên lý **Prototype Pattern** (mẫu thiết kế tạo object từ bản gốc).

### Tạo Prefab

Kéo GameObject từ Hierarchy thả vào cửa sổ **Project** — xong. Trong Hierarchy, object chuyển sang màu xanh (instance của prefab).

### Tác dụng

| Lợi ích | Giải thích |
|---|---|
| **Tái sử dụng** | Tạo 1 lần, dùng ở mọi scene, mọi nơi |
| **Đồng bộ** | Sửa prefab gốc → **tất cả bản sao tự cập nhật** (override giữ nguyên) |
| **Spawn động** | Chỉ có prefab mới `Instantiate` được khi game chạy |
| **Quản lý dễ** | Một chỗ sửa, mọi chỗ đổi — khỏi sửa từng object lẻ |

**Ví dụ thực tế:** bạn làm quái slime — gồm Sprite + Collider + Script AI. Nếu không có prefab, tạo 100 con slime = copy tay 100 lần. Có prefab: 1 khuôn + 1 vòng lặp.

---

## 2. Instantiate — sinh object khi game chạy

### Khái niệm kỹ thuật

`Instantiate()` là hàm **tạo object mới từ prefab (hoặc object có sẵn) tại runtime** — thời điểm game đang chạy, không phải lúc thiết kế.

### Cú pháp và các dạng dùng

```csharp
public GameObject bulletPrefab; // kéo prefab vào Inspector

void Shoot()
{
    // Dạng 1: Tạo tại vị trí + góc xoay chỉ định
    Instantiate(bulletPrefab, transform.position, Quaternion.identity);

    // Dạng 2: Tạo làm CON của object khác (gọn Hierarchy)
    Instantiate(bulletPrefab, transform.position, 
                Quaternion.identity, transform);

    // Dạng 3: Tạo ra biến để điều khiển ngay lập tức
    GameObject bullet = Instantiate(bulletPrefab, transform.position, 
                                     Quaternion.identity);
    bullet.GetComponent<Rigidbody2D>().velocity = Vector2.right * 10f;
}
```

### Tác dụng

- **Spawn** quái, đạn, hiệu ứng, item rơi ra
- **Clone** object đang có sẵn trong scene
- Tạo object **theo điều kiện/timing** (mỗi 2 giây spawn 1 quái, giết boss thì rớt đồ)

---

## 3. Ứng dụng thực tế trong game 2D

### ① Hệ thống bắn đạn

```csharp
public GameObject bulletPrefab;
public Transform firePoint;

void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
}
```

### ② Spawner quái theo thời gian

```csharp
public GameObject enemyPrefab;
float timer;

void Update()
{
    timer += Time.deltaTime;
    if (timer >= 3f)              // mỗi 3 giây
    {
        timer = 0;
        Vector3 pos = new Vector3(Random.Range(-5f, 5f), 5f, 0);
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}
```

### ③ Hiệu ứng nổ / rớt đồ khi chết

```csharp
public GameObject explosionPrefab;
public GameObject lootPrefab;

void Die()
{
    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    Instantiate(lootPrefab, transform.position, Quaternion.identity);
    Destroy(gameObject);
}
```

### ④ Object Pooling — nâng cao (nên biết)

`Instantiate` + `Destroy` liên tục gây **giật lag** (phân mảnh bộ nhớ). Giải pháp: tạo sẵn một "hồ" object, dùng xong **ẩn đi tái sử dụng** thay vì xóa:

```csharp
// Thay vì Instantiate mỗi lần bắn:
// → tạo sẵn 50 viên đạn lúc Start(), lấy ra dùng, hết thì reset vị trí
```

Đây là kỹ thuật bắt buộc với game bắn súng, bullet-hell có hàng trăm object cùng lúc.

---

## 4. Prefab Variants — biến thể của prefab

Unity hỗ trợ tạo **prefab con kế thừa prefab cha**:

```
Enemy (prefab gốc: máu 100, tốc độ 2)
 ├── EnemyFast   (variant: tốc độ 5, màu đỏ)
 └── EnemyTank   (variant: máu 500, to gấp đôi)
```

Sửa prefab gốc → cả 3 cùng cập nhật, còn khác biệt riêng giữ nguyên. Rất mạnh khi làm nhiều loại quái/boss từ chung nền tảng.

---

## 5. Ngược lại: Destroy

```csharp
Destroy(gameObject);              // Xóa sau ngay lập tức (cuối frame)
Destroy(gameObject, 2f);          // Xóa sau 2 giây — hay dùng cho hiệu ứng nổ
Destroy(bullet, 5f);              // Đạn tự hủy nếu không trúng ai
```

---

## Tóm lại

| | Prefab | Instantiate |
|---|---|---|
| Là gì | Khuôn mẫu asset trong Project | Hàm đúc bản sao từ khuôn |
| Khi nào | Lúc thiết kế (editor) | Lúc game chạy (runtime) |
| Giải quyết | Khỏi làm lại object giống nhau | Tạo object động theo gameplay |
| Ứng dụng | Quái, đạn, item, UI, hiệu ứng | Spawn, bắn, nổ, rớt đồ |

**Công thức nhớ:** *Prefab = bản thiết kế → Instantiate = khởi công → Destroy = tháo dỡ.* Ba thứ này là nền tảng của mọi hệ thống sinh object động trong Unity.
