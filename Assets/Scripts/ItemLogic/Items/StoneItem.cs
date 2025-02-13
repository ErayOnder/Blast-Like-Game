using UnityEngine;

public class StoneItem : Item, IDestructibleObstacle
{
    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;
        base.InitializeFromProperties(config, sprite);
    }

    public override void TryExecute(DamageSource source)
    {
        ApplyDamage(source);
    }

    public void ApplyDamage(DamageSource source)
    {
        if (source == DamageSource.Rocket)
        {
            health--;
            if (health <= 0)
            {
                base.TryExecute(source);
            }
        }
    }

}