using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Shop Data/Item Data")]
public class ItemDataProduct : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int price;
    public ItemCategory category; 


    public enum ItemCategory
    {
        Food,
        Fruit,
        Seed,
        Tool,
        Livestock 
    }
}