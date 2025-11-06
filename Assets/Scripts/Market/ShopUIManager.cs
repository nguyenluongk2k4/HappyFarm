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

        // Xóa sản phẩm cũ trước khi load mới
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Hiển thị danh sách sản phẩm mới
        foreach (ItemDataProduct item in itemsToDisplay)
        {
            GameObject newItem = Instantiate(productItemPrefab, contentParent);

            // Tìm và gán các thành phần con
            var nameText = newItem.transform.Find("ProductName").GetComponent<TextMeshProUGUI>();
            var priceText = newItem.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            var image = newItem.transform.Find("ProductImage").GetComponent<Image>();

            if (nameText != null) nameText.text = item.itemData.itemName;
            if (priceText != null) priceText.text = item.price + " G";
            if (image != null) image.sprite = item.itemData.icon;

            // --- Gán sự kiện click vào ảnh ---
            if (image != null)
            {
                // Đảm bảo ảnh có Button hoặc EventTrigger để nhận click
                Button imageButton = image.GetComponent<Button>();
                if (imageButton == null)
                    imageButton = image.gameObject.AddComponent<Button>();

                imageButton.onClick.RemoveAllListeners();
                imageButton.onClick.AddListener(() => OnProductImageClicked(item));
            }

        }
    }


    // --- Khi người chơi click vào ảnh sản phẩm ---
    private void OnProductImageClicked(ItemDataProduct item)
    {
        Debug.Log($"🛒 Bạn đã chọn mua: {item.itemData.itemName} ({item.price} G)");

        // 👉 Thêm logic mua hàng tại đây:
        // - Kiểm tra đủ tiền
        // - Trừ tiền
        // - Thêm item vào kho (inventory)
        // - Cập nhật UI hoặc hiệu ứng mua
    }
}

