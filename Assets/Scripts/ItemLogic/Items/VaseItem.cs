using UnityEngine;

public class VaseItem : Item, IDestructibleObstacle
{
    private int maxHealth;
    private bool spriteUpdated = false;

    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;
        base.InitializeFromProperties(config, sprite);
        maxHealth = health;
    }

    public override void TryExecute(DamageSource source)
    {
        ApplyDamage(source);
    }

    public void ApplyDamage(DamageSource source)
    {
        health--;

        if (!spriteUpdated && health <= maxHealth / 2)
        {
            UpdateSpriteForBonus();
            spriteUpdated = true;
        }

        if (health <= 0)
        {
            LevelProgress.Instance.ProcessObstacleDestroyed(itemType);
            base.TryExecute(source);
        }
        
    }
}