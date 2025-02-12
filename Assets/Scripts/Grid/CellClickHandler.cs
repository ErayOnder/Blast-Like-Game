using UnityEngine;
using UnityEngine.EventSystems;

public class CellClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Cell cell;

    private void Awake()
    {
        cell = GetComponent<Cell>();
        if (cell == null)
        {
            Debug.LogWarning("Cell component not found on " + gameObject.name);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cell != null)
        {
            cell.CellTapped();
        }
    }

}
