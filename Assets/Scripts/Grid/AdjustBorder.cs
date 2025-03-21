using UnityEngine;

// Class role: Adjusts the grid border sprite based on the current grid dimensions.
public class AdjustBorder : MonoBehaviour
{
    [SerializeField] private GameGrid gameGrid;
    private const float WIDTH_PADDING = 0.35f;
    private const float HEIGHT_PADDING = 0.45f;

    void Awake()
    {
        GameEvents.OnBoardUpdated += UpdateBorder;
        if (gameGrid != null && gameGrid.Grid != null)
        {
            UpdateBorder();
        }
    }

    void OnDestroy()
    {
        GameEvents.OnBoardUpdated -= UpdateBorder;
    }

    // Adjusts the sprite size using grid width, height, and padding.
    private void UpdateBorder()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && gameGrid != null)
        {
            float newWidth = gameGrid.Width + WIDTH_PADDING;
            float newHeight = gameGrid.Height + HEIGHT_PADDING;
            sr.size = new Vector2(newWidth, newHeight);
        }
    }
}
