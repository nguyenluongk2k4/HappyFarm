using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance { get; private set; }
    [System.Serializable] public class SlotChangedEvent : UnityEvent<int> { }

    public int size = 9;
    public List<ItemStack> slots = new List<ItemStack>();
    public UnityEvent OnChanged = new UnityEvent();
    public SlotChangedEvent OnSlotChanged = new SlotChangedEvent();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);
        while (slots.Count < size) slots.Add(new ItemStack(null, 0));
    }

    public void Set(int index, ItemStack v)
    {
        if (index < 0 || index >= slots.Count) return;
        slots[index] = v;
        OnSlotChanged.Invoke(index);
        OnChanged.Invoke();
    }

    public void Clear(int index)
    {
        if (index < 0 || index >= slots.Count) return;
        slots[index].Clear();
        OnSlotChanged.Invoke(index);
        OnChanged.Invoke();
    }

    public bool IsEmpty(int i) => i >= 0 && i < slots.Count && slots[i].IsEmpty;
}
