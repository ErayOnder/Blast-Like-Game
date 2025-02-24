using UnityEngine;

// Role: Manages damage logic for destructible obstacles.
public abstract class DestructibleObstacleItem : Item, IDestructibleObstacle
{
    // When TryExecute is called, we delegate to our ApplyDamage method.
    public override void TryExecute(DamageSource source)
    {
        ApplyDamage(source);
    }

    // Applies damage, reduces health, and destroys the item when depleted.
    public virtual void ApplyDamage(DamageSource source)
    {
        if (!destructibleWithRocket && source == DamageSource.Rocket)
            return;
        if (!destructibleWithBlast && source == DamageSource.Blast)
            return;

        health--;
        OnDamageReceived(source);
        if (health <= 0)
        {
            LevelProgress.Instance.ProcessObstacleDestroyed(itemType);
            // Call the base implementation (from Item) to play particles, clear the cell, and destroy the object.
            base.TryExecute(source);
        }
    }

    protected virtual void OnDamageReceived(DamageSource source)
    {
        // Default: no extra action.
    }
    
    // Initializes obstacle properties.
    protected void InitializeObstacle(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;
        base.InitializeFromProperties(config, sprite);
    }
}
