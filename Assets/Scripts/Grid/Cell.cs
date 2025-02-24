using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Represents a grid cell. Manages item assignment and interactions.
public class Cell : MonoBehaviour
{
    public TextMesh debugLabel;
    [HideInInspector] public int X;
    [HideInInspector] public int Y;

    public GameGrid GameGrid { get; private set; }

    private Item item;

    public Item Item
    {
        get { return item; }
        set
        {
            if (item == value) return;
            var oldItem = item;
            item = value;
            if (oldItem != null && Equals(oldItem.Cell, this))
                oldItem.Cell = null;
            if (value != null)
                value.Cell = this;
        }
    }

    public void InitializeCell(int xCoord, int yCoord, GameGrid grid)
    {
        GameGrid = grid;
        X = xCoord;
        Y = yCoord;
        gameObject.name = $"cell_{X}_{Y}";
        transform.localPosition = new Vector3(X, Y);
    }

    public void CellTapped()
    {
        if (item == null)
            return;

        RocketItem rocket = item as RocketItem;
        if (rocket != null)
        {
            RocketManager.Instance.ExplodeRocket(rocket, true);
            return;
        }

        CubeItem cube = item as CubeItem;
        if (cube != null)
        {
            MatchManager.Instance.ProcessMatch(this);
        }
    }

    public Cell GetCellBelow()
    {
        if (GameGrid != null && Y > 0)
            return GameGrid.Grid[X, Y - 1];
        return null;
    }

    // Finds the lowest empty cell below for falling items.
    public Cell GetFallTarget()
    {
        Cell targetCell = this;
        while (true)
        {
            Cell below = targetCell.GetCellBelow();
            if (below != null && below.Item == null)
                targetCell = below;
            else
                break;
        }
        return targetCell;
    }
}
