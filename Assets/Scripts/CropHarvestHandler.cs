using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CropHarvestHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image _dragImage;
    private Transform _parentCanvas;

    private void Start()
    {
        _parentCanvas = UIManager.Instance.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragImage = new GameObject("DragImage", typeof(Image)).GetComponent<Image>();
        _dragImage.transform.SetParent(_parentCanvas, false);
        _dragImage.sprite = GetComponent<Image>().sprite;
        _dragImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragImage != null)
        {
            _dragImage.transform.position = eventData.position;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCell cell = hit.collider.GetComponent<GridCell>();
            if (cell != null && cell.IsCropFullyGrown())
            {
                HarvestCrop(cell);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragImage != null)
        {
            Destroy(_dragImage.gameObject);
        }
    }

    private void HarvestCrop(GridCell cell)
    {
        cell.HarvestCrop();
    }
}
