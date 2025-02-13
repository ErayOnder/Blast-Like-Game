using UnityEngine;

public class RocketItem : Item
{
    public void InitializeConfig(ItemBase baseItem, ItemConfig config, RocketType rocketType)
    {
        var spriteConfig = Resources.Load<ItemSpriteConfig>("ItemSpriteConfig");
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;

        base.InitializeFromProperties(config, sprite);
    }
}