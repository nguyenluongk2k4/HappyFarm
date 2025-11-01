// 30/10/2025
// ScriptableObject để định nghĩa dữ liệu cho một vật phẩm (Item)

using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Farming/Item")]
public class ItemData : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon;
    public int maxStackSize = 99;
}