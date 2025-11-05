using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : SlotUIBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        // lắng nghe thay đổi slot riêng
        InventoryManager.Instance.OnSlotChanged.AddListener(OnSlotChanged);
        // sync ban đầu
        OnSlotChanged(slotIndex);
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnSlotChanged.RemoveListener(OnSlotChanged);
    }

    void OnSlotChanged(int idx)
    {
        if (idx != slotIndex) return;
        Show(InventoryManager.Instance.slots[slotIndex]);
    }

    // Drag from INVENTORY
    public void OnBeginDrag(PointerEventData e)
    {
        var s = InventoryManager.Instance.slots[slotIndex];
        if (s.IsEmpty) return;
        bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        int amount = isShiftHeld ? s.quantity : 1;

        DragService.Begin(canvas, DragSource.Inventory, slotIndex, s.item, amount);
    }

    public void OnDrag(PointerEventData e) { DragService.Move(e.position); }

    public void OnEndDrag(PointerEventData e) { DragService.End(); }

    // Drop TO INVENTORY
    public void OnDrop(PointerEventData e)
    {
        if (DragService.Source == DragSource.None || DragService.Item == null) return;
        var target = InventoryManager.Instance.slots[slotIndex];

        // Nếu cùng nguồn & cùng ô → bỏ qua
        if (DragService.Source == DragSource.Inventory && DragService.SourceIndex == slotIndex) return;

        // Thao tác theo nguồn
        if (DragService.Source == DragSource.Inventory)
        {
            // INVENTORY → INVENTORY (stack/swap/move)
            ApplyStackSwapInventoryToInventory(DragService.SourceIndex, slotIndex, DragService.Item, DragService.Amount);
        }
        else if (DragService.Source == DragSource.Hotbar)
        {
            // HOTBAR → INVENTORY (trả hàng)
            int moved = InventoryManager.Instance.Add(DragService.Item, DragService.Amount);
            // trừ từ hotbar
            var hs = HotbarManager.Instance.slots[DragService.SourceIndex];
            hs.quantity -= moved;
            if (hs.quantity <= 0) hs.Clear();
            HotbarManager.Instance.Set(DragService.SourceIndex, hs);
        }
    }

    void ApplyStackSwapInventoryToInventory(int from, int to, ItemData item, int amount)
    {
        var slots = InventoryManager.Instance.slots;
        var a = slots[from];
        var b = slots[to];

        // Nếu ô đích trống → move
        if (b.IsEmpty)
        {
            int moved = Mathf.Min(amount, a.quantity);
            b = new ItemStack(item, moved);
            a.quantity -= moved;
            if (a.quantity <= 0) a.Clear();
        }
        // Nếu cùng item → stack
        else if (b.item == item)
        {
            int space = b.item.maxStackSize - b.quantity;
            int moved = Mathf.Min(space, amount);
            b.quantity += moved;
            a.quantity -= moved;
            if (a.quantity <= 0) a.Clear();
        }
        // Item khác → swap
        else
        {
            var tmp = b;
            b = new ItemStack(a.item, Mathf.Min(amount, a.quantity));
            a.quantity -= b.quantity;
            if (a.quantity <= 0) a = tmp; // phần còn lại ở a = item cũ của b
            else
            {
                // nếu vẫn còn phần dư ở a, đẩy lại item cũ của b về to (swap toàn stack hợp lý hơn)
                var keep = tmp;
                tmp = a;
                a = keep;
            }
        }

        slots[from] = a; slots[to] = b;
        InventoryManager.Instance.OnSlotChanged.Invoke(from);
        InventoryManager.Instance.OnSlotChanged.Invoke(to);
        InventoryManager.Instance.OnChanged.Invoke();
    }
}
