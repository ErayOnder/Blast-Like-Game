using UnityEngine;

public class CubeItem : Item
{
    private MatchType matchType;

    public MatchType MatchType => matchType;

    public bool IsBonus { get; set; }

    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig, MatchType matchType)
    {
        this.matchType = matchType;
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;
        base.InitializeFromProperties(config, sprite);
    }

    public void ResetSpriteToNormal(ItemSpriteConfig spriteConfig)
    {
        if (spriteConfig != null)
        {
            Sprite normalSprite = spriteConfig.GetSpriteForItemType(itemType);
            if (normalSprite != null)
            {
                // Reapply the default sprite settings.
                ApplySpriteRendererProperties(spriteRenderer, normalSprite);
            }
            else
            {
                Debug.LogWarning("No normal sprite found for item type " + itemType);
            }
        }
        else
        {
            Debug.LogWarning("ItemSpriteConfig not found.");
        }
    }
}
