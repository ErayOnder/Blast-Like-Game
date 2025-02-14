using UnityEngine;

public class StoneItem : DestructibleObstacleItem
{
    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        InitializeObstacle(config, spriteConfig);
    }

    protected override bool CanApplyDamage(DamageSource source)
    {
        // Only subtract health when the damage source is Rocket.
        return source == DamageSource.Rocket;
    }
}