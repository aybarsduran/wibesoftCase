using UnityEngine;
using System.Collections;
using DG.Tweening;

public abstract class BaseCrop : MonoBehaviour
{
    public string CropName;
    public Mesh[] GrowthMeshes; 
    public float GrowthTime;
    public GameObject HarvestEffectPrefab;

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

            transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _meshFilter.mesh = GrowthMeshes[_currentStage];

                transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            });
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
    public void Harvest()
    {
        if (HarvestEffectPrefab != null)
        {
            GameObject effect = Instantiate(HarvestEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.zero;

            effect.transform.DOScale(Vector3.one* 1.1f, 0.3f).SetEase(Ease.OutBack);
            effect.transform.DOJump(transform.position+ new Vector3(0,0.3f,0), 2.5f, 1, 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(effect, 0.2f)); 
        }

        Destroy(gameObject); 
    }

}
