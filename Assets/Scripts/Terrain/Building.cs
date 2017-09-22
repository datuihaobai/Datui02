using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour 
{
    public enum BuildingType
    {
        None = 0,
        Shuijing = 1,
        Nest = 2,
    }
    [HideInInspector]
    public PATileTerrain.PATile tile = null;
    [HideInInspector]
    public PATileTerrain.TileElementType elementType;
    [HideInInspector]
    public string prefabName;
}