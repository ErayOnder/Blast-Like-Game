using UnityEngine;
using System.Collections.Generic;

public class RocketItem : Item
{
    private RocketType rocketType;

    public RocketType RocketType => rocketType;

    public void InitializeConfig(ItemConfig config, ItemSpriteConfig spriteConfig, RocketType rocketType)
    {
        this.rocketType = rocketType;
        Sprite selectedSprite = null;
        if (spriteConfig != null)
        {
            // Use the normal sprite for Horizontal rockets
            // Use the bonus sprite (assigned as vertical) for Vertical rockets.
            selectedSprite = rocketType == RocketType.Horizontal ?
                spriteConfig.GetSpriteForItemType(config.ItemType) :
                spriteConfig.GetBonusSpriteForItemType(config.ItemType);
        }
        base.InitializeFromProperties(config, selectedSprite);
    }

    public void ApplyExplosionSpriteRendererProperties(SpriteRenderer sr, Sprite sprite)
    {
        sr.sprite = sprite;
        // Reset local position in case it's not on the same object.
        sr.transform.localPosition = Vector3.zero;
        // Use the natural size of the sprite for explosion effects.
        sr.transform.localScale = Vector2.one;
        // Place the explosion on its dedicated sorting layer.
        sr.sortingLayerID = SortingLayer.NameToID("Explosion");
        // A fixed sorting order (adjust this value as needed).
        sr.sortingOrder = 100;
    }
    
    // Override TryExecute to delay destruction until after the explosion animation.
    public void TryExecuteWithItems(DamageSource source, List<Item> itemsToDestroy)
    {
        if (rocketAnimation == null)
        {
            base.TryExecute(source);
            return;
        }
        
        Cell currentCell = this.Cell;
        if (currentCell == null || currentCell.GameGrid == null)
        {
            base.TryExecute(source);
            return;
        }
        
        GameGrid grid = currentCell.GameGrid;
        if (rocketType == RocketType.Horizontal)
        {
            int row = currentCell.Y;
            Cell leftTarget = grid.Grid[0, row];
            Cell rightTarget = grid.Grid[grid.Width - 1, row];
            
            rocketAnimation.PlayHorizontalExplosionAnimation(leftTarget, rightTarget, () =>
            {
                // Destroy collected items after animation completes
                foreach (var item in itemsToDestroy)
                {
                    if (item != null && item.gameObject != null)
                        item.TryExecute(source);
                }
                base.TryExecute(source);
                GameEvents.BoardUpdated();
            });
        }
        else
        {
            int col = currentCell.X;
            Cell upTarget = grid.Grid[col, grid.Height - 1];
            Cell downTarget = grid.Grid[col, 0];
            
            rocketAnimation.PlayVerticalExplosionAnimation(upTarget, downTarget, () =>
            {
                // Destroy collected items after animation completes
                foreach (var item in itemsToDestroy)
                {
                    if (item != null && item.gameObject != null)
                        item.TryExecute(source);
                }
                base.TryExecute(source);
                GameEvents.BoardUpdated();
            });
        }
    }

    public void TryExecuteCombo(DamageSource source, List<Item> itemsToDestroy, bool waitForOthers = false)
    {
        if (rocketAnimation == null)
        {
            base.TryExecute(source);
            return;
        }
        
        Cell currentCell = this.Cell;
        if (currentCell == null || currentCell.GameGrid == null)
        {
            base.TryExecute(source);
            return;
        }
        
        GameGrid grid = currentCell.GameGrid;
        if (rocketType == RocketType.Horizontal)
        {
            int row = currentCell.Y;
            Cell leftTarget = grid.Grid[0, row];
            Cell rightTarget = grid.Grid[grid.Width - 1, row];
            
            rocketAnimation.PlayHorizontalExplosionAnimation(leftTarget, rightTarget, () =>
            {
                if (!waitForOthers)
                {
                    // Destroy collected items after animation completes
                    foreach (var item in itemsToDestroy)
                    {
                        if (item != null && item.gameObject != null)
                            item.TryExecute(source);
                    }
                    base.TryExecute(source);
                    GameEvents.BoardUpdated();
                }
            });
        }
        else
        {
            int col = currentCell.X;
            Cell upTarget = grid.Grid[col, grid.Height - 1];
            Cell downTarget = grid.Grid[col, 0];
            
            rocketAnimation.PlayVerticalExplosionAnimation(upTarget, downTarget, () =>
            {
                if (!waitForOthers)
                {
                    // Destroy collected items after animation completes
                    foreach (var item in itemsToDestroy)
                    {
                        if (item != null && item.gameObject != null)
                            item.TryExecute(source);
                    }
                    base.TryExecute(source);
                    GameEvents.BoardUpdated();
                }
            });
        }
    }
}
