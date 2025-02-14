using UnityEngine;

// Item: Base class for game items. Handles initialization, sprite setup, falling, and destruction.
public class Item : MonoBehaviour
{
    public ItemType itemType;
    public bool clickable;
    public bool fallable;
    public bool blastsWithExplosion;
    public int health;

    public FallAnimation fallAnimation;
    public RocketAnimation rocketAnimation;
    
    private const int BaseSortingOrder = 10;
    private static int childSpriteOrder;
    [SerializeField] protected SpriteRenderer spriteRenderer;
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

    // Init: Sets up this item from its config; adds animations if necessary.
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
        if (itemType == ItemType.Rocket)
        {
            if (rocketAnimation == null)
            {
                rocketAnimation = GetComponent<RocketAnimation>();
                if (rocketAnimation == null)
                {
                    Debug.LogWarning("RocketAnimation component not found on " + gameObject.name + ". Adding one.");
                    rocketAnimation = gameObject.AddComponent<RocketAnimation>();
                }
            }
            rocketAnimation.item = this;
        }
    }

    public void UpdateSpriteForBonus()
    {
        var spriteConfig = ItemFactory.Instance.spriteConfig;
        if (spriteConfig != null)
        {
            Sprite bonusSprite = spriteConfig.GetBonusSpriteForItemType(itemType);
            if (bonusSprite != null)
            {
                ApplySpriteRendererProperties(spriteRenderer, bonusSprite);
            }
            else
            {
                Debug.LogWarning("No bonus sprite found for item type " + itemType);
            }
        }
        else
        {
            Debug.LogWarning("ItemSpriteConfig not found.");
        }
    }
    
    public void Fall()
    {
        if (!fallable) return;
        fallAnimation.FallTo(cell.GetFallTarget());
    }
    
    public virtual void TryExecute(DamageSource source)
    {
        ParticleManager.Instance.PlayParticle(this);
        if (Cell != null)
        {
            Cell.Item = null;
        }
        Destroy(gameObject);
    }
    
    public void ApplySpriteRendererProperties(SpriteRenderer sr, Sprite sprite)
    {
        sr.sprite = sprite;
        if (sr.gameObject != gameObject)
        {
            sr.transform.localPosition = Vector3.zero;
        }
        sr.transform.localScale = new Vector2(0.7f, 0.7f);
        sr.sortingLayerID = SortingLayer.NameToID("Cell");
        sr.sortingOrder = BaseSortingOrder + childSpriteOrder++;
    }
}
