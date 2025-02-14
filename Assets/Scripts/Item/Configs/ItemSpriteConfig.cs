using UnityEngine;
using System.Collections.Generic;

// Provides sprite mappings for each item type.
[CreateAssetMenu(menuName = "Game/Item Sprite Config", fileName = "ItemSpriteConfig")]
public class ItemSpriteConfig : ScriptableObject
{
    public List<ItemSpriteMapping> itemSpriteMappings = new();
    private Dictionary<ItemType, Sprite> spriteDictionary;
    private Dictionary<ItemType, Sprite> bonusSpriteDictionary;

    private void OnEnable()
    {
        BuildDictionary();
    }

    // Builds dictionaries from the mapping list.
    private void BuildDictionary()
    {
        spriteDictionary = new Dictionary<ItemType, Sprite>();
        bonusSpriteDictionary = new Dictionary<ItemType, Sprite>();

        foreach (var mapping in itemSpriteMappings)
        {
            if (!spriteDictionary.ContainsKey(mapping.itemType))
                spriteDictionary.Add(mapping.itemType, mapping.sprite);
            if (mapping.bonusSprite != null && !bonusSpriteDictionary.ContainsKey(mapping.itemType))
                bonusSpriteDictionary.Add(mapping.itemType, mapping.bonusSprite);
        }
    }

    public Sprite GetSpriteForItemType(ItemType itemType)
    {
        if (spriteDictionary == null || spriteDictionary.Count == 0)
            BuildDictionary();
        spriteDictionary.TryGetValue(itemType, out Sprite sprite);
        return sprite;
    }

    public Sprite GetBonusSpriteForItemType(ItemType itemType)
    {
        if (bonusSpriteDictionary == null || bonusSpriteDictionary.Count == 0)
            BuildDictionary();
        bonusSpriteDictionary.TryGetValue(itemType, out Sprite bonusSprite);
        return bonusSprite;
    }
}

// Maps an item type to its corresponding sprites.
[System.Serializable]
public class ItemSpriteMapping
{
    public ItemType itemType;
    public Sprite sprite;
    public Sprite bonusSprite;
}
