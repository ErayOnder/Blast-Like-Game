using UnityEngine;

public class BoxItem : Item
{
    public void InitializeConfig(ItemBase baseItem, ItemConfig config)
    {
        var spriteConfig = Resources.Load<ItemSpriteConfig>("ItemSpriteConfig");
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;

        base.InitializeFromProperties(config, sprite);
    }
}