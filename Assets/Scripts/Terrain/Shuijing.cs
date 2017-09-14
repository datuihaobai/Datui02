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
    public PATileTerrain.TileElementType elementType;
    [HideInInspector]
    public List<Transform> buildings = new List<Transform>();
    [HideInInspector]
    public List<VirtualPoint> vPoints = new List<VirtualPoint>();
    [HideInInspector]
    public List<PATileTerrain.PATile> affectTiles = new List<PATileTerrain.PATile>();//水晶影响的tile列表，用来绘制贴花

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
        //return;
        RandomManager.instance.SetSeed(tileTerrain.settings.GetCrystal(tile.id).randomSeed);
        PATileTerrainChunk chunk = tileTerrain.GetChunk(tile.chunkId);

        foreach (var point in vPoints)
        {
            if (point.virtualPointType != VirtualPoint.VirtualPointType.Building)
                continue;
            buildings.Add(point.CreateBuilding(chunk.settings.buildingsRoot));
        }

        LocalNavMeshBuilder.instance.UpdateNavMesh();

        foreach (var point in vPoints)
        {
            if (point.virtualPointType != VirtualPoint.VirtualPointType.Animals)
                continue;
            buildings.Add(point.CreateBuilding(chunk.settings.buildingsRoot));
        }
    }

    public void RemoveBuildings()
    {
        if (buildings.Count == 0)
            return;
        //buildings[0].parent.gameObject.SetActive(true);
        foreach(var building in buildings)
            PoolManager.Pools["Shuijing"].Despawn(building);
        buildings.Clear();
    }
}