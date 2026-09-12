# Sự kiện (Event) & Coroutine trong Unity

> Tài liệu tổng hợp hai công cụ quan trọng giúp các script trong Unity giao tiếp với nhau và xử lý thời gian: **Event** (Action / Delegate / UnityEvent) và **Coroutine**.

---

# PHẦN I — SỰ KIỆN (EVENT) TRONG C# & UNITY

## 1. Sự kiện là gì? Tại sao cần?

**Sự kiện (Event)** là cơ chế cho phép một object **phát tín hiệu** khi có chuyện gì đó xảy ra, và các object khác **đăng ký lắng nghe** để phản ứng — mà hai bên **không cần biết nhau trực tiếp**.

### Vấn đề nếu không có Event

```csharp
// ❌ Cách "nối dây" trực tiếp — script phải biết nhau, dễ rối
void Update()
{
    if (player.health <= 0)
    {
        ui.ShowGameOver();        // UI phụ thuộc Player
        audio.PlayDeathSound();   // Audio phụ thuộc Player
        enemy.StopAll();          // Enemy phụ thuộc Player
    }
}
```

Player phải "nhớ" hết mọi thứ cần phản ứng. Thêm một hệ thống mới = sửa lại Player.

### Có Event

```csharp
// ✅ Player chỉ phát tín hiệu — ai quan tâm thì tự đăng ký
public class Player : MonoBehaviour
{
    public UnityEvent onDeath;   // phát tín hiệu

    void Die()
    {
        onDeath.Invoke();        // "Tôi chết rồi đây!" — không quan tâm ai nghe
    }
}
```

UI, Audio, Enemy tự đăng ký vào `onDeath`. Thêm hệ thống mới = chỉ đăng ký thêm, không sửa Player.

> **Ý nghĩa:** Event giúp code **lỏng ghép (decoupled)** — dễ mở rộng, dễ bảo trì.

---

## 2. Ba cách làm Event trong Unity

