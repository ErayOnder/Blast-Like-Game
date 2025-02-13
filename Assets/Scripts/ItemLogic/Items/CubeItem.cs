using UnityEngine;

public class CubeItem : Item
{
    private MatchType matchType;

    public MatchType MatchType => matchType;

    // Flag to indicate bonus status.
    public bool IsBonus { get; set; }

    public void InitializeConfig(ItemConfig config, MatchType matchType)
    {
        this.matchType = matchType;
        var spriteConfig = Resources.Load<ItemSpriteConfig>("ItemSpriteConfig");
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;
        base.InitializeFromProperties(config, sprite);
    }
    
    // Override the bonus effect execution for cube items.
    public override void ExecuteBonusEffect()
    {
        Debug.Log(gameObject.name + " executing bonus effect.");
        // Bonus effect logic can be added here (for example, clearing additional cells, special animations, etc.).
        // For now, we simply remove the item.
        TryExecute();
    }

    public void ResetSpriteToNormal()
    {
        var spriteConfig = Resources.Load<ItemSpriteConfig>("ItemSpriteConfig");
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
