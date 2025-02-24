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
            if (!targetItem.destructibleWithRocket)
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

        // Destroy all items in the 3x3 area
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (x < 0 || x >= grid.Width || y < 0 || y >= grid.Height)
                    continue;
                Cell cell = grid.Grid[x, y];
                if (cell != null && cell.Item != null)
                {
                    cell.Item.TryExecute(DamageSource.Rocket);
                }
            }
        }

        // Create and explode 3 horizontal rockets
        for (int y = centerY - 1; y <= centerY + 1; y++)
        {
            if (y < 0 || y >= grid.Height)
                continue;
            Item horizontalRocket = ItemFactory.Instance.CreateItem(ItemType.HorizontalRocket, grid.itemsParent);
            if (horizontalRocket is RocketItem rocket)
            {
                Cell targetCell = grid.Grid[centerX, y];
                rocket.transform.position = targetCell.transform.position;
                rocket.Cell = targetCell;
                ExplodeRocketSingle(rocket);
            }
        }

        // Create and explode 3 vertical rockets
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            if (x < 0 || x >= grid.Width)
                continue;

            Item verticalRocket = ItemFactory.Instance.CreateItem(ItemType.VerticalRocket, grid.itemsParent);
            if (verticalRocket is RocketItem rocket)
            {
                Cell targetCell = grid.Grid[x, centerY];    
                rocket.transform.position = targetCell.transform.position;
                rocket.Cell = targetCell;
                ExplodeRocketSingle(rocket);
            }
        }
    }

    private List<RocketItem> FindComboGroup(RocketItem startRocket)
    {
        List<RocketItem> comboGroup = new List<RocketItem>();
        if (startRocket == null || startRocket.Cell == null)
            return comboGroup;

        List<Cell> connectedCells = MatchFinder.FindConnectedRockets(startRocket.Cell);
        foreach (Cell cell in connectedCells)
        {
            if (cell.Item is RocketItem rocket)
            {
                comboGroup.Add(rocket);
            }
        }

        return comboGroup;
    }

}
