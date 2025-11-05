using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    public string itemName; // Tên item trong ItemDataList
    public int quantity = 1;

    public void Interact()
    {
        ItemData item = ItemDataList.Instance.GetItemByName(itemName);
        if (item != null)
        {
            // Kiểm tra chỗ trống
            if (!InventoryManager.Instance.CheckForSpace(item, quantity))
            {
                Debug.LogWarning("Kho đầy, không thể nhặt " + item.itemName);
                return;
            }

            // Thêm vào kho
            InventoryManager.Instance.AddItem(item, quantity);
            Debug.Log("Nhặt được " + quantity + " " + item.itemName);

            // Xóa vật phẩm khỏi map
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy itemName: " + itemName + " trong ItemDataList!");
        }
    }
}