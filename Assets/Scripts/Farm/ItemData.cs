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
    Animal,
    Rod,
    Fish
}

[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStackSize = 99;
    public int sellPrice = 0;
    public ItemType type;

    [Header("World Placement")]
    public GameObject worldPrefab; // optional prefab spawned when using the item in world
}
