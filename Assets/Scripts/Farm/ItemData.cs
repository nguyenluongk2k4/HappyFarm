using UnityEngine;

public enum ItemType
{
    None,
    Tool_Hoe,
    Tool_WateringCan,
    Seed,
    Crop,
    Material,
    Food,
    Tool_FarmLand,
}

[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStackSize = 99;

    public ItemType type; 
}