| | Delegate / Event (C# thuần) | Action (C# thuần) | UnityEvent (Unity) |
|---|---|---|---|
| Namespace | `System` | `System` | `UnityEngine.Events` |
| Hiện trong Inspector | ❌ | ❌ | ✅ |
| Gán từ Editor | ❌ | ❌ | ✅ (kéo thả) |
| Cần designer/level designer chỉnh | Không hợp | Không hợp | Rất hợp |
| Độ linh hoạt code | Cao | Cao nhất | Trung bình |

---

## 3. Delegate — tìm hiểu sơ qua

### Delegate là gì?

`Delegate` là **kiểu dữ liệu lưu tham chiếu tới hàm** — giống "hộp đựng hàm". Một delegate có thể chứa nhiều hàm cùng chữ ký, và gọi một lần thì tất cả hàm bên trong chạy theo.

```csharp
// 1. Khai báo delegate: giống "khuôn" của hàm
public delegate void OnHealthChanged(int currentHealth);

// 2. Tạo biến delegate
public OnHealthChanged healthChanged;

// 3. Đăng ký hàm vào delegate
void Start()
{
    healthChanged += UpdateHealthBar;   // gắn hàm
    healthChanged += PlayHitEffect;     // gắn thêm hàm nữa
}

// 4. Phát sự kiện — cả 2 hàm chạy
void TakeDamage(int dmg)
{
    health -= dmg;
    healthChanged?.Invoke(health);   // ?.Invoke = gọi nếu có ai đăng ký
}
```

> **`?.Invoke()`** — toán tử `?.` giúp tránh lỗi null nếu chưa ai đăng ký.

### Từ khóa `event` — bảo vệ delegate

```csharp
public event OnHealthChanged healthChanged;  // chỉ += / -= được bên ngoài

// bên ngoài KHÔNG thể làm: healthChanged = UpdateHealthBar;  (gán đè)
// chỉ được: healthChanged += UpdateHealthBar;
```

`event` ngăn việc gán đè mất danh sách đăng ký — nên luôn dùng.

---

## 4. Action — tìm hiểu sơ qua

### Action là gì?

`Action` là **delegate có sẵn của C#**, không cần khai báo kiểu riêng. Chỉ cần chỉ định kiểu tham số:

```csharp
// Tương đương: delegate void X(int);  — nhưng không cần tự khai báo
public event Action<int> onHealthChanged;   // hàm nhận 1 int
public event Action onDeath;                // hàm không tham số
public event Action<int, int> onHealthChangedFull; // (current, max)
```

### Đăng ký & phát sự kiện

```csharp
public class Player : MonoBehaviour
{
    public event Action<int> onHealthChanged;
    public event Action onDeath;

    int health = 100;

    void TakeDamage(int dmg)
    {
        health -= dmg;
        onHealthChanged?.Invoke(health);   // phát — truyền máu hiện tại

        if (health <= 0)
            onDeath?.Invoke();             // phát sự kiện chết
    }
}

public class HealthBarUI : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<Player>().onHealthChanged += UpdateBar;
    }

    void UpdateBar(int currentHealth)
    {
        slider.value = currentHealth;
    }

    // HỦY đăng ký khi object bị hủy — RẤT QUAN TRỌNG
    void OnDestroy()
    {
        FindObjectOfType<Player>().onHealthChanged -= UpdateBar;
    }
}
```

> **Quy tắc vàng: đăng ký ở đâu thì hủy ở đó** (`+=` đi đôi với `-=`). Quên hủy đăng ký là nguyên nhân số 1 gây **NullReferenceException** và rò rỉ bộ nhớ.

---

## 5. UnityEvent — TẬP TRUNG CHÍNH

### UnityEvent là gì?

`UnityEvent` là event **tích hợp của Unity**, hoạt động y hệt Action nhưng có **giao diện trong Inspector** — designer có thể gán hàm bằng cách **kéo thả**, không cần đụng code.

```csharp
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityEvent<int> onHealthChanged;  // event có tham số int
    public UnityEvent onDeath;               // event không tham số
}
```

Trong Inspector sẽ hiện:

```
On Death ()
├── +  (thêm listener)
│     └── Object: [AudioManager]  Function: AudioManager → PlayDeathSound()
└── +  (thêm listener)
      └── Object: [GameOverUI]    Function: GameOverUI → Show()
```

### Đăng ký UnityEvent — 2 cách

**① Từ Editor (Inspector)** — không cần code:
1. Chọn GameObject có script chứa `UnityEvent`
2. Bấm dấu `+` dưới danh sách event
3. Kéo GameObject chứa hàm cần gọi vào ô Object
4. Chọn hàm từ dropdown

**② Từ Code** — giống Action:

```csharp
public class GameManager : MonoBehaviour
{
    public Player player;

    void Start()
    {
        player.onDeath.AddListener(ShowGameOver);        // đăng ký
        player.onHealthChanged.AddListener(UpdateHealthUI);
    }

    void ShowGameOver()
    {
        Debug.Log("Game Over!");
    }

    void UpdateHealthUI(int health)
    {
        Debug.Log("Máu còn: " + health);
    }

    void OnDestroy()
    {
        player.onDeath.RemoveListener(ShowGameOver);     // hủy đăng ký
        player.onHealthChanged.RemoveListener(UpdateHealthUI);
    }
}
```

> Lưu ý: với UnityEvent, dùng `AddListener()` / `RemoveListener()` thay cho `+=` / `-=`.

### So sánh nhanh

| | `Action` / `event` | `UnityEvent` |
|---|---|---|
| Đăng ký trong code | `onDeath += ShowGameOver;` | `onDeath.AddListener(ShowGameOver);` |
| Hủy đăng ký | `onDeath -= ShowGameOver;` | `onDeath.RemoveListener(ShowGameOver);` |
| Đăng ký trong Editor | ❌ | ✅ kéo thả |
| Gọi (phát) | `onDeath?.Invoke();` | `onDeath.Invoke();` |

---

## 6. Gọi (phát) Event — Invoke()

`Invoke()` kích hoạt **tất cả** hàm đã đăng ký, chạy lần lượt:

```csharp
// Action / event
onHealthChanged?.Invoke(health);   // ?. = an toàn nếu null
onDeath?.Invoke();

// UnityEvent
onDeath.Invoke();
```

**Thứ tự thực thi:** các hàm chạy theo thứ tự đăng ký, đồng bộ (hàm sau chờ hàm trước chạy xong).

**Khi nào nên dùng `?.Invoke()`:**
- `Action`/`event`: **luôn dùng `?.`** — nếu chưa ai đăng ký, gọi thẳng sẽ NullReferenceException
- `UnityEvent`: `Invoke()` an toàn — UnityEvent tự xử lý null, không cần `?.`

---

## 7. Cẩm nang chọn công cụ

| Tình huống | Nên dùng |
|---|---|
| Giao tiếp nội bộ giữa các script, logic phức tạp | `Action` / `event` |
| Designer cần tự gán hàm (hiệu ứng, UI, level event) | `UnityEvent` |
| Button, Slider trong UGUI/UIToolkit | `UnityEvent` (tích hợp sẵn — `onClick`, `onValueChanged`) |
| Hệ thống game lớn (loot, quest, achievement) | `Action` / `event` |

> Ví dụ thực tế: `Button.onClick` trong Unity chính là `UnityEvent` — đó là lý do bạn có thể kéo thả hàm vào nút trong Inspector.

---

# PHẦN II — COROUTINE TRONG UNITY

## 1. Khái niệm luồng Coroutine

### Coroutine là gì?

**Coroutine** là một hàm **có thể tạm dừng (pause) và tiếp tục** ở chỗ cũ, thay vì chạy một mạch từ đầu đến cuối.

```csharp
// Hàm thường: chạy hết trong 1 frame
void Explode() { /* tất cả chạy ngay lập tức */ }

// Coroutine: có thể chờ 2 giây, rồi mới nổ
IEnumerator ExplodeAfterDelay()
{
    yield return new WaitForSeconds(2f);  // TẠM DỪNG 2 giây
    Explode();                            // rồi tiếp tục từ đây
}
```

### Tại sao cần Coroutine?

Unity không cho `Update()` "đợi" được. Muốn làm chuỗi hành động theo thời gian (đợi 2 giây → nổ → đợi 1 giây → xóa), các lựa chọn:

| Cách | Vấn đề |
|---|---|
| Đếm thời gian trong `Update()` bằng biến timer | Code rối, nhiều biến, khó đọc |
| `Thread.Sleep()` | ❌ **Đóng băng cả game** — Unity không an toàn thread |
| **Coroutine** | ✅ Dừng đúng chỗ, code gọn, vẫn chạy trên main thread |

### Bản chất luồng

- Coroutine **KHÔNG phải thread riêng** — nó vẫn chạy trên main thread của Unity
- Khi gặp `yield return`, Unity "cất" hàm lại, quay về vòng lặp game bình thường
- Khi điều kiện chờ thỏa mãn, Unity "mở" hàm ra và chạy tiếp từ dòng sau `yield`
- Vì chạy trên main thread → **an toàn** khi truy cập Transform, GameObject, UI

---

## 2. Cú pháp & cách sử dụng

### Hàm Coroutine phải trả về `IEnumerator`

```csharp
using System.Collections;

public class Enemy : MonoBehaviour
{
    IEnumerator PatrolRoutine()
    {
        // đoạn 1: chạy ngay
        MoveRight();

        // đoạn 2: chờ 2 giây
        yield return new WaitForSeconds(2f);

        // đoạn 3: chạy tiếp SAU KHI đã đợi xong
        MoveLeft();
    }
}
```

> **Phải có `using System.Collections;`** để dùng `IEnumerator`.

### Hai kiểu `yield return` cơ bản

**① `yield return null` — chờ 1 frame**

```csharp
IEnumerator SpawnNextFrame()
{
    yield return null;              // tạm dừng, sang frame sau mới chạy tiếp
    Debug.Log("Dòng này chạy ở frame KẾ TIẾP");
}
```

Ứng dụng: đợi physics tính xong, đợi object khác khởi tạo, tránh chạy code quá sớm trong cùng frame.

**② `yield return new WaitForSeconds(x)` — chờ x giây**

```csharp
IEnumerator HitEffect()
{
    sprite.color = Color.red;                        // đổi đỏ ngay
    yield return new WaitForSeconds(0.2f);           // chờ 0.2 giây
    sprite.color = Color.white;                      // trắng lại
}
```

Ứng dụng: hiệu ứng chớp đỏ khi trúng đòn, delay nổ, cooldown kỹ năng, đếm ngược.

### Các kiểu yield khác (tham khảo)

| Kiểu yield | Chờ đến khi |
|---|---|
| `yield return null` | Frame kế tiếp |
| `yield return new WaitForSeconds(t)` | Hết t giây |
| `yield return new WaitForFixedUpdate()` | Frame vật lý kế tiếp (FixedUpdate) |
| `yield return new WaitForEndOfFrame()` | Cuối frame hiện tại |
| `yield return new WaitUntil(() => dieuKien)` | Điều kiện đúng |
| `yield return new WaitWhile(() => dieuKien)` | Điều kiện sai |
| `yield return StartCoroutine(...)` | Coroutine khác chạy xong |

---

## 3. Bắt đầu và dừng Coroutine

### StartCoroutine — bắt đầu

```csharp
void Start()
{
    // Cách 1: gọi trực tiếp tên hàm
    StartCoroutine(PatrolRoutine());

    // Cách 2: truyền hàm kèm tham số
    StartCoroutine(ExplodeAfter(3f));
}

IEnumerator ExplodeAfter(float seconds)
{
    yield return new WaitForSeconds(seconds);
    Explode();
}
```

> Chỉ có thể gọi `StartCoroutine` từ một **MonoBehaviour**.

### StopCoroutine — dừng

```csharp
Coroutine patrol;   // lưu tham chiếu để dừng

void Start()
{
    patrol = StartCoroutine(PatrolRoutine());
}

void OnPlayerDetected()
{
    StopCoroutine(patrol);      // dừng đúng cái đang chạy
}
```

⚠️ **Lưu ý:** `StopCoroutine(PatrolRoutine())` (gọi tên hàm) thường **không hoạt động** với cách `StartCoroutine` kiểu 1 — phải **lưu tham chiếu `Coroutine`** khi start, rồi truyền nó vào stop.

### Các cách dừng khác

```csharp
StopAllCoroutines();      // dừng MỌI coroutine của script này
SetActive(false);         // deactive object → mọi coroutine dừng
Destroy(gameObject);      // hủy object → coroutine dừng
```

### Ví dụ hoàn chỉnh — bẫy tự reset

```csharp
using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] float activeTime = 1f;
    [SerializeField] float cooldownTime = 2f;

    Coroutine trapRoutine;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && trapRoutine == null)
            trapRoutine = StartCoroutine(TrapCycle());
    }

    IEnumerator TrapCycle()
    {
        ShowSpikes();                              // gai nhô lên
        yield return new WaitForSeconds(activeTime);   // giữ 1 giây

        HideSpikes();                              // gai thụt xuống
        yield return new WaitForSeconds(cooldownTime); // nghỉ 2 giây

        trapRoutine = null;                        // sẵn sàng cho lượt kế
    }
}
```

---

## 4. Sai lầm thường gặp

| Sai lầm | Hậu quả | Cách sửa |
|---|---|---|
| Gọi `PatrolRoutine()` trực tiếp mà không `StartCoroutine` | Hàm không chạy, warning | Luôn bọc trong `StartCoroutine()` |
| `StopCoroutine(PatrolRoutine())` | Không dừng được | Lưu `Coroutine` lúc start, stop bằng biến đó |
| Quên `yield` trong vòng lặp | Vòng lặp chạy hết trong 1 frame → treo game | `while` phải có `yield return` bên trong |
| Truy cập object đã bị Destroy từ coroutine | NullReferenceException | Kiểm tra `this == null` hoặc dừng coroutine trước |

### Vòng lặp trong Coroutine — lặp mỗi frame

```csharp
IEnumerator FadeOut()
{
    float alpha = 1f;
    while (alpha > 0f)
    {
        alpha -= Time.deltaTime;        // giảm dần
        sprite.color = new Color(1, 1, 1, alpha);
        yield return null;              // ⚠️ BẮT BUỘC — nhường frame, nếu không game treo
    }
}
```

---

## Tóm tắt

**Event:**
- `Delegate`/`Action` = event trong code C#, nhanh, linh hoạt → `+=` đăng ký, `-=` hủy, `?.Invoke()` phát
- `UnityEvent` = event có giao diện Inspector, designer kéo thả được → `AddListener()`/`RemoveListener()`, `.Invoke()` phát
- **Đăng ký ở đâu, hủy ở đó** — quên hủy là mồi của NullReferenceException

**Coroutine:**
- Hàm `IEnumerator`, tạm dừng bằng `yield return`
- `yield return null` = chờ 1 frame; `WaitForSeconds(x)` = chờ x giây
- `StartCoroutine()` để chạy, lưu tham chiếu `Coroutine` + `StopCoroutine()` để dừ
