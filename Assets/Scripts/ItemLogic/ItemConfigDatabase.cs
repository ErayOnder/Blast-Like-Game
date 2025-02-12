using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Config Database", fileName = "ItemConfigDatabase")]
public class ItemConfigDatabase : ScriptableObject
{
    public List<ItemConfig> itemConfigs;

    public ItemConfig GetConfig(ItemType itemType)
    {
        return itemConfigs.Find(config => config.ItemType == itemType);
    }
    
}
