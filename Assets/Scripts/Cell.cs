using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Cell : MonoBehaviour
{
    [HideInInspector]
    public int x, y;

    public TextMesh debugLabel;

    public GameObject currentItem;

    public List<Cell> neighbours = new(); 


    public void Prepare(int xCoord, int yCoord)
    {
        x = xCoord;
        y = yCoord;
        UpdateLabel();
    }

    public void UpdateLabel()
    {
        if (debugLabel != null)
        {
            debugLabel.text = $"({x},{y})";
        }
    }

    public void UpdateNeighbours(Cell[,] grid, int gridWidth, int gridHeight)
    {
        neighbours.Clear();

        if (y + 1 < gridHeight)
            neighbours.Add(grid[x, y + 1]);

        if (y - 1 >= 0)
            neighbours.Add(grid[x, y - 1]);

        if (x + 1 < gridWidth)
            neighbours.Add(grid[x + 1, y]);

        if (x - 1 >= 0)
            neighbours.Add(grid[x - 1, y]);
    }


    public void HandleTap()
    {
        Debug.Log($"Cell tapped at ({x}, {y})");
        // TODO: Implement additional tap handling logic (e.g., notify the GameGrid or interact with currentItem).
    }

    public Cell GetFallTarget()
    {
        // Look for a neighbour with the same x coordinate and y one less than this cell.
        foreach (Cell neighbour in neighbours)
        {
            if (neighbour != null && neighbour.x == x && neighbour.y == y - 1)
            {
                return neighbour;
            }
        }
        return null;
    }
}
