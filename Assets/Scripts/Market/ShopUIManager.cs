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
        Debug.Log($"🛒 Bạn đã chọn mua: {item.itemName} ({item.price} G)");
        if (Player.instance.Coins < item.price)
        {
            Debug.LogWarning("❌ Không đủ tiền để mua vật phẩm này!");
            return;
        }

        bool hasSpace = InventoryManager.Instance.CheckForSpace(item, 1);
        if (!hasSpace)
        {
            Debug.LogWarning("⚠️ Kho đồ đã đầy, không thể thêm vật phẩm mới!");
            return;
        }
        // --- 3️⃣ Trừ tiền ---
        Player.instance.Coins -= item.price;
        Debug.Log($"💰 Đã trừ {item.price} G. Số tiền còn lại: {Player.instance.Coins} G");

        // --- 4️⃣ Thêm vật phẩm vào kho ---
        InventoryManager.Instance.AddItem(item, 1);

        // --- 5️⃣ Cập nhật UI hoặc hiệu ứng mua hàng ---
        Debug.Log($"✅ Mua thành công: {item.itemName}");
    }
}

