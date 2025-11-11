using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarSlotUI : SlotUIBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Canvas canvas;
    private Image background; // để highlight slot đang chọn
    private Color normalColor = new Color(1f, 1f, 1f, 0.6f);
    private Color selectedColor = new Color(1f, 1f, 0.3f, 0.9f); // màu vàng nhạt highlight

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        background = GetComponent<Image>();
        if (background == null)
        {
            background = gameObject.AddComponent<Image>();
            background.color = normalColor;
        }

        // Lắng nghe khi dữ liệu hotbar thay đổi
        HotbarManager.Instance.OnSlotChanged.AddListener(OnSlotChanged);
        HotbarManager.Instance.OnSelectedChanged.AddListener(OnSelectedChanged);

        // Cập nhật hiển thị ban đầu
        OnSlotChanged(slotIndex);
        UpdateHighlight();
    }

    void OnDestroy()
    {
        if (HotbarManager.Instance != null)
        {
            HotbarManager.Instance.OnSlotChanged.RemoveListener(OnSlotChanged);
            HotbarManager.Instance.OnSelectedChanged.RemoveListener(OnSelectedChanged);
        }
    }

    void OnSlotChanged(int idx)
    {
        if (idx != slotIndex) return;
        Show(HotbarManager.Instance.slots[slotIndex]);
    }

    void OnSelectedChanged(int selectedIdx)
    {
        UpdateHighlight();
    }

    void UpdateHighlight()
    {
        if (background == null) return;
        bool isSelected = (HotbarManager.Instance.SelectedIndex == slotIndex);
        background.color = isSelected ? selectedColor : normalColor;
    }

    // === DRAG & DROP ===
    public void OnBeginDrag(PointerEventData e)
    {
        var s = HotbarManager.Instance.slots[slotIndex];
        if (s.IsEmpty) return;
        bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        int amount = isShiftHeld ? s.quantity : 1;

        DragService.Begin(canvas, DragSource.Hotbar, slotIndex, s.item, amount);
    }

    public void OnDrag(PointerEventData e)
    {
        DragService.Move(e.position);
    }

    public void OnEndDrag(PointerEventData e)
    {
        DragService.End();
    }

    // Drop TO HOTBAR
    public void OnDrop(PointerEventData e)
    {
        if (DragService.Source == DragSource.None || DragService.Item == null)
            return;

        if (DragService.Source == DragSource.Hotbar)
        {
            // HOTBAR → HOTBAR
            if (DragService.SourceIndex == slotIndex) return;
            ApplyStackSwapHotbarToHotbar(DragService.SourceIndex, slotIndex, DragService.Item, DragService.Amount);
        }
        else if (DragService.Source == DragSource.Inventory)
        {
            // INVENTORY → HOTBAR
            int take = DragService.Amount;
            int actually = InventoryManager.Instance.Remove(DragService.Item, take);
            if (actually <= 0) return;

            var target = HotbarManager.Instance.slots[slotIndex];
            if (target.IsEmpty)
            {
                target = new ItemStack(DragService.Item, actually);
            }
            else if (target.item == DragService.Item)
            {
                target.quantity = Mathf.Min(target.quantity + actually, target.item.maxStackSize);
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

        slots[from] = a;
        slots[to] = b;

        HotbarManager.Instance.OnSlotChanged.Invoke(from);
        HotbarManager.Instance.OnSlotChanged.Invoke(to);
        HotbarManager.Instance.OnChanged.Invoke();
    }

    void Update()
    {
        if (HotbarManager.Instance.SelectedIndex == slotIndex)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                TrySellItem();
            }
        }
    }
    private void TrySellItem()
    {
        var stack = HotbarManager.Instance.slots[slotIndex];
        if (stack.IsEmpty)
        {
            Debug.Log("❌ Slot trống, không có gì để bán.");
            return;
        }

        // Kiểm tra loại item
        if (stack.item.type != ItemType.Material && stack.item.type != ItemType.Fish)
        {
            Debug.Log($"❌ Không thể bán {stack.item.itemName}. Loại item hiện tại là: {stack.item.type}");
            return;
        }

        int amount = stack.quantity;
        int gain = stack.item.sellPrice * amount;

        // Cộng coins cho player (tùy code game bạn)
        Player.instance.AddCoins(gain);

        // Xóa item khỏi hotbar
        HotbarManager.Instance.Set(slotIndex, new ItemStack(null, 0));

        Debug.Log($"✅ Đã bán {stack.item.itemName} x{amount} -> +{gain} coins!");
    }

}
