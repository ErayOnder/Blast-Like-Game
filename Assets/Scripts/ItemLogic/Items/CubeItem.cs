using UnityEngine;

public class CubeItem : Item
{
    private MatchType matchType;

    public void InitializeConfig(ItemConfig config, MatchType matchType)
    {
        this.matchType = matchType;
        var spriteConfig = Resources.Load<ItemSpriteConfig>("ItemSpriteConfig");
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;

        base.InitializeFromProperties(config, sprite);
        Debug.Log("CubeItem initialized with match type: " + matchType);
    }
}
