using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Config", fileName = "ItemConfig")]
public class ItemConfig : ScriptableObject
{
    public ItemType ItemType;
    public bool Clickable = true;
    public bool Fallable = true;
    public bool BlastsWithExlosion = false;
    public int Health = 1;
}
