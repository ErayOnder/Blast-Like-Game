using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public bool clickable;
    public bool fallable;
    public bool blastsWithExplosion;
    public int health;

    private const int BaseSortingOrder = 10;
    private static int childSpriteOrder;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Cell cell;

    public Cell Cell
    {
        get => cell;
        set
        {
            if (cell == value)
                return;

            if (cell != null && cell.Item == this)
            {
                cell.Item = null;
            }

            cell = value;

            if (cell != null)
            {
                cell.Item = this;
                gameObject.name = cell.gameObject.name + " " + GetType().Name;
            }
        }
    }

    public void InitializeFromProperties(ItemConfig config, Sprite sprite)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not found on " + gameObject.name);
                return;
            }
        }

        ApplySpriteRendererProperties(spriteRenderer, sprite);

        itemType = config.ItemType;
        clickable = config.Clickable;
        fallable = config.Fallable;
        blastsWithExplosion = config.BlastsWithExlosion;
        health = config.Health;
    }

    private void ApplySpriteRendererProperties(SpriteRenderer sr, Sprite sprite)
    {
        sr.sprite = sprite;
        sr.transform.localPosition = Vector3.zero;
        sr.transform.localScale = new Vector2(0.7f, 0.7f);
        sr.sortingLayerID = SortingLayer.NameToID("Cell");
        sr.sortingOrder = BaseSortingOrder + childSpriteOrder++;
    }

}
