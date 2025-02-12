using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{

    public TextMesh debugLabel;
    [HideInInspector] public int X;
    [HideInInspector] public int Y;

    public GameGrid GameGrid { get; private set; }

    // Backing field for the item on this cell.
    private Item item;

    // Property for getting/setting the item on this cell.
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

    public void Prepare(int xCoord, int yCoord, GameGrid grid)
    {
        GameGrid = grid;
        X = xCoord;
        Y = yCoord;
        gameObject.name = $"cell_{X}_{Y}";
        transform.localPosition = new Vector3(X, Y);
    }

}
