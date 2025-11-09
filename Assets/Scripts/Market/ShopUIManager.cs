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
        // Gán tên cửa hàng
        if (shopNameText != null)
            shopNameText.text = currentShopName;

        // Xóa sản phẩm cũ
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Hiển thị sản phẩm mới
        foreach (ItemDataProduct item in itemsToDisplay)
        {
            GameObject newItem = Instantiate(productItemPrefab, contentParent);

            var nameText = newItem.transform.Find("ProductName")?.GetComponent<TextMeshProUGUI>();
            var priceText = newItem.transform.Find("Price")?.GetComponent<TextMeshProUGUI>();
            var image = newItem.transform.Find("ProductImage")?.GetComponent<Image>();

            if (nameText != null) nameText.text = item.itemData.itemName;
            if (priceText != null) priceText.text = item.price + " G";
            if (image != null) image.sprite = item.itemData.icon;

            // --- Gán sự kiện click vào ảnh ---
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
        try
        {
            if (item == null)
            {
                Debug.LogWarning("⚠️ Sản phẩm bị null!");                                            
                return;
            }

            if (item.itemData == null)
            {
                Debug.LogWarning($"⚠️ {item.name} chưa gán ItemData!");
                return;
            }
                                                                        
            // 💸 Trừ tiền
            if (!Player.instance.SpendCoins(item.price))
            {
                Debug.LogWarning("❌ Giao dịch thất bại vì bạn không đủ tiền. Hãy kiếm thêm trước khi quay lại!");
                return;
            }

            InventoryManager.Instance.Add(item.itemData, 1);

            Debug.Log($"✅ Đã mua {item.itemData.itemName} với giá {item.price} G");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"⚠️ Lỗi khi mua sản phẩm: {ex.Message}");
        }
    }



}
