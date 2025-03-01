using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CropDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public BaseCrop cropPrefab; 
    private Image dragImage;
    private Transform parentCanvas;

    private void Start()
    {
        parentCanvas = UIManager.Instance.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragImage = new GameObject("DragImage", typeof(Image)).GetComponent<Image>();
        dragImage.transform.SetParent(parentCanvas, false);
        dragImage.sprite = GetComponent<Image>().sprite;
        dragImage.raycastTarget = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragImage != null)
        {
            dragImage.transform.position = eventData.position; 
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCell cell = hit.collider.GetComponent<GridCell>();
            if (cell != null && !cell.IsOccupied)
            {
                PlaceCrop(cell);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragImage != null)
        {
            Destroy(dragImage.gameObject);
        }

       
    }

    private void PlaceCrop(GridCell cell)
    {
        cell.PlantCrop(cropPrefab);
    }
}
