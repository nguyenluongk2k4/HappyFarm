using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Shop", menuName = "Shop Data/Shop List")]
public class ShopData : ScriptableObject
{
    public string shopName;

    public List<ItemData> itemsForSale;
}