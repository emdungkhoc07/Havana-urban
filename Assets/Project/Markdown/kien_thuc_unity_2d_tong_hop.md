# Kiến thức nền tảng Unity 2D: Transform, Collider2D, Rigidbody2D, Raycast2D, LayerMask, Trigger

> Tài liệu tổng hợp các khái niệm cốt lõi của hệ thống vật lý và không gian trong Unity 2D, cùng mối quan hệ giữa chúng.

---

## 1. Transform — "xương sống" không gian của mọi object

### Transform là gì?

`Transform` là **component bắt buộc có mặt trên mọi GameObject** — không thể xóa. Nó quản lý **vị trí (position), góc xoay (rotation) và tỉ lệ (scale)** của object trong không gian game.

```
Transform
├── Position: (x, y, z)
├── Rotation: (x, y, z)
└── Scale:    (x, y, z)
```

### Những điểm quan trọng

**1. Transform tạo ra hệ thống cha-con (hierarchy)**
- Mỗi Transform có thể chứa các Transform con (`transform.parent`, `transform.GetChild()`)
- Con **kế thừa** vị trí, xoay, scale của cha — ví dụ: súng gắn vào tay nhân vật sẽ di chuyển theo tay

**2. Tọa độ có 2 dạng**
- `position` / `rotation` — tọa độ **thế giới** (world space)
- `localPosition` / `localRotation` / `localScale` — tọa độ **so với cha** (local space)

### Sử dụng phổ biến

| Mục đích | Code thường gặp |
|---|---|
| Di chuyển object | `transform.position += Vector3.forward * speed * Time.deltaTime;` |
| Di chuyển theo hướng riêng | `transform.Translate(Vector3.forward * speed * Time.deltaTime);` |
| Xoay object | `transform.Rotate(Vector3.up * 90 * Time.deltaTime);` |
| Xoay nhìn về mục tiêu | `transform.LookAt(target.position);` |
| Tìm object con | `transform.Find("TênCon")`, `transform.GetChild(0)` |
| Hướng phía trước | `transform.forward`, `transform.up`, `transform.right` |

---

## 2. Collider2D — "hình dạng" va chạm

### Collider2D là gì?

`Collider2D` xác định **vùng va chạm** của object trong mặt phẳng 2D. Các dạng phổ biến: `BoxCollider2D`, `CircleCollider2D`, `CapsuleCollider2D`, `PolygonCollider2D`.

### Đặc điểm then chốt

- Collider **không phải "vật cản"** — nó chỉ là thông tin hình học để hệ vật lý (Physics2D) phát hiện chồng lấp
- Không có Collider → hệ vật lý **không biết object tồn tại** → raycast xuyên qua, không có va chạm
- **Không Collider → chắc chắn đâm xuyên tường** khi di chuyển bằng Transform

---

## 3. Rigidbody2D — "linh hồn vật lý"

### Rigidbody2D là gì?

Component biến GameObject từ "hình ảnh tĩnh" thành **"vật thể vật lý"** — có khối lượng, vận tốc, chịu trọng lực và lực tác động.

### Có Rigidbody vs không có

| Không Rigidbody | Có Rigidbody |
|---|---|
| Đứng yên, không rơi, không bị đẩy | Rơi xuống do trọng lực (`Gravity Scale`) |
| Collider chỉ là vùng cảm biến | Va chạm tự phản ứng (nảy, dừng lại) |
| Tự code mọi thứ | Vật lý có sẵn: trọng lực, ma sát, nảy, xoay |

### Thuộc tính quan trọng

- **Mass:** vật nặng hơn ít bị đẩy hơn khi va chạm
- **Drag / Angular Drag:** lực cản chuyển động / xoay
- **Collision Detection:** `Discrete` (nhanh) vs `Continuous` (chính xác cho vật nhanh — đạn)
- **Freeze Rotation:** chặn xoay khi va chạm (tránh nhân vật 2D bị lật nhào)
- **bodyType:** Dynamic / Kinematic / Static

### Di chuyển ĐÚNG cách với Rigidbody

