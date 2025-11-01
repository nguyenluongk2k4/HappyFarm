using UnityEngine;
using System.Collections.Generic;

public class ShopTrigger : MonoBehaviour
{
    public ShopData shopData;

    // Cần gán ShopPanel UI từ Hierarchy
    public GameObject shopPanelUI;

    private ShopUIManager shopManager;

    void Start()
    {
        if (shopPanelUI != null)
        {
            shopPanelUI.SetActive(false);

            shopManager = shopPanelUI.GetComponent<ShopUIManager>();
            if (shopManager == null)
            {
                Debug.LogError("Lỗi: ShopUIManager không tìm thấy trên ShopPanelUI. Vui lòng kiểm tra việc gán script.");
            }
        }
    }

    // Xử lý khi nhân vật BƯỚC VÀO vùng Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã vào cửa hàng: " + shopData.shopName);


            if (shopPanelUI != null)
            {
                shopPanelUI.SetActive(true);
            }

       
            if (shopManager != null && shopData != null)
            {
                shopManager.DisplayShopContent(shopData.itemsForSale, shopData.shopName);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã rời cửa hàng: " + shopData.shopName);

            // Ẩn UI Panel
            if (shopPanelUI != null)
            {
                shopPanelUI.SetActive(false);
            }
        }
    }

   
}