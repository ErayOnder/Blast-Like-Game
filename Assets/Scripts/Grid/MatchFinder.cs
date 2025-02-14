using System.Collections.Generic;
using UnityEngine;

// Role: Finds connected matching cells on the grid.
public static class MatchFinder
{
    // Returns cells connected to the start cell that share the same cube match type.
    public static List<Cell> FindMatches(Cell startCell)
    {
        List<Cell> matchedCells = new();
        if (startCell == null || startCell.Item == null)
        {
            Debug.Log("No item attached to the starting cell.");
            return matchedCells;
        }
        CubeItem cubeItem = startCell.Item as CubeItem;
        if (cubeItem == null)
        {
            Debug.Log("Item in starting cell is not matchable.");
            return matchedCells;
        }
        MatchType targetMatchType = cubeItem.MatchType;
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
                    CubeItem neighborCube = neighbor.Item as CubeItem;
                    if (neighborCube != null && neighborCube.MatchType == targetMatchType)
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
