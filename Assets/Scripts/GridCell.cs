using UnityEngine;

public class GridCell : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false; 
    private Outline _outline;
    private BaseCrop _currentCrop;
    private void Start()
    {
        _outline = GetComponent<Outline>();
        Outline(false);
    }

   
    public void Outline(bool state)
    {
        if (_outline != null)
        {
           _outline.enabled = state;
        }
    }
    public void PlantCrop(BaseCrop cropPrefab)
    {
        if (IsOccupied) return;

        _currentCrop = Instantiate(cropPrefab, transform.position, Quaternion.identity);
        IsOccupied = true;
    }
}
