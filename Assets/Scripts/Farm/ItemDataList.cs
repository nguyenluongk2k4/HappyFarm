using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDataList : MonoBehaviour
{
    public List<ItemData> itemDatas = new List<ItemData>();
    public static ItemDataList Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public ItemData GetItemByName(string name)
    {
        foreach (ItemData item in itemDatas)
        {
            if (item.itemName == name)
            {
                return item;
            }
        }
        return null;
    }
}
