using UnityEngine;
using DG.Tweening;

// Class role: Animates an item's fall to a target cell.
public class FallAnimation : MonoBehaviour
{
    public Item item;

    [SerializeField] private float animationDuration = 0.35f;


    private Cell targetCell;
    private Vector3 targetPosition;

    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);
    }

    public void FallTo(Cell newTargetCell)
    {
        if (newTargetCell == null || IsInvalidTargetCell(newTargetCell))
        {
            Debug.Log("FallAnimation: Invalid target cell for " + (item != null ? item.gameObject.name : "null"));
            return;
        }
        SetTargetCell(newTargetCell);
        AnimateFall();
    }

    private bool IsInvalidTargetCell(Cell newTargetCell)
    {
        return targetCell != null && newTargetCell.Y >= targetCell.Y;
    }

    private void SetTargetCell(Cell newTargetCell)
    {
        targetCell = newTargetCell;
        if (item != null)
        {
            item.Cell = targetCell;
        }
        targetPosition = targetCell.transform.position;
    }

    // Handles the fall animation tween.
    private void AnimateFall()
    {
        if (item == null || targetCell == null)
            return;

        item.transform.DOMoveY(targetPosition.y, animationDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() => {
                targetCell = null;
            });
    }
}