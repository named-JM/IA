using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMapInfoData", menuName = "Map Info/Create New Map Info")]
public class MapInfoData : ScriptableObject
{
    public string buttonName;
    public List<MapInfoEntry> entries;
}

[System.Serializable]
public class MapInfoEntry
{
    public string placeName;
    public Sprite image;
}
