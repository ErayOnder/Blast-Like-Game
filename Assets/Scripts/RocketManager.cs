using UnityEngine;
using System.Collections;

public class RocketManager : Singleton<RocketManager>
{
    public void ExplodeRocket(RocketItem rocket)
    {
        if (rocket == null)
            return;
            
        // Remove the rocket from its cell.
        Cell originCell = rocket.Cell;
        if (originCell != null)
        {
            originCell.Item = null;
        }
        
        // Propagate the explosion according to the rocket's orientation.
        if (rocket.RocketType == RocketType.Horizontal)
        {
            ProcessExplosionDirection(originCell, -1, 0, rocket.RocketType);
            ProcessExplosionDirection(originCell, 1, 0, rocket.RocketType);
        }
        else // RocketType.Vertical
        {
            ProcessExplosionDirection(originCell, 0, 1, rocket.RocketType);
            ProcessExplosionDirection(originCell, 0, -1, rocket.RocketType);
        }
        
        // Finally, destroy this rocket.
        rocket.TryExecute();
        
        // Instead of directly starting the cascade, raise the board updated event.
        GameEvents.BoardUpdated();
    }
    
    private void ProcessExplosionDirection(Cell startCell, int dx, int dy, RocketType sourceRocketType)
    {
        if (startCell == null || startCell.GameGrid == null)
            return;
        
        int x = startCell.X;
        int y = startCell.Y;
        GameGrid grid = startCell.GameGrid;
        
        // Propagate cell by cell in the specified direction.
        while (true)
        {
            x += dx;
            y += dy;
            
            // Stop if we've reached the limits of the grid.
            if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height)
                break;
                
            Cell cell = grid.Grid[x, y];
            if (cell == null)
                continue;
                
            // If the cell is empty, move to the next one.
            if (cell.Item == null)
                continue;
            
            Item targetItem = cell.Item;
            
            // If the item does NOT blast with explosion, stop the propagation.
            if (!targetItem.blastsWithExplosion)
                break;
            
            // If the item is a rocket, check its orientation.
            RocketItem targetRocket = targetItem as RocketItem;
            if (targetRocket != null)
            {
                if (targetRocket.RocketType != sourceRocketType)
                {
                    // Different oriented rocket: you may choose to trigger its explosion.
                    ExplodeRocket(targetRocket);
                }
                else
                {
                    // Same oriented rocket: simply destroy it.
                    targetRocket.ExecuteBonusEffect();
                }
            }
            else
            {
                // For non-rocket items, just destroy the item.
                targetItem.TryExecute();
            }
            
            // Continue to the next cell.
        }
    }
}