```csharp
Rigidbody2D rb;

void Start() => rb = GetComponent<Rigidbody2D>();

void FixedUpdate()
{
    // Cách 1: đặt vận tốc (phổ biến nhất cho nhân vật 2D)
    rb.velocity = new Vector2(speed, rb.velocity.y);

    // Cách 2: di chuyển mượt, tôn trọng va chạm
    rb.MovePosition(rb.position + move * Time.fixedDeltaTime);

    // Cách 3: nhảy bằng lực đẩy
    if (Input.GetKeyDown(KeyCode.Space))
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
}
```

⚠️ **Lỗi kinh điển:** thêm Collider rồi vẫn xuyên tường — nguyên nhân gần như luôn là đang gán `transform.position` thay vì dùng Rigidbody, và code vật lý phải đặt trong `FixedUpdate()` (50 lần/giây, đồng bộ với physics).

---

## 4. Raycast2D — tia dò va chạm thủ công

### Raycast2D là gì?

`Raycast2D` là một **tia ảo** bắn từ một điểm theo một hướng — để **dò xem trên đường đi có Collider2D nào không**. Kết quả trả về là `RaycastHit2D` chứa: object trúng (`hit.collider`), điểm chạm (`hit.point`), pháp tuyến (`hit.normal`), khoảng cách (`hit.distance`).

### Nó dò được cái gì?

**Chỉ dò được Collider2D** — object không có Collider thì tia xuyên qua như không khí.

### Dùng làm điều kiện — cách dùng phổ biến nhất

#### ① Ground check — kiểm tra đứng trên đất

```csharp
bool isGrounded;

void Update()
{
    // Tia chiếu xuống chạm ground → được phép nhảy
    isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);

    if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        Jump();
}
```

#### ② Phát hiện tường — di chuyển không cần Rigidbody

```csharp
RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.2f);

if (hit.collider != null)
{
    Debug.Log("Đụng tường: " + hit.collider.name);
    // Không đi tiếp
}
```

#### ③ Tầm nhìn kẻ địch

```csharp
RaycastHit2D hit = Physics2D.Linecast(enemyPos, playerPos);

if (hit.collider.CompareTag("Player"))
    Chase();
else
    Idle(); // Có vật cản giữa đường → không thấy người chơi
```

### Các biến thể

| Hàm | Tác dụng |
|---|---|
| `Physics2D.Raycast()` | 1 tia, trả về cú trúng **đầu tiên** |
| `Physics2D.RaycastAll()` | Trả về **tất cả** collider trên đường tia |
| `Physics2D.Linecast()` | Tia nối 2 điểm cụ thể |
| `Physics2D.BoxCast()` | Bắn hình hộp thay vì tia mỏng |
| `Physics2D.CircleCast()` | Bắn hình tròn — quét khu vực có bán kính |

### Lưu ý thực tế

**1. Tia bắt đầu bên trong chính mình sẽ tự trúng mình** — phải loại trừ bằng LayerMask:

```csharp
RaycastHit2D hit = Physics2D.Raycast(
    transform.position, Vector2.down, 0.5f,
    ~LayerMask.GetMask("Player")  // Bỏ qua layer Player (chính mình)
);
```

**2. Vẽ tia để debug** — raycast vô hình:

```csharp
Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.red);
```

**3. Độ dài tia quan trọng** — quá ngắn không phát hiện được, quá dài thì "đứng gần mép vực vẫn tưởng có đất".

---

## 5. LayerMask — nhãn phân loại & bộ lọc tương tác

### Layer là gì?

Mỗi GameObject được gán một **Layer** (góc trên Inspector): `Default`, `Player`, `Enemy`, `Ground`, `UI`... LayerMask là "tờ phiếu chọn": *"tôi chỉ tương tác với những ai mang nhãn này"*.

### Dùng để làm gì?

**① Lọc va chạm vật lý** — `Edit → Project Settings → Physics 2D → Layer Collision Matrix`: tích ô = 2 layer va chạm được, bỏ tích = xuyên qua nhau.

**② Lọc Raycast**

```csharp
// Chỉ dò Ground
RaycastHit2D hit = Physics2D.Raycast(
    pos, Vector2.down, 1f,
    LayerMask.GetMask("Ground")
);
```

