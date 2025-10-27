# HappyFarm - Game Nông Trại Unity C#

## Cấu trúc thư mục

- **Scripts/**: Chứa các script C#
  - Managers/: Quản lý game (GameManager, FarmManager, etc.)
  - Farm/: Logic nông trại (đất, cây trồng, vật nuôi)
  - UI/: Giao diện người dùng
  - Data/: Lưu trữ dữ liệu (save/load, inventory)

- **Prefabs/**: Các prefab tái sử dụng
  - FarmObjects/: Đối tượng nông trại
  - UI/: Các thành phần UI

- **Sprites/**: Hình ảnh 2D
  - Farm/: Sprite cho nông trại
  - UI/: Sprite cho giao diện

- **Audio/**: Âm thanh
  - SFX/: Hiệu ứng âm thanh
  - BGM/: Nhạc nền

- **Resources/**: Tài nguyên động
- **Animations/**: Animation clips
- **Materials/**: Materials cho rendering

## Cách sử dụng

1. Thêm GameManager vào scene chính
2. Tạo FarmTile objects cho các ô đất
3. Phát triển logic game theo từng thư mục