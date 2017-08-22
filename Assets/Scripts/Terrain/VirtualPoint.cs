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
    public List<BuildingData> buildings = new List<BuildingData>();
	
    public Transform CreateBuilding(Transform chunk)
    {
        int randomValue = RandomManager.instance.Range(0,buildings.Count);
        Transform building = null;
        if (Application.isPlaying)
            building = PoolManager.Pools["Shuijing"].Spawn(buildings[randomValue].trans);
        else
            building = Object.Instantiate(buildings[randomValue].trans);
        building.SetParent(chunk,false);
        building.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        building.localRotation = Quaternion.Euler(0, buildings[randomValue].rotateY, 0);
        return building;
    }
}
