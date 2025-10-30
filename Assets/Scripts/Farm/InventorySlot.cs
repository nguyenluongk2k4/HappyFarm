// 30/10/2025
// Một class đơn giản để lưu trữ dữ liệu của một ô trong kho

[System.Serializable] // Dòng này giúp nó hiển thị trong Inspector
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    // Kiểm tra xem ô này đã đầy 99/99 chưa
    public bool IsStackFull()
    {
        return quantity >= item.maxStackSize;
    }
}