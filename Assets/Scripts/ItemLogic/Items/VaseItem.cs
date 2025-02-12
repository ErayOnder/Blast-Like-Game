using UnityEngine;

public class VaseItem : Item
{
    public void InitializeConfig(ItemBase baseItem, ItemConfig config)
    {
        transform.position = baseItem.transform.position;
        Debug.Log("VaseItem initialized");
    }
}