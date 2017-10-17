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

    public Transform CreateBuilding(Transform parent)
    {
        int randomValue = RandomManager.instance.Range(0,buildings.Count);
        Transform building = null;
        if (Application.isPlaying)
            building = PoolManager.Pools["Shuijing"].Spawn(buildings[randomValue].trans);
        else
            building = Object.Instantiate(buildings[randomValue].trans);
        building.SetParent(parent,false);
        building.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (buildings[randomValue].rotateY.Equals(0f))
            building.rotation = transform.rotation;
        else
            building.localRotation = Quaternion.Euler(0, buildings[randomValue].rotateY, 0);
        
        return building;
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
