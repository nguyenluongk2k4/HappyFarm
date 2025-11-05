// HotbarUI.cs
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform parent;
    public int size = 9;

    void Start()
    {
        HotbarManager.Instance.size = size;
        for (int i = 0; i < size; i++)
        {
            var go = Instantiate(slotPrefab, parent);
            var ui = go.GetComponent<HotbarSlotUI>();
            ui.slotIndex = i;
            ui.Show(HotbarManager.Instance.slots[i]);
        }
    }
}
