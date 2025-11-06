using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI storeTitle;
    public Transform contentPanel;
    public GameObject productItemPrefab;
    public List<ItemDataProduct> items;

    void Start()
    {
        storeTitle.text = "Fruit Adventure Market";
        PopulateShop();
    }

    void PopulateShop()
    {
        foreach (var itemData in items)
        {
            GameObject item = Instantiate(productItemPrefab, contentPanel);

            item.transform.Find("ProductName").GetComponent<Text>().text = itemData.itemData.itemName;
            item.transform.Find("ProductImage").GetComponent<Image>().sprite = itemData.itemData.icon;
            item.transform.Find("Price").GetComponent<Text>().text = itemData.price + " coins";

        }
    }


}
