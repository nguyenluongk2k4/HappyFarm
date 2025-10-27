using UnityEngine;

public class FarmTile : MonoBehaviour
{
    public enum TileState { Empty, Planted, Harvested }

    public TileState currentState = TileState.Empty;
    public GameObject plantedCrop;

    void Start()
    {
        // Initialize tile
    }

    public void PlantCrop(GameObject cropPrefab)
    {
        if (currentState == TileState.Empty)
        {
            plantedCrop = Instantiate(cropPrefab, transform.position, Quaternion.identity);
            currentState = TileState.Planted;
        }
    }

    public void Harvest()
    {
        if (currentState == TileState.Planted && plantedCrop != null)
        {
            Destroy(plantedCrop);
            currentState = TileState.Harvested;
            // Add to inventory logic here
        }
    }
}