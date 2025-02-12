using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public bool clickable;
    public bool fallable;
    public bool blastsWithExplosion;
    public int health;

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

    public void InitializeFromProperties(ItemConfig config)
    {
        itemType = config.ItemType;
        clickable = config.Clickable;
        fallable = config.Fallable;
        blastsWithExplosion = config.BlastsWithExlosion;
        health = config.Health;
    }
}
