using UnityEngine;

public class StoneItem : Item
{
    public void InitializeConfig(ItemBase baseItem, ItemConfig config)
    {
        transform.position = baseItem.transform.position;
        Debug.Log("StoneItem initialized");
    }
}