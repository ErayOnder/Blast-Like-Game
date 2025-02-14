using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameGrid : MonoBehaviour
{
    [SerializeField]
    private Cell cellPrefab;
    public Transform cellsParent;
    public Transform itemsParent;

    public Cell[,] Grid { get; private set; }    
    public int Width { get; private set; }
    public int Height { get; private set; }

    public void BuildGrid(LevelInfo levelinfo)
    {
        Width = levelinfo.grid_width;
        Height = levelinfo.grid_height;
        Grid = new Cell[Width, Height];

        if (cellsParent != null)
        {
            foreach (Transform child in cellsParent)
            {
                Destroy(child.gameObject);
            }
        }

        ResizeBoard();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Grid[x, y] = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, cellsParent);
                Grid[x, y].InitializeCell(x, y, this);
            }
        }        

        GameEvents.BoardUpdated();
    }

    private void ResizeBoard()
    {
        Transform currTrans = this.transform;

        float newX = (9 - Width) * 0.5f;
        float newY = (9 - Height) * 0.5f;

        this.transform.position = new Vector3(newX, newY, currTrans.position.z);
    }
    
}
