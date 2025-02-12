using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{

    public TextMesh debugLabel;
    [HideInInspector] public int X;
    [HideInInspector] public int Y;

    public GameGrid GameGrid { get; private set; }

    private Item item;

    public Item Item
    {
        get => item;
        set
        {
            if (item == value)
                return;

            if (item != null && item.Cell == this)
            {
                item.Cell = null;
            }

            item = value;

            if (item != null && item.Cell != this)
            {
                item.Cell = this;
            }
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

        Debug.Log($"Cell tapped at: ({X}, {Y})");

        CubeItem cubeItem = item as CubeItem;
        if (cubeItem != null)
        {
            var matchingCells = MatchManager.Instance.FindMatches(this);
            Debug.Log($"Match result: {matchingCells.Count} cells connected.");
        }
    }

}