**③ Lọc hàm va chạm trong code**

```csharp
void OnCollisionEnter2D(Collision2D col)
{
    if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        isGrounded = true;
}
```

**④ Camera** — `Culling Mask`: camera chỉ render layer nào (UI, minimap, hậu cảnh).

> `LayerMask.GetMask("Ground")` lấy mask theo **tên layer**; `1 << layerIndex` là cách viết bitmask thủ công. Hai cách cùng kết quả.

---

## 6. Trigger — collider "vô hình" không chặn vật lý

### Trigger là gì?

Tick ô **Is Trigger** trong Collider2D → collider biến từ "bức tường" thành **"vùng cảm ứng"**: object đi **xuyên qua** được, nhưng vẫn phát sinh sự kiện.

```
Collider thường:  Đạn bị chặn lại, nảy ra, dừng lại
Collider Trigger: Đạn bay xuyên qua, nhưng kích hoạt sự kiện
```

### Dùng để làm gì?

Trigger sinh ra cho mục đích: **phát hiện "ai đó vừa đi vào vùng này"**.

| Trường hợp | Cách dùng |
|---|---|
| Đạn trúng địch | Đạn bay xuyên qua (trigger), gây sát thương rồi biến mất |
| Vùng nhặt item | Player đi vào trigger → cộng máu/coin, xóa item |
| Checkpoint, vùng chết | Trigger ở vực → chết/hồi sinh |
| Cửa tự động mở | Trigger trước cửa → phát animation mở cửa |
| Khu vực kích hoạt event, thoại, cutscene | |

### Code nhận sự kiện trigger

```csharp
void OnTriggerEnter2D(Collider2D other)   // Vừa bước vào
{
    if (other.CompareTag("Player"))
        Debug.Log("Player đi vào!");
}

void OnTriggerStay2D(Collider2D other)    // Đang đứng trong vùng (mỗi frame)
{
    // Ví dụ: đứng trong lửa thì mất máu dần
}

void OnTriggerExit2D(Collider2D other)    // Vừa đi ra
{
    Debug.Log("Player đi ra!");
}
```

⚠️ Hàm trigger chỉ chạy khi **ít nhất 1 trong 2 object có Rigidbody2D**.

---

## 7. Mối quan hệ giữa các khái niệm — bức tranh tổng thể

### Sơ đồ vai trò

```
┌─────────────────────────────────────────────────────┐
│                     TRANSFORM                        │
│  Vị trí / xoay / scale — tồn tại trên MỌI object    │
│  → chỉ là con số toán học, KHÔNG có va chạm         │
└──────────────┬──────────────────────────────────────┘
               │ gắn lên
               ▼
┌─────────────────────────────────────────────────────┐
│                    COLLIDER2D                        │
│  "Hình dạng" va chạm — chỉ là thông tin, chưa sống  │
└──────────────┬──────────────────────────────────────┘
               │ + Rigidbody2D (ít nhất 1 bên)
               ▼
┌─────────────────────────────────────────────────────┐
│                   RIGIDBODY2D                        │
│  "Linh hồn vật lý" — trọng lực, lực, va chạm tự động│
└──────────────┬──────────────────────────────────────┘
               │ phát sinh sự kiện
               ▼
        OnCollisionEnter2D / OnTriggerEnter2D

┌─────────────────────────────────────────────────────┐
│                    RAYCAST2D                         │
│  Tia dò Collider2D — kiểm tra va chạm THỦ CÔNG      │
│  (thay thế khi không muốn dùng Rigidbody)            │
└──────────────┬──────────────────────────────────────┘
               │ được lọc bởi
               ▼
┌─────────────────────────────────────────────────────┐
│                    LAYERMASK                         │
│  Chọn "ai được tương tác với ai" trong cả 4 thứ:    │
│  physics matrix, raycast, hàm va chạm, camera        │
└──────────────────────────────────────────────────────┘
```

### Bảng tổng hợp

