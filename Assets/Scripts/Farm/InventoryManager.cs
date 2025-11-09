using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [System.Serializable] public class SlotChangedEvent : UnityEvent<int> { }

    public UnityEvent OnChanged = new UnityEvent();        // tổng
    public SlotChangedEvent OnSlotChanged = new SlotChangedEvent(); // theo ô

    [Header("Settings")] public int maxSlots = 20;
    [Header("Data")] public List<ItemStack> slots = new List<ItemStack>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);
        // Khởi tạo đủ ô rỗng
        while (slots.Count < maxSlots) slots.Add(new ItemStack(null, 0));
    }

    public bool CanAdd(ItemData item, int amount)
    {
        // đơn giản: còn ô trống hoặc có stack cùng loại chưa full
        int left = amount;
        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i];
            if (s.item == item && s.quantity < item.maxStackSize)
            {
                int space = item.maxStackSize - s.quantity;
                left -= Mathf.Min(space, left);
                if (left <= 0) return true;
            }
        }
        // tìm ô trống
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                int stacks = Mathf.CeilToInt((float)left / item.maxStackSize);
                // có đủ số ô trống?
                int empties = 0;
                for (int j = 0; j < slots.Count; j++) if (slots[j].IsEmpty) empties++;
                return empties >= stacks;
            }
        }
        return left <= 0;
    }

    public int Add(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return 0;
        int left = amount;

        // 1) fill các stack hiện có
        for (int i = 0; i < slots.Count; i++)
        {
            if (left <= 0) break;
            var s = slots[i];
            if (s.item == item && s.quantity < item.maxStackSize)
            {
                int space = item.maxStackSize - s.quantity;
                int add = Mathf.Min(space, left);
                s.quantity += add; left -= add;
                slots[i] = s;
                OnSlotChanged.Invoke(i);
            }
        }
        // 2) tạo stack mới vào ô trống
        for (int i = 0; i < slots.Count; i++)
        {
            if (left <= 0) break;
            if (slots[i].IsEmpty)
            {
                int add = Mathf.Min(item.maxStackSize, left);
                slots[i] = new ItemStack(item, add);
                left -= add;
                OnSlotChanged.Invoke(i);
            }
        }
        OnChanged.Invoke();
        return amount - left; // số đã thêm
    }

    public int Remove(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return 0;
        int left = amount;

        for (int i = 0; i < slots.Count; i++)
        {
            if (left <= 0) break;
            var s = slots[i];
            if (s.item == item && s.quantity > 0)
            {
                int take = Mathf.Min(s.quantity, left);
                s.quantity -= take; left -= take;
                if (s.quantity <= 0) s.Clear();
                slots[i] = s;
                OnSlotChanged.Invoke(i);
            }
        }
        OnChanged.Invoke();
        return amount - left; // số đã lấy
    }

    public void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i] = new ItemStack(null, 0);
            OnSlotChanged.Invoke(i);
        }
        OnChanged.Invoke();
        Debug.Log("🗑 Inventory đã được reset!");
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<ItemSave> items = new();

        [System.Serializable]
        public class ItemSave
        {
            public string itemName;
            public int amount;
            public int slotIndex;
        }

        public void Save()
        {
            items.Clear();
            for (int i = 0; i < InventoryManager.Instance.slots.Count; i++)
            {
                var slot = InventoryManager.Instance.slots[i];
                if (!slot.IsEmpty)
                {
                    items.Add(new ItemSave
                    {
                        itemName = slot.item.itemName,
                        amount = slot.quantity,
                        slotIndex = i
                    });
                }
            }
        }

        public void Load()
        {
            InventoryManager.Instance.Clear();
            foreach (var i in items)
            {
                var item = ItemDataList.Instance.GetItemByName(i.itemName);
                if (item != null)
                    InventoryManager.Instance.slots[i.slotIndex] = new ItemStack(item, i.amount);

                InventoryManager.Instance.OnSlotChanged.Invoke(i.slotIndex);
            }
            InventoryManager.Instance.OnChanged.Invoke();
        }
    }

}
