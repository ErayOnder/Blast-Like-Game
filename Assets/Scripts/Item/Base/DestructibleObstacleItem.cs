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
        if (!CanApplyDamage(source))
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

    // Override this method in subclasses if damage should be applied conditionally.
    protected virtual bool CanApplyDamage(DamageSource source)
    {
        return true;
    }

    // Hook for extra behavior right after taking damage.
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
