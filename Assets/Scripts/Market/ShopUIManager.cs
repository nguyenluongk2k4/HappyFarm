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

    public void DisplayShopContent(List<ItemData> itemsToDisplay, string currentShopName)
    {
        // Gán tên cửa hàng
        if (shopNameText != null)
            shopNameText.text = currentShopName;

        // Xóa sản phẩm cũ trước khi load mới
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Hiển thị danh sách sản phẩm mới
        foreach (ItemData item in itemsToDisplay)
        {
            GameObject newItem = Instantiate(productItemPrefab, contentParent);

            // Tìm và gán các thành phần con
            var nameText = newItem.transform.Find("ProductName").GetComponent<TextMeshProUGUI>();
            var priceText = newItem.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            var image = newItem.transform.Find("ProductImage").GetComponent<Image>();

            if (nameText != null) nameText.text = item.itemName;
            if (priceText != null) priceText.text = item.price + " G";
            if (image != null) image.sprite = item.itemIcon;
        }
    }
}
