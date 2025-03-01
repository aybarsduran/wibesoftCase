using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Building System/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public GameObject prefab;
    public Vector2Int size;
}
