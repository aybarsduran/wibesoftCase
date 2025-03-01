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
            Vector3 inputPosition;

            if (Input.touchCount > 0)
                inputPosition = Input.GetTouch(0).position; 
            else
                inputPosition = Input.mousePosition;

            Ray ray = Cam.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GridCell cell = hit.collider.GetComponent<GridCell>();

                if (_isPlacingCrop) 
                {
                    if (cell != null && !cell.IsOccupied)
                    {
                        cell.PlantCrop(_selectedCrop);
                        _selectedCrop = null;
                        _isPlacingCrop = false;
                    }
                }
                else 
                {
                    if (cell != null)
                    {
                        if (_selectedCell != null && _selectedCell != cell)
                        {
                            _selectedCell.Outline(false);
                        }

                        _selectedCell = cell;
                        _selectedCell.Outline(true);

                        UIManager.Instance.ShowCropSelectionPanel(_selectedCell.transform.position);
                    }
                    else 
                    {
                        if (_selectedCell != null)
                        {
                            _selectedCell.Outline(false);
                            _selectedCell = null;
                        }
                        UIManager.Instance.HideCropSelectionPanel();
                    }
                }
            }
            else 
            {
                if (_selectedCell != null)
                {
                    _selectedCell.Outline(false);
                    _selectedCell = null;
                }
                UIManager.Instance.HideCropSelectionPanel();
            }
        }
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
