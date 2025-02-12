using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Item Sprite Config", fileName = "ItemSpriteConfig")]
public class ItemSpriteConfig : ScriptableObject
{
    public List<ItemSpriteMapping> itemSpriteMappings = new();

    private Dictionary<ItemType, Sprite> spriteDictionary;

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        spriteDictionary = new Dictionary<ItemType, Sprite>();
        foreach (var mapping in itemSpriteMappings)
        {
            if (!spriteDictionary.ContainsKey(mapping.itemType))
            {
                spriteDictionary.Add(mapping.itemType, mapping.sprite);
            }
        }
    }

    public Sprite GetSpriteForItemType(ItemType itemType)
    {
        if (spriteDictionary == null || spriteDictionary.Count == 0)
        {
            BuildDictionary();
        }
        spriteDictionary.TryGetValue(itemType, out Sprite sprite);
        return sprite;
    }
}

[System.Serializable]
public class ItemSpriteMapping
{
    public ItemType itemType;
    public Sprite sprite;
}
