<details>
<summary>Unity buổi 3 (1/9/2026)</summary>
<details style="margin-left: 20px;">
<summary>Vòng đời (monobehaviour)</summary>

### Vòng đời monobehaviour
là thứ tự thực thi của các hàm được dựng sẵn (event funtions) từ lúc 1 scrips được tạo ra

nó giống như vòng đời của 1 con người : Sinh ra - Lớn lên - Đẻ con - Chết

#### khởi tạo
* `awake()` : Chạy 1 lần duy nhất ngay khi GameObject được tải  
  - chạy kể cả khi scrips tắt
  - Dùng để khởi tạo biến, kết nối component nội bộ  
- `OnEnable()`: Chạy mỗi khi GameObject hoặc Script chuyển từ trạng thái tắt sang bật.
  - Thường dùng để đăng ký Event/Action.
- `Reset()`: Chỉ chạy trong Unity Editor khi bạn vừa gắn script vào GameObject hoặc bấm nút Reset component.
- `Start()`: Chạy 1 lần duy nhất trước frame đầu tiên mà script được kích hoạt, luôn chạy sau Awake()

#### Vòng lặp  (Physics Loop)

* `FixedUpdate()`: Chạy theo một chu kỳ thời gian cố định (mặc định 0.02 giây, không phụ thuộc FPS của máy

#### Logic và Render (Game Logic & Frame Updates)
* `Update()`: Chạy một lần mỗi khung hình (Frame). Tốc độ gọi phụ thuộc vào FPS của máy. 
  - Dùng để bắt phím bấm (Input), tính toán thời gian, di chuyển logic thông thường.

- `LateUpdate()`: Chạy ngay sau khi tất cả các hàm Update() trên mọi object đã chạy xong. 
    - Thường dùng để điều khiển Camera follow (đảm bảo nhân vật đã đi xong rồi camera mới di chuyển theo để tránh rung lắc).

#### Tạm dừng và Hủy bỏ (Decommissioning & Destruction)
- `OnDisable()`: Chạy mỗi khi script hoặc GameObject bị tắt. 
  - Thường dùng để hủy đăng ký Event/Action để tránh rò rỉ bộ nhớ (Memory Leak).
- `OnDestroy()`: Chạy khi GameObject/Component bị xóa hoàn toàn khỏi Scene (`Destroy(gameObject)`) hoặc khi chuyển Scene. Dùng để dọn dẹp tài nguyên.
</details>
<details style="margin-left: 20px;">
<summary>Add assets</summary>

- kéo thả sprites/ assets từ explorer vào cửa sổ assets của unity
- Các nguồn lấy assets : unity assets store, tự vẽ :))
- nhìn lên cửa sổ **inspector**

    - *Texture Type* : Cài đặt này định nghĩa cách Unity sẽ xử lý hình ảnh được đưa vào

    - *Texture Shape* : Xác định hình dạng không gian của ảnh. Khi chọn Texture Type là Sprite, mục này mặc định bị khóa ở 2D

    - *Sprite Mode* : 
        Cài đặt quan trọng nhất để xác định số lượng hình ảnh có trong file file này.

        - *Single*: Dùng khi file ảnh chỉ chứa duy nhất một vật thể/nhân vật. Unity sẽ lấy toàn bộ ảnh làm một Sprite.

        - *Multiple*: Dùng khi file là một Spritesheet hoặc Tilemap chứa nhiều ảnh nhỏ ghép lại. Bạn phải dùng nút Open Sprite Editor bên dưới để cắt (Slice) tấm ảnh lớn này thành các mảnh nhỏ.

        - *Polygon*: Cho phép tự vẽ hình dạng lưới đa giác cho Sprite thay vì dùng hình chữ nhật mặc định.
    
    - *Pixels Per Unit (PPU)*:
Xác định bao nhiêu pixel trên file ảnh sẽ tương đương với 1 đơn vị khoảng cách (1 ô vuông lưới) trong thế giới của Unity.

       > Nếu bạn vẽ đồ họa Pixel Art với lưới 16x16, hãy nhập 16 .

         > Lưu ý quan trọng là toàn bộ Sprite trong cùng một game nên có chung một chỉ số PPU để tỷ lệ kích thước không bị sai lệch khi đặt cạnh nhau.

 - *Mesh Type* :
