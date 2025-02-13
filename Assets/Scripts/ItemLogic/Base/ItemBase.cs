using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public ItemType ItemType;
    public bool Clickable = true;
    public bool Fallable = true;
    public bool BlastsWithExlosion = false;
    public int Health = 1;
    public FallAnimation FallAnimation;
}