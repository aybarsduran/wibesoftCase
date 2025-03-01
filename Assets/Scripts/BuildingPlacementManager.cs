using UnityEngine;
using System.Collections.Generic;

public class BuildingPlacementManager : MonoBehaviour
{
    public static BuildingPlacementManager Instance { get; private set; }

    private BuildingData _selectedBuilding; 
    private GameObject _placementIndicator; 
    private Camera _camera;
    private bool _isPlacing = false;
    private bool _isMovingExistingBuilding = false;
    private List<BuildingGridCell> selectedCells = new List<BuildingGridCell>();
    private GameObject _selectedExistingBuilding;
    private float _holdStartTime;
    private bool _isHolding;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        HandleInput();

        if (_isHolding && Time.time - _holdStartTime >= 2f)
        {
            StartMovingExistingBuilding();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            HandleTouchStart(Input.mousePosition);
        }

        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchStart(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && (_isPlacing || _isMovingExistingBuilding))
            {
                UpdatePlacementIndicator(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _isHolding = false; 
                if (_isPlacing) PlaceBuilding();
                else if (_isMovingExistingBuilding) PlaceExistingBuilding();
            }
        }

        if (_isPlacing || _isMovingExistingBuilding)
        {
            UpdatePlacementIndicator(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            CancelPlacement();
        }
    }

    public void StartPlacingBuilding(BuildingData building)
    {
        _selectedBuilding = building;
        _isPlacing = true;
        _isMovingExistingBuilding = false;

        if (_placementIndicator != null)
            Destroy(_placementIndicator);

        _placementIndicator = Instantiate(building.prefab);
        _placementIndicator.transform.localScale = new Vector3(1, 1, 1);

        SetIndicatorTransparency(_placementIndicator, 0.5f);
    }
    private void SetIndicatorTransparency(GameObject obj, float alpha)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
                mat.SetFloat("_Mode", 3); 
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }

   
    private void SetIndicatorColor(GameObject obj, Color color)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.color = color;
            }
        }
    }


    private bool CanPlaceBuilding()
    {
        if (selectedCells.Count < _selectedBuilding.size.x * _selectedBuilding.size.y)
        {
            return false;
        }

        foreach (var cell in selectedCells)
        {
            if (cell.IsOccupied) return false;
        }

        return true;
    }

    private void PlaceBuilding()
    {
        if (!CanPlaceBuilding()) return;

        Vector3 snappedPosition = _placementIndicator.transform.position;
        GameObject newBuilding = Instantiate(_selectedBuilding.prefab, snappedPosition, Quaternion.identity);

        foreach (var cell in selectedCells)
        {
            cell.SetCellState(true);
        }

        Destroy(_placementIndicator);

        _isPlacing = false;
    }




    private void CancelPlacement()
    {
        _isPlacing = false;
        if (_placementIndicator != null)
        {
            Destroy(_placementIndicator);
            _placementIndicator = null;
        }
    }

    private List<BuildingGridCell> GetCellsForBuilding(Vector3 position, Vector2Int size)
    {
        List<BuildingGridCell> cells = new List<BuildingGridCell>();

        Collider[] colliders = Physics.OverlapBox(position, new Vector3(size.x-1, 0.1f, size.y-1));

        foreach (var col in colliders)
        {
            BuildingGridCell cell = col.GetComponent<BuildingGridCell>();
            if (cell != null) cells.Add(cell);
        }

        return cells;
    }
    private Vector3 RoundToGrid(Vector3 position, Vector2Int size)
    {
        float cellSize = 1f; 

        float x = Mathf.Round(position.x / cellSize) * cellSize;
        float z = Mathf.Round(position.z / cellSize) * cellSize;

        x -= (size.x % 2 == 0) ? cellSize / 2f : 0;
        z -= (size.y % 2 == 0) ? cellSize / 2f : 0;

        return new Vector3(x, position.y, z);
    }
  
    private void UpdatePlacementIndicator(Vector3 touchPosition)
    {
        Ray ray = _camera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BuildingGridCell cell = hit.collider.GetComponent<BuildingGridCell>();
            if (cell != null)
            {
                Vector3 snappedPosition = RoundToGrid(cell.transform.position + new Vector3(0f, 2f, 0f), _selectedBuilding.size);
                _placementIndicator.transform.position = snappedPosition;

                selectedCells = GetCellsForBuilding(snappedPosition, _selectedBuilding.size);
                _placementIndicator.SetActive(true);

                Color indicatorColor = CanPlaceBuilding() ? Color.green : Color.red;
                SetIndicatorColor(_placementIndicator, indicatorColor);
            }
        }
    }

    private void HandleTouchStart(Vector3 touchPosition)
    {
        Ray ray = _camera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Building"))
            {
                _selectedExistingBuilding = hit.collider.gameObject;
                _isHolding = true;
                _holdStartTime = Time.time; 
            }
            else if (_isPlacing)
            {
                UpdatePlacementIndicator(touchPosition);
            }
        }
    }

    private void PlaceExistingBuilding()
    {
        if (!CanPlaceBuilding()) return;

        Vector3 snappedPosition = _placementIndicator.transform.position;
        _selectedExistingBuilding.SetActive(true);
        _selectedExistingBuilding.transform.position = snappedPosition;

        Destroy(_placementIndicator);
        _isMovingExistingBuilding = false;
    }

    public bool IsPlacingOrMovingBuilding()
    {
        return _isPlacing || _isMovingExistingBuilding;
    }
    private void StartMovingExistingBuilding()
    {
        _isHolding = false;
        _isMovingExistingBuilding = true;
        _isPlacing = false;

        if (_placementIndicator != null)
        {
            Destroy(_placementIndicator);
        }

        _selectedExistingBuilding.SetActive(false);
        _placementIndicator = Instantiate(_selectedExistingBuilding);
        SetIndicatorTransparency(_placementIndicator, 0.5f);
    }


}
