using UnityEngine;

public class CubeItem : Item
{
    public void InitializeConfig(ItemBase baseItem, ItemConfig config, MatchType matchType)
    {
        transform.position = baseItem.transform.position;
        Debug.Log("CubeItem initialized with match type: " + matchType);
    }

}
