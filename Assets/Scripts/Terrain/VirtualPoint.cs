using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

[System.Serializable]
public class BuildingData
{
    public Transform trans;
    public float rotateY;
}

public class VirtualPoint : MonoBehaviour 
{
    public enum VirtualPointType
    {
        Building,
        Animals,
    }

    public enum ElementType
    {
        Fire,
        Wood,
        Sand,
    }

    //占地面积类型
    public enum AreaType
    {
        Area1X1,
        Area2X2,
        Area3X3,
    }

    public List<BuildingData> buildings = new List<BuildingData>();
    public VirtualPointType virtualPointType;
    public ElementType elementType;
    public AreaType areaType;

    [HideInInspector]
    public Transform building;
    [HideInInspector]
    public PATileTerrain.PATile closeTile; //距离最近的tile

    void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        building = null;
        closeTile = null;
    }

    public Transform CreateBuilding(Transform parent)
    {
        if (buildings.Count == 0)
        {
            building = null;
            return null;
        }
            
        int randomValue = RandomManager.instance.Range(0,buildings.Count);
        Transform newBuilding = null;
        if (buildings[randomValue].trans == null)
            return null;
        if (Application.isPlaying)
            newBuilding = PoolManager.Pools["Shuijing"].Spawn(buildings[randomValue].trans);
        else
            newBuilding = Object.Instantiate(buildings[randomValue].trans);
        newBuilding.SetParent(parent,false);
        newBuilding.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (buildings[randomValue].rotateY.Equals(0f))
            newBuilding.rotation = transform.rotation;
        else
            newBuilding.localRotation = Quaternion.Euler(0, buildings[randomValue].rotateY, 0);

        building = newBuilding;

        return newBuilding;
    }

    public void RemoveBuilding()
    {
        if (building == null)
            return;
        PoolManager.Pools["Shuijing"].Despawn(building);
        building = null;
    }

    public bool CheckElementType(PATileTerrain.PATile checkTile)
    {
        if (checkTile == null)
            return false;

        if (elementType == ElementType.Sand && checkTile.element.IsMultiElement())
            return true;
        else if (elementType == ElementType.Fire && checkTile.element.IsFire())
            return true;
        else if (elementType == ElementType.Wood && checkTile.element.IsWood())
            return true;
        return false;
    }

    private bool CheckCloseTileElementType()
    {
        return CheckElementType(closeTile);
    }

    public bool CheckAreaType(PATileTerrain tileTerrain)
    {
        if (closeTile == null)
            return false;
        if (!CheckCloseTileElementType())
            return false;
        if (areaType == AreaType.Area1X1)
            return true;

        PATileTerrain.PATile[] nTiles = tileTerrain.GetNeighboringTilesNxN(closeTile, 1);
        if(areaType == AreaType.Area2X2)
        {
            for (int i = 5; i < nTiles.Length; i++)
            {
                if (!CheckElementType(nTiles[i]))
                    return false;
            }
        }
        else if(areaType == AreaType.Area3X3)
        {
            for (int i = 0; i < nTiles.Length; i++ )
            {
                if (!CheckElementType(nTiles[i])) 
                    return false;
            }
        }

        return true;
    }
}
