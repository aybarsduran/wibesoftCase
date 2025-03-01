using UnityEngine;
using System.Collections;

public abstract class BaseCrop : MonoBehaviour
{
    public string CropName;
    public Mesh[] GrowthMeshes; 
    public float GrowthTime;

    private int _currentStage = 0;
    private MeshFilter _meshFilter;

    private float _growthStartTime;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _growthStartTime = Time.time;
        StartCoroutine(GrowCrop());
    }

    IEnumerator GrowCrop()
    {
        while (_currentStage < GrowthMeshes.Length - 1)
        {
            yield return new WaitForSeconds(GrowthTime / (GrowthMeshes.Length - 1));

            _currentStage++;
            _meshFilter.mesh = GrowthMeshes[_currentStage]; 
        }
    }

    public bool IsFullyGrown()
    {
        return _currentStage >= GrowthMeshes.Length - 1;
    }
    public float GetRemainingTime()
    {
        if (IsFullyGrown()) return 0;

        float elapsed = Time.time - _growthStartTime;
        float totalTimePerStage = GrowthTime / (GrowthMeshes.Length - 1);
        float remainingTime = totalTimePerStage * ((GrowthMeshes.Length - 1) - _currentStage) - elapsed % totalTimePerStage;

        return Mathf.Max(remainingTime, 0);
    }
    public float GetGrowthEndTime()
    {
        return _growthStartTime + GrowthTime;
    }

}
