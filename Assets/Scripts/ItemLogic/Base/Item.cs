using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public bool clickable;
    public bool fallable;
    public bool blastsWithExplosion;
    public int health;

    public FallAnimation fallAnimation;
    
    private const int BaseSortingOrder = 10;
    private static int childSpriteOrder;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Cell cell;

    public Cell Cell
    {
        get { return cell; }
        set
        {
            if (cell == value) return;

            var oldCell = cell;
            cell = value;

            if (oldCell != null && oldCell.Item == this)
                oldCell.Item = null;
    
            if (value != null)
            {
                value.Item = this;
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
        
        // Only set up fallAnimation if the item is fallable.
        if (fallable)
        {
            if (fallAnimation == null)
            {
                fallAnimation = GetComponent<FallAnimation>();
                if (fallAnimation == null)
                {
                    Debug.LogWarning("FallAnimation component not found on " + gameObject.name + ". Adding one.");
                    fallAnimation = gameObject.AddComponent<FallAnimation>();
                }
            }
            fallAnimation.item = this;
        }
    }
    
    public void Fall()
    {
        if (!fallable) return;
        fallAnimation.FallTo(cell.GetFallTarget());
    }
    
    public virtual void TryExecute()
    {
        Debug.Log("Destroying item: " + gameObject.name);
        
        // TODO: Implement particle effects, sounds, etc.

        if (Cell != null)
        {
            Cell.Item = null;
        }
        
        Destroy(gameObject);
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
