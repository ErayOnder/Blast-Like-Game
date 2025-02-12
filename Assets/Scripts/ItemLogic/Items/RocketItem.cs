using UnityEngine;

public class RocketItem : Item
{
    public void InitializeConfig(ItemBase baseItem, ItemConfig config)
    {
        transform.position = baseItem.transform.position;
        Debug.Log("RocketItem initialized");
    }
}