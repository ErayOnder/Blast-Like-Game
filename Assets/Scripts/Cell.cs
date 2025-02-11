using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Cell : MonoBehaviour
{

    public TextMesh debugLabel;
    [HideInInspector] public int X;
    [HideInInspector] public int Y;

    public GameGrid GameGrid { get; private set; }


    public void Prepare(int xCoord, int yCoord, GameGrid grid)
    {
        GameGrid = grid;
        X = xCoord;
        Y = yCoord;
        gameObject.name = $"cell_{X}_{Y}";
        transform.localPosition = new Vector3(X, Y);
    }

}
