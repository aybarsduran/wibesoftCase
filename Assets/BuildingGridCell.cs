using UnityEngine;

public class BuildingGridCell : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    

    public void SetCellState(bool occupied)
    {
        IsOccupied = occupied;
    }

    public bool CanPlaceBuilding()
    {
        return !IsOccupied;
    }

    public void PlaceBuilding(GameObject building)
    {
        if (!CanPlaceBuilding()) return;

        Instantiate(building, transform.position, Quaternion.identity);
        SetCellState(true);
    }

    public void RemoveBuilding()
    {
        SetCellState(false);
    }
}
