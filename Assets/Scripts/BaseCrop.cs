using UnityEngine;
using System.Collections;

public abstract class BaseCrop : MonoBehaviour
{
    public Mesh[] GrowthMeshes; 
    public float GrowthTime;

    private int _currentStage = 0;
    private MeshFilter _meshFilter;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
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
}
