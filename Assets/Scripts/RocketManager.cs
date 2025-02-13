using UnityEngine;
using System.Collections.Generic;

public class RocketManager : Singleton<RocketManager>
{
    // Entry point: when a rocket is tapped
    public void ExplodeRocket(RocketItem rocket)
    {
        if (rocket == null)
            return;

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
        if (rocket == null)
            return;

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

        rocket.TryExecute();
    }

    // Combo explosion logic
    private void ExplodeCombo(RocketItem tappedRocket, List<RocketItem> comboGroup)
    {
        if (tappedRocket == null || tappedRocket.Cell == null)
            return;

        // Store center cell and its coordinates before removal.
        Cell centerCell = tappedRocket.Cell;
        int centerX = centerCell.X;
        int centerY = centerCell.Y;
        GameGrid grid = centerCell.GameGrid;

        // Remove all rockets in the combo (they are now part of the combo explosion).
        foreach (RocketItem rocket in comboGroup)
        {
            if (rocket.Cell != null)
            {
                rocket.Cell.Item = null;
            }
            rocket.TryExecute();
        }

        // First, destroy all items inside the 3x3 block centered at the tapped cell.
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
                    // If orientations differ, trigger its explosion; otherwise simply destroy it.
                    if (targetRocket.RocketType != tappedRocket.RocketType)
                    {
                        ExplodeRocket(targetRocket);
                    }
                    else
                    {
                        targetRocket.ExecuteBonusEffect();
                    }
                }
                else
                {
                    targetItem.TryExecute();
                }
            }
        }

        // Next, propagate the explosion outward from the edges of the 3x3 block.
        // For horizontal propagation, iterate over the three rows of the combo zone.
        for (int r = -1; r <= 1; r++)
        {
            int row = centerY + r;
            if (row < 0 || row >= grid.Height)
                continue;

            // Leftward: start from the left edge of the combo area.
            if (centerX - 1 >= 0)
            {
                ProcessExplosionDirection(grid.Grid[centerX - 1, row], -1, 0, tappedRocket.RocketType);
            }
            // Rightward: start from the right edge.
            if (centerX + 1 < grid.Width)
            {
                ProcessExplosionDirection(grid.Grid[centerX + 1, row], 1, 0, tappedRocket.RocketType);
            }
        }

        // For vertical propagation, iterate over the three columns of the combo zone.
        for (int c = -1; c <= 1; c++)
        {
            int col = centerX + c;
            if (col < 0 || col >= grid.Width)
                continue;

            // Upward: start from the top edge.
            if (centerY + 1 < grid.Height)
            {
                ProcessExplosionDirection(grid.Grid[col, centerY + 1], 0, 1, tappedRocket.RocketType);
            }
            // Downward: start from the bottom edge.
            if (centerY - 1 >= 0)
            {
                ProcessExplosionDirection(grid.Grid[col, centerY - 1], 0, -1, tappedRocket.RocketType);
            }
        }
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

            // Stop propagation if the item does not blast with explosion.
            if (!targetItem.blastsWithExplosion)
                break;

            // If the item is a rocket, check its orientation.
            RocketItem targetRocket = targetItem as RocketItem;
            if (targetRocket != null)
            {
                if (targetRocket.RocketType != sourceRocketType)
                {
                    ExplodeRocket(targetRocket);
                }
                else
                {
                    targetRocket.ExecuteBonusEffect();
                }
            }
            else
            {
                targetItem.TryExecute();
            }
        }
    }
}
