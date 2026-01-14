using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("所有物品数据")]
    public List<ItemData> allItems = new List<ItemData>();

    private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDatabase()
    {
        itemDictionary.Clear();

        foreach (ItemData item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.itemID))
            {
                if (!itemDictionary.ContainsKey(item.itemID))
                {
                    itemDictionary.Add(item.itemID, item);
                }
                else
                {
                    Debug.LogWarning("重复的物品ID: " + item.itemID);
                }
            }
        }

        Debug.Log("物品数据库初始化完成，共 " + itemDictionary.Count + " 个物品");
    }

    public ItemData GetItem(string itemID)
    {
        if (itemDictionary.ContainsKey(itemID))
        {
            return itemDictionary[itemID];
        }

        Debug.LogWarning("找不到物品: " + itemID);
        return null;
    }
}