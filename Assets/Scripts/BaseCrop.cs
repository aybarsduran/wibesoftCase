using UnityEngine;
using System.Collections;

public abstract class BaseCrop : MonoBehaviour
{
    public GameObject[] GrowthStages; 
    public float GrowthTime; 

    private int _currentStage = 0;

    private void Start()
    {
        StartCoroutine(GrowCrop());
    }

    IEnumerator GrowCrop()
    {
        while (_currentStage < GrowthStages.Length - 1)
        {
            yield return new WaitForSeconds(GrowthTime / (GrowthStages.Length - 1));

            if (GrowthStages[_currentStage] != null)
                GrowthStages[_currentStage].SetActive(false);

            _currentStage++;

            if (GrowthStages[_currentStage] != null)
                GrowthStages[_currentStage].SetActive(true);
        }
    }
}
