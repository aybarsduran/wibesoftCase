using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private BuildingGridCell _cell;

    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 5, Vector3.down, out hit, 10f))
        {
            _cell = hit.collider.GetComponent<BuildingGridCell>();
            if (_cell != null)
            {
                _cell.SetCellState(true); 
            }
        }
    }

    private void OnDestroy()
    {
        if (_cell != null)
        {
            _cell.SetCellState(false); 
        }
    }
}
