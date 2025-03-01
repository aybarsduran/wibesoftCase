using UnityEngine;

public class UserInputManager : MonoBehaviour
{
    public Camera Cam;
    private GridCell _selectedCell = null; 
    private BaseCrop _selectedCrop = null;
    private bool _isPlacingCrop = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
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
                    else if (_selectedCell != null) 
                    {
                        _selectedCell.Outline(false);
                        _selectedCell = null;
                    }
                }
            }
        }
    }
}
