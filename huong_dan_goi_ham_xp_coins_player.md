# 🎮 Hướng dẫn sử dụng Player.cs (XP & Coins System)

## 🧩 Tổng quan
Script `Player.cs` quản lý toàn bộ hệ thống **XP**, **Level**, và **Coins** cho nhân vật.
Nó được thiết kế theo mô hình **Singleton**, đảm bảo chỉ có một Player tồn tại xuyên suốt các Scene.

---

## ⚙️ Các hàm chính

### 🎯 XP & Level

#### ➕ `AddXP(int amount)`
Tăng kinh nghiệm cho người chơi. Nếu đủ XP, tự động lên cấp.

```csharp
Player.instance.AddXP(50);
```

#### 🧮 `GetXPRequiredForNextLevel()`
Trả về lượng XP cần để lên cấp tiếp theo.

```csharp
int xpToNext = Player.instance.GetXPRequiredForNextLevel();
```

#### 🏆 `GetLevel()`
Lấy cấp độ hiện tại của người chơi.

```csharp
int currentLevel = Player.instance.GetLevel();
```

---

### 💰 Coins

#### ➕ `AddCoins(int amount)`
Cộng thêm số coin cho người chơi.

```csharp
Player.instance.AddCoins(100);
```

#### 💸 `SpendCoins(int amount)`
Trừ coin khi mua vật phẩm. Trả về `true` nếu đủ tiền, `false` nếu không đủ.

```csharp
if (Player.instance.SpendCoins(50))
{
    Debug.Log("Đã mua vật phẩm!");
}
else
{
    Debug.Log("Không đủ tiền!");
}
```

#### 🪙 `GetCoins()`
Trả về số lượng coin hiện tại.

```csharp
int currentCoins = Player.instance.GetCoins();
```

---

## 🧠 Sự kiện (UnityEvents)

Các event có thể được gắn vào UI hoặc các hệ thống khác để cập nhật realtime khi giá trị thay đổi:

| Event | Kiểu dữ liệu | Kích hoạt khi | Ví dụ sử dụng |
|--------|---------------|----------------|----------------|
| `OnXPChanged` | `int` | XP thay đổi | Cập nhật thanh XP |
| `OnLevelChanged` | `int` | Người chơi lên cấp | Hiển thị popup "Level Up!" |
| `OnCoinChanged` | `int` | Coins thay đổi | Cập nhật text số tiền trên UI |

Ví dụ đăng ký sự kiện:
```csharp
Player.instance.OnCoinChanged.AddListener(UpdateCoinUI);
```

---

## 💡 Gợi ý sử dụng trong các hệ thống khác

### 🌾 Trong `LandPlot.cs` khi thu hoạch cây trồng
```csharp
Player.instance.AddXP(currentCrop.xpGained);
Player.instance.AddCoins(5); // thưởng thêm coin
```

### 🛒 Trong Shop khi mua vật phẩm
```csharp
if (Player.instance.SpendCoins(product.price))
{
    InventoryManager.Instance.Add(product.baseItem, 1);
}
```

### 🎨 Trong UI HUD
```csharp
coinText.text = Player.instance.GetCoins().ToString();
xpBar.value = Player.instance.GetXP() / Player.instance.GetXPRequiredForNextLevel();
levelText.text = "Lv " + Player.instance.GetLevel();
```

---

## 🧱 Hệ thống lưu dữ liệu (Gợi ý mở rộng)
Bạn có thể lưu Coins và XP khi thoát game bằng PlayerPrefs:

```csharp
void OnApplicationQuit()
{
    PlayerPrefs.SetInt("coins", Player.instance.GetCoins());
    PlayerPrefs.SetInt("xp", Player.instance.GetXP());
}

void Start()
{
    Player.instance.AddCoins(PlayerPrefs.GetInt("coins", 0));
    Player.instance.AddXP(PlayerPrefs.GetInt("xp", 0));
}
```

---

## 🧩 Gợi ý mở rộng
- Thêm **XP Bar** trên UI (dùng `Slider`).
- Thêm **hiệu ứng Level Up** bằng `Animator` hoặc `Particle System`.
- Tạo **Popup thông báo coin/xp** khi thu hoạch hoặc mua bán.

---

© 2025 - Player System Documentation (Generated with Assistant AI)
