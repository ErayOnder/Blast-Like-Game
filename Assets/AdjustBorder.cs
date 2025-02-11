using System;
using UnityEngine;

public class AdjustBorder : MonoBehaviour
{
    [SerializeField] 
    private GameGrid gameGrid;

    private const float WIDTH_PADDING = 0.35f;
    private const float HEIGHT_PADDING = 0.45f;

    void OnEnable()
    {
        if (gameGrid != null)
            gameGrid.OnGridBuilt += UpdateBorder;
    }

    void OnDisable()
    {
        if (gameGrid != null)
            gameGrid.OnGridBuilt -= UpdateBorder;
    }

    private void UpdateBorder()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float newWidth = gameGrid.Width + WIDTH_PADDING;
        float newHeight = gameGrid.Height + HEIGHT_PADDING;
        sr.size = new Vector2(newWidth, newHeight);
    }

}
