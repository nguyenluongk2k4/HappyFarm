using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public int quantity;
    }

    [System.Serializable]
    public class LandTile
    {
        public int x, y;
        public string cropName;
        public float growProgress;
        public bool isPlanted;
    }

    // Dữ liệu chính
    public int gold = 1000;
    public int level = 1;
    public int exp = 0;
    public string currentMap = "Farm";
    public Vector3 playerPosition;

    public List<InventoryItem> inventory = new List<InventoryItem>();
    public List<LandTile> plantedTiles = new List<LandTile>();

    // Khởi tạo game mới
    public void InitializeNewGame()
    {
        gold = 1000; level = 1; exp = 0;
        currentMap = "Farm"; playerPosition = Vector3.zero;
        inventory.Clear(); plantedTiles.Clear();

        inventory.Add(new InventoryItem { itemName = "CarrotSeed", quantity = 10 });
        inventory.Add(new InventoryItem { itemName = "WateringCan", quantity = 1 });
    }
}