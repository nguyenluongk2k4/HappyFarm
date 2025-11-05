using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    public string itemName;
    public int quantity = 1;

    public void Interact(PlayerInteraction interactor)
    {
        ItemData item = ItemDataList.Instance.GetItemByName(itemName);
        if (item == null)
        {
            Debug.LogWarning($"Không tìm thấy item '{itemName}' trong ItemDataList!");
            return;
        }

        if (!InventoryManager.Instance.CanAdd(item, quantity))
        {
            Debug.LogWarning("Kho đầy, không thể nhặt " + item.itemName);
            return;
        }

        InventoryManager.Instance.Add(item, quantity);
        Debug.Log($"Nhặt được {quantity} {item.itemName}");

        Destroy(gameObject);
    }
}
