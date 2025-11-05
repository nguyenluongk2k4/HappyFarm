using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI shopNameText;        // Text_Title
    public Transform contentParent;             // Scroll View → Viewport → Content
    public GameObject productItemPrefab;        // Prefab ProductItem

    public void DisplayShopContent(List<ItemDataProduct> itemsToDisplay, string currentShopName)
    {
        if (shopNameText != null)
            shopNameText.text = currentShopName;

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (ItemDataProduct item in itemsToDisplay)
        {
            GameObject newItem = Instantiate(productItemPrefab, contentParent);

            var nameText = newItem.transform.Find("ProductName").GetComponent<TextMeshProUGUI>();
            var priceText = newItem.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            var image = newItem.transform.Find("ProductImage").GetComponent<Image>();

            if (nameText != null) nameText.text = item.itemName;
            if (priceText != null) priceText.text = item.price + " G";
            if (image != null) image.sprite = item.itemIcon;

            if (image != null)
            {
                Button imageButton = image.GetComponent<Button>();
                if (imageButton == null)
                    imageButton = image.gameObject.AddComponent<Button>();

                imageButton.onClick.RemoveAllListeners();
                imageButton.onClick.AddListener(() => OnProductImageClicked(item));
            }

        }
    }


    private void OnProductImageClicked(ItemDataProduct item)
    {
        Debug.Log($"💡 DEBUG: Coins hiện tại = {Player.instance.Coins}, giá = {item.price}");

        if (Player.instance.Coins < item.price)
        {
            Debug.LogWarning("❌ Không đủ tiền để mua vật phẩm này!");
        }
        else
        {
            // --- 2️⃣ Tìm ItemData tương ứng theo tên ---
            ItemData matchedItem = ItemDataList.Instance.GetItemByName(item.itemName);
            if (matchedItem == null)
            {
                Debug.LogError($"❌ Không tìm thấy vật phẩm '{item.itemName}' trong ItemDataList!");
                return;
            }

            // --- 3️⃣ Kiểm tra kho có thể thêm không ---
            if (!InventoryManager.Instance.CanAdd(matchedItem, 1))
            {
                Debug.LogWarning("⚠️ Kho đồ đã đầy, không thể thêm vật phẩm mới!");
                return;
            }

            // --- 4️⃣ Trừ tiền ---
            Player.instance.Coins -= item.price;
            Debug.Log($"💰 Đã trừ {item.price} G. Còn lại: {Player.instance.Coins} G");

            // --- 5️⃣ Thêm vật phẩm vào kho ---
            int added = InventoryManager.Instance.Add(matchedItem, 1);
            if (added > 0)
            {
                Debug.Log($"✅ Mua thành công: {matchedItem.itemName}");
            }
            else
            {
                Debug.LogWarning("⚠️ Không thể thêm vật phẩm vào kho (có lỗi bất ngờ)!");
            }
        }
    }

}

