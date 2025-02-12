using UnityEngine;

public class BoxItem : Item
{
    public void InitializeConfig(ItemBase baseItem, ItemConfig config)
    {
        transform.position = baseItem.transform.position;
        Debug.Log("BoxItem initialized");
    }
}