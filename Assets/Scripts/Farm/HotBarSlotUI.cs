using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarSlotUI : SlotUIBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        HotbarManager.Instance.OnSlotChanged.AddListener(OnSlotChanged);
        OnSlotChanged(slotIndex);
    }

    void OnDestroy()
    {
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.OnSlotChanged.RemoveListener(OnSlotChanged);
    }

    void OnSlotChanged(int idx)
    {
        if (idx != slotIndex) return;
        Show(HotbarManager.Instance.slots[slotIndex]);
    }

    // Drag from HOTBAR
    public void OnBeginDrag(PointerEventData e)
    {
        var s = HotbarManager.Instance.slots[slotIndex];
        if (s.IsEmpty) return;
        bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        int amount = isShiftHeld ? s.quantity : 1;

        DragService.Begin(canvas, DragSource.Hotbar, slotIndex, s.item, amount);
    }

    public void OnDrag(PointerEventData e) { DragService.Move(e.position); }

    public void OnEndDrag(PointerEventData e) { DragService.End(); }

    // Drop TO HOTBAR
    public void OnDrop(PointerEventData e)
    {
        if (DragService.Source == DragSource.None || DragService.Item == null) return;

        if (DragService.Source == DragSource.Hotbar)
        {
            // HOTBAR → HOTBAR (stack/swap/move)
            if (DragService.SourceIndex == slotIndex) return;
            ApplyStackSwapHotbarToHotbar(DragService.SourceIndex, slotIndex, DragService.Item, DragService.Amount);
        }
        else if (DragService.Source == DragSource.Inventory)
        {
            // INVENTORY → HOTBAR
            int take = DragService.Amount;
            int actually = InventoryManager.Instance.Remove(DragService.Item, take); // lấy ra từ kho
            if (actually <= 0) return;

            var target = HotbarManager.Instance.slots[slotIndex];
            if (target.IsEmpty)
            {
                target = new ItemStack(DragService.Item, actually);
            }
            else if (target.item == DragService.Item)
            {
                target.quantity = Mathf.Min(target.quantity + actually, target.item.maxStackSize);
                // nếu muốn xử lý phần dư trả về inventory có thể thêm ở đây
            }
            else
            {
                // replace (trả item cũ về inventory)
                if (!target.IsEmpty)
                    InventoryManager.Instance.Add(target.item, target.quantity);
                target = new ItemStack(DragService.Item, actually);
            }
            HotbarManager.Instance.Set(slotIndex, target);
        }
    }

    void ApplyStackSwapHotbarToHotbar(int from, int to, ItemData item, int amount)
    {
        var slots = HotbarManager.Instance.slots;
        var a = slots[from];
        var b = slots[to];

        if (b.IsEmpty)
        {
            int moved = Mathf.Min(amount, a.quantity);
            b = new ItemStack(a.item, moved);
            a.quantity -= moved;
            if (a.quantity <= 0) a.Clear();
        }
        else if (b.item == a.item)
        {
            int space = b.item.maxStackSize - b.quantity;
            int moved = Mathf.Min(space, amount);
            b.quantity += moved;
            a.quantity -= moved;
            if (a.quantity <= 0) a.Clear();
        }
        else
        {
            // swap toàn stack
            var tmp = b;
            b = a;
            a = tmp;
        }

        slots[from] = a; slots[to] = b;
        HotbarManager.Instance.OnSlotChanged.Invoke(from);
        HotbarManager.Instance.OnSlotChanged.Invoke(to);
        HotbarManager.Instance.OnChanged.Invoke();
    }
}
