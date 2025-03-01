using UnityEngine;

public class Carrot : BaseCrop
{
    private void Awake()
    {
        GrowthTime = 30f;
        CropName = "Carrot";
    }
}
