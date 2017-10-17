using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class Shuijing : Building
{
    public int brushType;
    public Transform vPointRoot;
    public GameObject selectTag;
    
    [HideInInspector]
    public int level;
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

    PATileTerrain.PATile GetTileByPoint(PATileTerrain tileTerrain, Transform vPointTrans)
    {
        float minDistance = float.PositiveInfinity;
        PATileTerrain.PATile pointTile = null;
        foreach(var affectTile in affectTiles)
        {
            float distance = Vector3.Distance(affectTile.WorldPos(tileTerrain.transform),vPointTrans.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                pointTile = affectTile;
            }
        }

        if (minDistance < 2f)
            return pointTile;
        else
            return null;
    }

    public void CreateBuildings(PATileTerrain tileTerrain)
    {
        //return;
        RandomManager.instance.SetSeed(tileTerrain.settings.GetCrystalBuilding(tile.id).randomSeed);
        PATileTerrainChunk chunk = tileTerrain.GetChunk(tile.chunkId);

        foreach (var point in vPoints)
        {
            PATileTerrain.PATile pointTile = GetTileByPoint(tileTerrain,point.transform);
            if (pointTile == null)
                continue;
            Debug.Log("pointTile.x " + pointTile.x + " pointTile.y " + pointTile.y + " point " + point.transform);
            if (point.virtualPointType != VirtualPoint.VirtualPointType.Building)
                continue;
            if (!point.CheckElementType(pointTile))
                continue;
            buildings.Add(point.CreateBuilding(chunk.settings.decoratesRoot));
        }

        if(Application.isPlaying)
            LocalNavMeshBuilder.instance.UpdateNavMesh();

        foreach (var point in vPoints)
        {
            if (point.virtualPointType != VirtualPoint.VirtualPointType.Animals)
                continue;
            buildings.Add(point.CreateBuilding(chunk.settings.decoratesRoot));
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

    public void SetSelectTag(bool isSelect)
    {
        selectTag.SetActive(isSelect);
    }
}