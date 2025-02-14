using UnityEngine;
using System.Collections.Generic;

// RocketItem: Implements rocket behavior and explosion chain reactions.
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
            selectedSprite = rocketType == RocketType.Horizontal 
                ? spriteConfig.GetSpriteForItemType(config.ItemType) 
                : spriteConfig.GetBonusSpriteForItemType(config.ItemType);
        }
        base.InitializeFromProperties(config, selectedSprite);
    }

    public void ApplyExplosionSpriteRendererProperties(SpriteRenderer sr, Sprite sprite)
    {
        sr.sprite = sprite;
        sr.transform.localPosition = Vector3.zero;
        sr.transform.localScale = Vector2.one;
        sr.sortingLayerID = SortingLayer.NameToID("Explosion");
        sr.sortingOrder = 100;
    }
    
    // Executes rocket explosion with chain reactions.
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
                foreach (var item in itemsToDestroy)
                {
                    if (item != null && item.gameObject != null)
                    {
                        if (item is RocketItem r && r.RocketType != this.rocketType)
                            RocketManager.Instance.ExplodeRocket(r);
                        else
                            item.TryExecute(source);
                    }
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
                foreach (var item in itemsToDestroy)
                {
                    if (item != null && item.gameObject != null)
                    {
                        if (item is RocketItem r && r.RocketType != this.rocketType)
                            RocketManager.Instance.ExplodeRocket(r);
                        else
                            item.TryExecute(source);
                    }
                }
                base.TryExecute(source);
                GameEvents.BoardUpdated();
            });
        }
    }
}
