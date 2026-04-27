using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDicitonary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictitonary;
    private void Awake()
    {
        itemDictitonary = new Dictionary<int, GameObject>();
        for(int i = 0; i < itemPrefabs.Count; i++)
        {
            if(itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1;
            }
        }
        foreach(Item item in itemPrefabs)
        {
            itemDictitonary[item.ID] = item.gameObject;
        }
    }
    public GameObject GetItemPrefab(int itemID)
    {
        itemDictitonary.TryGetValue(itemID, out GameObject prefab);
        if(prefab == null)
        {
            Debug.LogWarning("Item with ID" +itemID+ "not found in dictionary");
        }
        return prefab;
    }
}
