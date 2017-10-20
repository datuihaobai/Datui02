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

    public List<BuildingData> buildings = new List<BuildingData>();
    public VirtualPointType virtualPointType;
    public ElementType elementType;

    public Transform building;
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
        else if (elementType == ElementType.Fire && checkTile.element.FireValue > 0 && checkTile.element.WoodValue == 0)
            return true;
        else if (elementType == ElementType.Wood && checkTile.element.FireValue == 0 && checkTile.element.WoodValue > 0)
            return true;
        return false;
    }
}
