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
        _currentCrop.transform.SetParent(transform);
        _currentCrop.transform.position = transform.position + new Vector3(0,2.2f,0);
        IsOccupied = true;
    }

    public bool IsCropFullyGrown()
    {
        if(_currentCrop == null)
        {
            return false;
        }
        return _currentCrop.IsFullyGrown();
    }

    public float GetTimeUntilFullyGrown()
    {
        if (_currentCrop != null && !_currentCrop.IsFullyGrown())
        {
            return _currentCrop.GetRemainingTime();
        }
        return 0;
    }
    public BaseCrop GetCrop()
    {
        return _currentCrop;
    }
    public void HarvestCrop()
    {
        if (_currentCrop != null && IsCropFullyGrown())
        {
            _currentCrop.Harvest();
            _currentCrop = null;
            IsOccupied = false;
        }
    }
}
