using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class Shuijing : MonoBehaviour
{
    public int brushType;
    public Transform vPointRoot;

    [HideInInspector]
    public PATileTerrain.PATile tile = null;
    [HideInInspector]
    public int level;
    [HideInInspector]
    public List<Transform> buildings = new List<Transform>();
    [HideInInspector]
    public List<VirtualPoint> vPoints = new List<VirtualPoint>();

    void Awake()
    {
        vPoints.Clear();
        if (vPointRoot == null)
            return;

        for (int i = 0; i < vPointRoot.childCount; i ++)
        {
            Transform vPointTrans = vPointRoot.GetChild(i);
            vPoints.Add(vPointTrans.GetComponent<VirtualPoint>());
        }
    }

    public void CreateBuildings(PATileTerrain tileTerrain)
    {
        PATileTerrainChunk chunk = tileTerrain.GetChunk(tile.chunkId);

        foreach(var point in vPoints)
            buildings.Add(point.CreateBuilding(chunk.settings.buildingsRoot));
    }

    public void RemoveBuildings()
    {
        foreach(var building in buildings)
            PoolManager.Pools["Shuijing"].Despawn(building);
        buildings.Clear();
    }
}