using UnityEngine;

public class VaseItem : DestructibleObstacleItem
{
    private int maxHealth;
    private bool spriteUpdated = false;

    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        InitializeObstacle(config, spriteConfig);
        maxHealth = health;
    }

    protected override void OnDamageReceived(DamageSource source)
    {
        // If health drops to or below half and we haven't updated yet, update to bonus sprite.
        if (!spriteUpdated && health <= maxHealth / 2)
        {
            UpdateSpriteForBonus();
            spriteUpdated = true;
        }
    }
}