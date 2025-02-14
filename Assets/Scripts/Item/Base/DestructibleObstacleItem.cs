using UnityEngine;

public abstract class DestructibleObstacleItem : Item, IDestructibleObstacle
{
    // When TryExecute is called, we delegate to our ApplyDamage method.
    public override void TryExecute(DamageSource source)
    {
        ApplyDamage(source);
    }

    // Default implementation: subtract 1 health if the damage source is acceptable.
    // Then, if health reaches zero, perform the destruction.
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
    
    // A helper method to consolidate common initialization logic.
    protected void InitializeObstacle(ItemConfig config, ItemSpriteConfig spriteConfig)
    {
        Sprite sprite = spriteConfig != null ? spriteConfig.GetSpriteForItemType(config.ItemType) : null;
        base.InitializeFromProperties(config, sprite);
    }
}
