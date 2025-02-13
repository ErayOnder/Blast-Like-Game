using UnityEngine;

public class StoneItem : Item
{
    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;

        base.InitializeFromProperties(config, sprite);
    }
}