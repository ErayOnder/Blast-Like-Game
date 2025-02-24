using System.Collections.Generic;
using UnityEngine;

// Role: Finds connected matching cells on the grid.
public static class MatchFinder
{
    public static List<Cell> FindMatches(Cell startCell)
    {
        return FindConnectedCells(startCell, (item) => item is CubeItem cube && cube.MatchType == (startCell.Item as CubeItem)?.MatchType);
    }

    public static List<Cell> FindConnectedRockets(Cell startCell)
    {
        return FindConnectedCells(startCell, (item) => item is RocketItem);
    }

    private static List<Cell> FindConnectedCells(Cell startCell, System.Func<Item, bool> matcher)
    {
        List<Cell> matchedCells = new();
        if (startCell == null || startCell.Item == null)
        {
            Debug.Log("No item attached to the starting cell.");
            return matchedCells;
        }

        if (!matcher(startCell.Item))
        {
            Debug.Log("Starting cell item doesn't match the criteria.");
            return matchedCells;
        }

        HashSet<Cell> visited = new();
        Queue<Cell> toVisit = new();
        toVisit.Enqueue(startCell);
        visited.Add(startCell);
        matchedCells.Add(startCell);

        while (toVisit.Count > 0)
        {
            Cell current = toVisit.Dequeue();
            foreach (Cell neighbor in GetAdjacentCells(current))
            {
                if (neighbor != null && !visited.Contains(neighbor) && neighbor.Item != null)
                {
                    if (matcher(neighbor.Item))
                    {
                        visited.Add(neighbor);
                        matchedCells.Add(neighbor);
                        toVisit.Enqueue(neighbor);
                    }
                }
            }
        }
        return matchedCells;
    }

    // Retrieves adjacent cells (up, down, left, right) based on the cell's position.
    private static List<Cell> GetAdjacentCells(Cell cell)
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
}