Cách Unity vẽ khung lưới (Mesh) bao quanh hình ảnh để render lên màn hình.

      - *Tight* : Unity tự động tạo một lưới bám sát viền của phần có hình ảnh, cắt bỏ tối đa các vùng trong suốt

      - *Full Rect* : Unity vẽ một hình chữ nhật bao trọn toàn bộ ảnh gốc, bao gồm cả vùng trong suốt. Dùng cho các Sprite vốn đã vuông

- *Extrude Edges* :
Tạo ra một viền lưới nhỏ mở rộng ra ngoài hình ảnh bao nhiêu pixel.

   >Thường đặt là 1. Việc này giúp ngăn chặn hiện tượng rách hình .hoặc xuất hiện các đường chỉ mờ giữa các viên gạch khi Camera trong game di chuyển hoặc phóng to/thu nhỏ.

- *Generate Physics Shape*:
Khi được tích, Unity sẽ tự động quét viền hình ảnh để tạo ra một khung va chạm nội suy.

    > Khi nào nên dùng: Tích chọn nếu vật thể này sẽ cần tương tác vật lý và bạn dự định sử dụng `PolygonCollider2D` hoặc `CompositeCollider2D`.

    >Khi nào không nên dùng: Bỏ tích nếu hình ảnh chỉ dùng để trang trí (như đám mây, mặt trời) hoặc nếu bạn dùng `BoxCollider2D/CircleCollider2D` cơ bản để tiết kiệm tài nguyên tính toán lúc Import.

- *Wrap Mode*:
Cách ảnh phản ứng khi được kéo giãn hoặc lặp lại vượt quá kích thước gốc.

    - *Clamp*: Kéo giãn màu của pixel cuối cùng ở mép ảnh. Bắt buộc dùng cho Tilemap và UI để các mảnh ghép không bị lem viền rác từ mảnh bên cạnh sang.

    - *Repeat* : Lặp lại toàn bộ ảnh từ đầu. Dùng cho các hình nền cuộn vô tận (Scrolling Background) hoặc vật liệu dán liên tục.

    - *Mirror*: Giống Repeat nhưng mỗi lần lặp lại ảnh sẽ bị lật ngược.

- *Filter Mode*:
Cách Unity nội suy màu sắc khi hình ảnh bị phóng to hoặc thu nhỏ trên màn hình.

  - *Point (no filter)* : Không làm mờ, giữ nguyên độ vuông vức sắc nét của từng điểm ảnh. Bắt buộc dùng cho đồ họa Pixel để hình không bị nhòe.

  - *Bilinear / Trilinear* : Làm mờ và trộn các điểm ảnh lân cận lại với nhau cho mịn màng. Dùng cho game đồ họa vẽ tay HD, vector, hoặc UI hiện đại để xóa tình trạng răng cưa.

- *Aniso Level*: 
Chỉ số tăng chất lượng bề mặt hình ảnh khi nhìn từ các góc Camera cực nghiêng. Trong game 2D và giao diện người dùng (nhìn thẳng góc 90 độ vuông góc với màn hình), cài đặt này không có tác dụng và nên để ở mức 1 hoặc vô hiệu hóa.
</details>
<details style="margin-left: 20px;">
<summary>Mathf</summary>

- *Mathf* : là một struct tĩnh được Unity cung cấp sẵn trong `namespace UnityEngine`, tập hợp các hàm và hằng số toán học thông dụng được tối ưu riêng cho kiểu dữ liệu số thực `float` trong lập trình game.

### Các hàm hay dùng:

#### Giới hạn

- `Mathf.Clamp(value, min, max)` : Giữ value luôn nằm trong khoảng $[min, max]$.

  >Ví dụ: Giữ thanh máu từ 0 đến 100: 
  `currentHp = Mathf.Clamp(currentHp, 0f, 100f);` 
- `Mathf.Clamp01(value)`: Tương tự Clamp nhưng giới hạn cố định trong đoạn $[0, 1]$ 

