// InventoryUI.cs
using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform gridParent;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    void Start()
    {
        // tạo đủ ô
        for (int i = 0; i < InventoryManager.Instance.maxSlots; i++)
        {
            var go = Instantiate(slotPrefab, gridParent);
            var ui = go.GetComponent<InventorySlotUI>();
            ui.slotIndex = i;
            slotUIs.Add(ui);
        }
        // cập nhật toàn bộ 1 lần
        for (int i = 0; i < slotUIs.Count; i++)
            slotUIs[i].Show(InventoryManager.Instance.slots[i]);
    }
}