| Khái niệm | Vai trò | Bắt buộc? |
|---|---|---|
| **Transform** | Vị trí/xoay/scale, hệ cha-con | Có — mọi GameObject đều có, không xóa được |
| **Collider2D** | Hình dạng va chạm | Cần khi muốn va chạm/raycast phát hiện object |
| **Rigidbody2D** | Mô phỏng vật lý (rơi, lực, phản ứng va chạm) | Cần ít nhất 1 bên để va chạm hoạt động |
| **Raycast2D** | Dò Collider2D bằng tia, dùng làm điều kiện | Không — thay thế kiểm tra va chạm thủ công |
| **LayerMask** | Nhãn + bộ lọc tương tác | Không — nhưng nên dùng để tránh va chạm rác |
| **Trigger** | Collider mềm: xuyên qua được, chỉ phát sự kiện | Không — dùng cho vùng cảm ứng |

### Các mối quan hệ cụ thể

**Transform ↔ Collider2D:** Collider phụ thuộc Transform — dịch chuyển Transform thì vùng va chạm di chuyển theo. Nhưng Transform **không quan tâm** Collider có tồn tại hay không → gán `transform.position` = teleport, xuyên tường dù có Collider.

**Collider2D ↔ Rigidbody2D:** đây là "thân xác" và "linh hồn". Công thức va chạm chuẩn:

```
Collider2D (bạn) + Collider2D (tường) + Rigidbody2D (ít nhất 1 bên)
        ↓
Hệ vật lý 2D mới phát hiện và phản ứng va chạm
```

- Collider thường → chặn vật lý (`OnCollisionEnter2D`)
- Collider Is Trigger → xuyên qua, chỉ phát sự kiện (`OnTriggerEnter2D`)
- Di chuyển bằng Transform trong `Update()` → vẫn xuyên tường dù đủ bộ trên

**Raycast2D ↔ Collider2D:** raycast **chỉ dò được Collider2D** — không Collider thì tia bay xuyên. Raycast là cách kiểm tra va chạm thủ công (ground check, tường, tầm nhìn) khi không dùng Rigidbody.

**LayerMask ↔ tất cả:** LayerMask lọc ở mọi ngả:
- Collision Matrix: 2 layer có va chạm được không
- Raycast: tia chỉ trúng layer nào
- Hàm va chạm: kiểm tra layer của object kia
- Camera: render layer nào

Nói cách khác: **Collider/Rigidbody/Raycast quyết định "có tương tác hay không", LayerMask quyết định "tương tác với ai"**.

### Mẫu kết hợp kinh điển — hệ thống đạn 2D

```
Đạn:  Collider2D (Is Trigger ✓) + Rigidbody2D + Layer "Bullet"
Địch: Collider2D (thường)       + Layer "Enemy"
Tường: Collider2D (thường)      + Layer "Ground"
```

- **Collision Matrix**: Bullet va Enemy ✓, Bullet va Ground ✓ (đạn dính tường), Bullet **không** va Player
- Đạn là trigger → **bay xuyên qua địch**, không bị nảy
- Khi xuyên qua → `OnTriggerEnter2D` → trừ máu

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Enemy"))
    {
        other.GetComponent<Enemy>().TakeDamage(10);
        Destroy(gameObject);
    }
}
```

### Hai cách tiếp cận va chạm

| | Vật lý tự động (Rigidbody) | Kiểm tra thủ công (Raycast) |
|---|---|---|
| Cần | Collider (2 bên) + Rigidbody | Chỉ cần Collider ở vật bị dò |
| Di chuyển | `rb.velocity`, `rb.MovePosition` | `transform.position` (kiểm tra trước khi đi) |
| Phù hợp | Nhân vật, vật rơi, vật bị đẩy | Ground check, tường, AI nhìn thấy gì |
| Chi phí | Nặng hơn | Nhẹ, kiểm soát tuyệt đối |

---

## Tóm tắt một câu

> **Transform** đặt object ở đâu → **Collider2D** cho nó hình dạng → **Rigidbody2D** cho nó sống trong hệ vật lý → **Trigger** biến va chạm thành vùng cảm ứng → **Raycast2D** dò va chạm thủ công bằng tia → còn **LayerMask** thì điều phối tất cả: chọn ai được va chạm, được dò, được nhìn thấy với ai.
