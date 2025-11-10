using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance { get; private set; }

    [Header("Slots")]
    public int size = 9;
    public List<ItemStack> slots = new List<ItemStack>();

    [Header("Events")]
    public UnityEvent<int> OnSlotChanged = new UnityEvent<int>();
    public UnityEvent OnChanged = new UnityEvent();
    public UnityEvent<int> OnSelectedChanged = new UnityEvent<int>(); // 🆕 thêm event này

    [Header("Defaults")]
    public ItemData handItem;
    private int selectedIndex = 0;
    public int SelectedIndex => selectedIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        while (slots.Count < size)
            slots.Add(new ItemStack());
        if (handItem != null)
        {
            slots[0] = new ItemStack(handItem, 1);
        }
        else
        {
            Debug.LogWarning("⚠️ HotbarManager: chưa gán handItem trong Inspector!");
        }
        SelectSlot(0);
    }

    private void Update()
    {
        HandleHotbarInput();
    }

    private void HandleHotbarInput()
    {
        for (int i = 0; i < size; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= size) return;
        if (selectedIndex == index) return;

        selectedIndex = index;
        Debug.Log($"🎯 Hotbar chọn ô {index + 1}");
        OnSelectedChanged.Invoke(selectedIndex);
    }


    public ItemStack GetSelectedStack() => slots[selectedIndex];
    public void Set(int index, ItemStack stack)
    {
        if (index < 0 || index >= slots.Count) return;

        slots[index] = stack;
        OnSlotChanged.Invoke(index);
        OnChanged.Invoke();
    }

    public bool ConsumeSelected(int amount)
    {
        if (amount <= 0) return false;

        var stack = slots[selectedIndex];
        if (stack.IsEmpty || stack.quantity < amount) return false;

        stack.quantity -= amount;
        if (stack.quantity <= 0)
        {
            stack.Clear();
        }

        slots[selectedIndex] = stack;
        OnSlotChanged.Invoke(selectedIndex);
        OnChanged.Invoke();
        return true;
    }

    public void SetSlot(int index, ItemData item, int amount)
    {
        if (index < 0 || index >= slots.Count) return;

        slots[index] = new ItemStack(item, amount);
        OnSlotChanged.Invoke(index);
        OnChanged.Invoke();
    }

    public void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i] = new ItemStack(null, 0);
            OnSlotChanged.Invoke(i); // ✅ update UI từng ô
        }
        OnSelectedChanged.Invoke(selectedIndex);
        OnChanged.Invoke();
        Debug.Log("🧽 Hotbar đã được reset!");
    }



}
[System.Serializable]
public class HotbarSaveData
{
    public List<HotbarItemSave> hotbarItems = new();

    [System.Serializable]
    public class HotbarItemSave
    {
        public string itemName;
        public int amount;
        public int slotIndex;
    }

    public void Save()
    {
        hotbarItems.Clear();
        for (int i = 0; i < HotbarManager.Instance.slots.Count; i++)
        {
            var slot = HotbarManager.Instance.slots[i];
            if (!slot.IsEmpty)
            {
                hotbarItems.Add(new HotbarItemSave
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
        HotbarManager.Instance.Clear();
        foreach (var data in hotbarItems)
        {
            var item = ItemDataList.Instance.GetItemByName(data.itemName);
            if (item != null)
                HotbarManager.Instance.SetSlot(data.slotIndex, item, data.amount);
        }
    }
}