#### Nội suy và làm mượt

- `Mathf.Lerp(a, b, t)`: Tuyến tính chuyển từ $a$ sang $b$ dựa trên tỉ lệ $t$ (với $t \in [0, 1]$).

  >Ví dụ: Di chuyển camera mượt theo nhân vật:  
  `camX = Mathf.Lerp(camX, targetX, Time.deltaTime * speed);`

- `Mathf.MoveTowards(current, target, maxDelta)`: Dịch chuyển giá trị từ $current$ về phía $target$ với một bước nhảy tối đa cố định ***(không bị chậm dần như Lerp).***

#### Làm tròn số (Rounding)

- `Mathf.Floor(x)`: Làm tròn xuống số nguyên nhỏ hơn gần nhất.

- `Mathf.Ceil(x)`: Làm tròn lên số nguyên lớn hơn gần nhất.

- `Mathf.Round(x)`: Làm tròn tới số nguyên gần nhất (0.5).

>Lưu ý: Các hàm có hậu tố ToInt như `Mathf.FloorToInt()`, `Mathf.RoundToInt()` trả về thẳng kiểu int, không cần ép kiểu (int)

#### Lượng giác và Hằng số

- `Mathf.PI`: Số $\pi \approx 3.1415927$.
- `Mathf.Deg2Rad / Mathf.Rad2Deg`: Hệ số nhân để chuyển đổi giữa Độ (Degree) và Radian.
- `Mathf.Sin(rad), Mathf.Cos(rad)`: Lưu ý góc truyền vào bắt buộc là Radian.
  >Ví dụ: Tạo chuyển động bập bênh theo sóng:  `y = Mathf.Sin(Time.time * freq) * amplitude`;
#### Xử lý sai số số thực

- `Mathf.Approximately(a, b)`: So sánh xem 2 số thực có bằng nhau hay không, tránh lỗi sai số thập phân dấu phẩy động thay vì viết a == b.

</details>
<details style="margin-left: 20px;">
<summary>Gizmos lưới</summary>  

  

>Gizmos giúp vẽ ra các vùng diện tích (thường là phạm vi tầm đánh của vũ khí, chiêu thức hoặc tầm nhìn của quái ...), vùng này chỉ hiện đối với dev và không hiện trog game

```C#
using UnityEngine;

public class NewMonoBehaviourScript1 : MonoBehaviour
{
    [Header("Cài đặt lưới")]
    [SerializeField] int columns = 8;
    [SerializeField] int rows = 5;
    [SerializeField] float cellSize = 1f;
    // BẮT BUỘC phải dùng tên hàm OnDrawGizmos của Unity
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // 1. Đồng bộ Gizmos theo vị trí, góc xoay và scale của GameObject
        Gizmos.matrix = transform.localToWorldMatrix;

        float width = columns * cellSize;
        float height = rows * cellSize;

        Vector3 Tam_do_thi = new Vector3(-width / 2f, -height / 2f, 0f);
        // lùi dịch xuống và sang trái để vẽ tâm đồ thị
        // 2. Kẻ các đường dọc (Duyệt theo số cột)
        for (int i = 0; i <= columns; i++)
        {
            Vector3 bottom = Tam_do_thi + Vector3.right * (i * cellSize);
            Gizmos.DrawLine(bottom, bottom + Vector3.up * height);
        }

        // 3. Kẻ các đường ngang (Duyệt theo số hàng)
        for (int j = 0; j <= rows; j++)
        {
            Vector3 left = Tam_do_thi + Vector3.up * (j * cellSize);
            Gizmos.DrawLine(left, left + Vector3.right * width);
        }
    }
} 
```
</details>
<details style="margin-left: 20px;">
<summary>Tranform</summary>

- ***Transform :*** là một thành phần (Component) cốt lõi và quan trọng nhất trong Unity.

- ***Mọi đối tượng***: (GameObject) xuất hiện trong Scene đều bắt buộc phải có một Transform
- ***Transform*** quản lý 2 nhiệm vụ chính:  
 > xác định không gian của vật thể 
 
 > quản lý mối quan hệ cha - con
</details>