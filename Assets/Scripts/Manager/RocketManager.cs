using UnityEngine;
using System.Collections.Generic;

// RocketManager: Controls rocket explosion logic, including combo explosions.
public class RocketManager : Singleton<RocketManager>
{
    // Explodes a rocket; triggers combo explosion if adjacent rockets are found.
    public void ExplodeRocket(RocketItem rocket, bool isInitialClick = false)
    {
        if (rocket == null)
            return;

        if (isInitialClick)
        {
            LevelProgress.Instance.ProcessMove();
        }

        List<RocketItem> comboGroup = FindComboGroup(rocket);
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

    private void ExplodeRocketSingle(RocketItem rocket)
    {
        if (rocket == null || rocket.Cell == null)
            return;

        Cell originCell = rocket.Cell;
        List<Item> itemsToDestroy = new List<Item>();

        if (rocket.RocketType == RocketType.Horizontal)
        {
            CollectItemsInDirection(originCell, -1, 0, itemsToDestroy);
            CollectItemsInDirection(originCell, 1, 0, itemsToDestroy);
        }
        else
        {
            CollectItemsInDirection(originCell, 0, 1, itemsToDestroy);
            CollectItemsInDirection(originCell, 0, -1, itemsToDestroy);
        }

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

    // Executes combo explosion in a 3x3 area and propagates outward.
    private void ExplodeCombo(RocketItem tappedRocket, List<RocketItem> comboGroup)
    {
        if (tappedRocket == null || tappedRocket.Cell == null)
            return;

        Cell centerCell = tappedRocket.Cell;
        int centerX = centerCell.X;
        int centerY = centerCell.Y;
        GameGrid grid = centerCell.GameGrid;

        foreach (RocketItem rocket in comboGroup)
        {
            rocket.TryExecute(DamageSource.Rocket);
        }

        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height)
                    continue;
                Cell cell = grid.Grid[x, y];
                if (cell == null || cell.Item == null)
                    continue;
                Item targetItem = cell.Item;
                if (!targetItem.blastsWithExplosion)
                    continue;
                RocketItem targetRocket = targetItem as RocketItem;
                if (targetRocket != null)
                {
                    if (targetRocket.RocketType != tappedRocket.RocketType)
                    {
                        ExplodeRocket(targetRocket);
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

        for (int r = -1; r <= 1; r++)
        {
            int row = centerY + r;
            if (row < 0 || row >= grid.Height)
                continue;

            if (centerX - 1 >= 0)
            {
                ProcessExplosionDirection(grid.Grid[centerX - 1, row], -1, 0, tappedRocket.RocketType);
            }
            if (centerX + 1 < grid.Width)
            {
                ProcessExplosionDirection(grid.Grid[centerX + 1, row], 1, 0, tappedRocket.RocketType);
            }
        }

        for (int c = -1; c <= 1; c++)
        {
            int col = centerX + c;
            if (col < 0 || col >= grid.Width)
                continue;

            if (centerY + 1 < grid.Height)
            {
                ProcessExplosionDirection(grid.Grid[col, centerY + 1], 0, 1, tappedRocket.RocketType);
            }
            if (centerY - 1 >= 0)
            {
                ProcessExplosionDirection(grid.Grid[col, centerY - 1], 0, -1, tappedRocket.RocketType);
            }
        }
    }

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

    // Propagates explosion in one direction until a non-blastable item is encountered.
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

            if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height)
                break;

            Cell cell = grid.Grid[x, y];
            if (cell == null)
                continue;

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
                    ExplodeRocket(targetRocket);
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
