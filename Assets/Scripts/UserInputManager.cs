using UnityEngine;
using UnityEngine.EventSystems;

public class UserInputManager : MonoBehaviour
{
    public Camera Cam;
    private GridCell _selectedCell = null;
    private BaseCrop _selectedCrop = null;
    private bool _isPlacingCrop = false;
    void Update()
    {
        if (IsPointerOverUI()) return;

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Vector3 inputPosition = (Input.touchCount > 0) ? Input.GetTouch(0).position : Input.mousePosition;

        Ray ray = Cam.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCell cell = hit.collider.GetComponent<GridCell>();
            HandleCellClick(cell);
        }
        else
        {
            DeselectCell();
        }
    }

    private void HandleCellClick(GridCell cell)
    {
        if (_isPlacingCrop)
        {
            TryPlaceCrop(cell);
        }
        else
        {
            HandleCellSelection(cell);
        }
    }

    private void TryPlaceCrop(GridCell cell)
    {
        if (cell != null && !cell.IsOccupied)
        {
            cell.PlantCrop(_selectedCrop);
            _selectedCrop = null;
            _isPlacingCrop = false;
        }
    }

    private void HandleCellSelection(GridCell cell)
    {
        if (cell == null)
        {
            DeselectCell();
            return;
        }

        if (_selectedCell != null && _selectedCell != cell)
        {
            _selectedCell.Outline(false);
        }

        _selectedCell = cell;
        _selectedCell.Outline(true);

        if (_selectedCell.IsOccupied)
        {
            if (_selectedCell.IsCropFullyGrown())
            {
                UIManager.Instance.ShowHarvestPanel(_selectedCell.transform.position); 
            }
            else
            {
                UIManager.Instance.ShowCropTimerPanel(_selectedCell.transform.position, _selectedCell.GetCrop());
            }
        }
        else
        {
            UIManager.Instance.ShowCropSelectionPanel(_selectedCell.transform.position);
        }
    }

    private void DeselectCell()
    {
        if (_selectedCell != null)
        {
            _selectedCell.Outline(false);
            _selectedCell = null;
        }
        UIManager.Instance.HideCropSelectionPanel();
        UIManager.Instance.HideCropTimerPanel();
        UIManager.Instance.HideHarvestPanel();
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;

        if (Input.touchCount > 0)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.GetTouch(0).position
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        return false;
    }
}
