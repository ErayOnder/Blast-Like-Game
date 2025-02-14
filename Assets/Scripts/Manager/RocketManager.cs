using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RocketManager : Singleton<RocketManager>
{
    // Entry point: when a rocket is tapped
    public void ExplodeRocket(RocketItem rocket)
    {
        if (rocket == null)
            return;
        
        // Deduct a move when a rocket is tapped.
        LevelProgress.Instance.ProcessMove();

        // Look for adjacent (cardinal) rockets to form a combo.
        List<RocketItem> comboGroup = FindComboGroup(rocket);
        // If two or more rockets combine, trigger the combo explosion.
        if (comboGroup.Count >= 2)
        {
            ExplodeCombo(rocket, comboGroup);
        }
        else
        {
            ExplodeRocketSingle(rocket);
        }

        GameEvents.BoardUpdated();
    }

    // Single explosion logic (unchanged)
    private void ExplodeRocketSingle(RocketItem rocket)
    {
        if (rocket == null || rocket.Cell == null)
            return;

        Cell originCell = rocket.Cell;
        List<Item> itemsToDestroy = new List<Item>();

        // Collect items to destroy instead of destroying them immediately
        if (rocket.RocketType == RocketType.Horizontal)
        {
            CollectItemsInDirection(originCell, -1, 0, itemsToDestroy);
            CollectItemsInDirection(originCell, 1, 0, itemsToDestroy);
        }
        else // RocketType.Vertical
        {
            CollectItemsInDirection(originCell, 0, 1, itemsToDestroy);
            CollectItemsInDirection(originCell, 0, -1, itemsToDestroy);
        }

        // Start the rocket animation, destroy items only after it completes
        rocket.TryExecuteWithItems(DamageSource.Rocket, itemsToDestroy);
    }

    private void CollectItemsInDirection(Cell startCell, int dx, int dy, List<Item> itemsToDestroy)
    {
        if (startCell == null || startCell.GameGrid == null)
            return;

        int x = startCell.X;
        int y = startCell.Y;
        GameGrid grid = startCell.GameGrid;

        while (true)
        {
            x += dx;
            y += dy;

            if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height)
                break;

            Cell cell = grid.Grid[x, y];
            if (cell == null || cell.Item == null)
                continue;

            Item targetItem = cell.Item;
            if (!targetItem.blastsWithExplosion)
                break;

            itemsToDestroy.Add(targetItem);
        }
    }

    // Combo explosion logic
    private void ExplodeCombo(RocketItem tappedRocket, List<RocketItem> comboGroup)
    {
        if (tappedRocket == null || tappedRocket.Cell == null)
            return;

        // Store center cell and its coordinates
        Cell centerCell = tappedRocket.Cell;
        GameGrid grid = centerCell.GameGrid;
        
        // Collect all items that will be destroyed by the combo
        Dictionary<RocketItem, List<Item>> rocketTargets = new Dictionary<RocketItem, List<Item>>();
        
        // For each rocket in the combo group
        foreach (RocketItem rocket in comboGroup)
        {
            List<Item> itemsToDestroy = new List<Item>();
            
            // Add horizontal items from the 3x3 grid centered on the tapped rocket
            for (int y = centerCell.Y - 1; y <= centerCell.Y + 1; y++)
            {
                if (y >= 0 && y < grid.Height)
                {
                    CollectItemsInDirection(grid.Grid[0, y], 1, 0, itemsToDestroy);
                }
            }
            
            // Add vertical items from the 3x3 grid centered on the tapped rocket
            for (int x = centerCell.X - 1; x <= centerCell.X + 1; x++)
            {
                if (x >= 0 && x < grid.Width)
                {
                    CollectItemsInDirection(grid.Grid[x, 0], 0, 1, itemsToDestroy);
                }
            }
            
            rocketTargets[rocket] = itemsToDestroy;
        }

        // Trigger all rocket animations simultaneously
        foreach (RocketItem rocket in comboGroup)
        {
            rocket.TryExecuteCombo(DamageSource.Rocket, rocketTargets[rocket], true);
        }

        StartCoroutine(WaitForComboAnimations(comboGroup, rocketTargets));
    }

    private IEnumerator WaitForComboAnimations(List<RocketItem> rockets, Dictionary<RocketItem, List<Item>> rocketTargets)
    {
        // Wait for animation duration plus a small buffer
        yield return new WaitForSeconds(0.8f);

        // Destroy all items and rockets
        foreach (var kvp in rocketTargets)
        {
            foreach (var item in kvp.Value)
            {
                if (item != null && item.gameObject != null)
                    item.TryExecute(DamageSource.Rocket);
            }
            if (kvp.Key != null && kvp.Key.gameObject != null)
                kvp.Key.TryExecute(DamageSource.Rocket);
        }

        GameEvents.BoardUpdated();
    }

    // Helper: find all rockets (using cardinal neighbors) connected to the tapped rocket.
    private List<RocketItem> FindComboGroup(RocketItem startRocket)
    {
        List<RocketItem> comboGroup = new List<RocketItem>();
        if (startRocket == null || startRocket.Cell == null)
            return comboGroup;

        Queue<Cell> toVisit = new Queue<Cell>();
        HashSet<Cell> visited = new HashSet<Cell>();
        toVisit.Enqueue(startRocket.Cell);
        visited.Add(startRocket.Cell);

        while (toVisit.Count > 0)
        {
            Cell current = toVisit.Dequeue();
            if (current.Item is RocketItem r)
            {
                if (!comboGroup.Contains(r))
                {
                    comboGroup.Add(r);
                }
            }
            foreach (Cell neighbor in GetCardinalNeighbors(current))
            {
                if (neighbor != null && !visited.Contains(neighbor) && neighbor.Item is RocketItem)
                {
                    visited.Add(neighbor);
                    toVisit.Enqueue(neighbor);
                }
            }
        }

        return comboGroup;
    }

    // Helper: returns the up/down/left/right neighbors of a cell.
    private List<Cell> GetCardinalNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        GameGrid grid = cell.GameGrid;
        if (grid == null || grid.Grid == null)
            return neighbors;
        int x = cell.X;
        int y = cell.Y;
        if (x > 0)
            neighbors.Add(grid.Grid[x - 1, y]);
        if (x < grid.Width - 1)
            neighbors.Add(grid.Grid[x + 1, y]);
        if (y > 0)
            neighbors.Add(grid.Grid[x, y - 1]);
        if (y < grid.Height - 1)
            neighbors.Add(grid.Grid[x, y + 1]);
        return neighbors;
    }

    // Propagates the explosion in a single direction, one cell after the starting cell,
    // until the grid edge is reached or a non-blastable item stops the propagation.
    private void ProcessExplosionDirection(Cell startCell, int dx, int dy, RocketType sourceRocketType)
    {
        if (startCell == null || startCell.GameGrid == null)
            return;

        int x = startCell.X;
        int y = startCell.Y;
        GameGrid grid = startCell.GameGrid;

        while (true)
        {
            x += dx;
            y += dy;

            // Stop if we've reached the grid limits.
            if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height)
                break;

            Cell cell = grid.Grid[x, y];
            if (cell == null)
                continue;

            // If the cell is empty, continue outward.
            if (cell.Item == null)
                continue;

            Item targetItem = cell.Item;

            if (!targetItem.blastsWithExplosion)
                break;

            if (targetItem is IDestructibleObstacle)
            {
                targetItem.TryExecute(DamageSource.Rocket);
            }
            else if (targetItem is RocketItem targetRocket)
            {
                if (targetRocket.RocketType != sourceRocketType)
                {
                    // Collect items for this rocket before triggering its explosion
                    List<Item> itemsToDestroy = new List<Item>();
                    if (targetRocket.RocketType == RocketType.Horizontal)
                    {
                        CollectItemsInDirection(cell, -1, 0, itemsToDestroy);
                        CollectItemsInDirection(cell, 1, 0, itemsToDestroy);
                    }
                    else // RocketType.Vertical
                    {
                        CollectItemsInDirection(cell, 0, 1, itemsToDestroy);
                        CollectItemsInDirection(cell, 0, -1, itemsToDestroy);
                    }
                    targetRocket.TryExecuteWithItems(DamageSource.Rocket, itemsToDestroy);
                }
                else
                {
                    targetRocket.TryExecute(DamageSource.Rocket);
                }
            }
            else
            {
                targetItem.TryExecute(DamageSource.Rocket);
            }
        }
    }
}
