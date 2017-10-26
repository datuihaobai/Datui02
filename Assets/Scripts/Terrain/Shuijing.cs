using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class Shuijing : Building
{
    //public int brushType;
    public Transform vPointRoot;
    public GameObject selectTag;
    
    [HideInInspector]
    public int level;
    [HideInInspector]
    public List<Transform> buildings = new List<Transform>();
    [HideInInspector]
    public List<VirtualPoint> vPoints = new List<VirtualPoint>();
    [HideInInspector]
    public Dictionary<int,PATileTerrain.PATile> affectTiles = new Dictionary<int,PATileTerrain.PATile>();//水晶影响的tile列表，用来绘制贴花

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
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(vPointTrans.position.x,vPointTrans.position.y + 100 , vPointTrans.position.z),Vector3.down);
        Physics.Raycast(ray, out hit, Mathf.Infinity, TerrainManager.instance.terrainChunkLayermask);

        PATileTerrain tt = tileTerrain.IsTerrain(hit.transform);
        if (tt == null)
            return null;

        Vector3 pos = tileTerrain.transform.InverseTransformPoint(hit.point);
        int x = (int)Mathf.Abs(pos.x / tileTerrain.tileSize);
        int y = (int)Mathf.Abs(pos.z / tileTerrain.tileSize);
        PATileTerrain.PATile tile = tileTerrain.GetTile(x, y);

        return tile;
    }

    public void CreateBuildings(PATileTerrain tileTerrain)
    {
        //return;
        RandomManager.instance.SetSeed(tileTerrain.settings.GetCrystalBuilding(tile.id).randomSeed);
        PATileTerrainChunk chunk = tileTerrain.GetChunk(tile.chunkId);

        foreach (var point in vPoints)
        {
            if (point.closeTile == null)
                point.closeTile = GetTileByPoint(tileTerrain, point.transform);
            if (point.closeTile == null)
                continue;
            //Debug.Log("pointTile.x " + pointTile.x + " pointTile.y " + pointTile.y + " point " + point.transform);
            if (point.virtualPointType != VirtualPoint.VirtualPointType.Building)
                continue;
            if (!point.CheckAreaType(tileTerrain))
            {
                if(point.building != null)
                {
                    buildings.Remove(point.building);
                    point.RemoveBuilding();
                }
            } 
            else
            {
                if (point.building == null)
                {
                    Transform building = point.CreateBuilding(chunk.settings.decoratesRoot);
                    if (building != null)
                        buildings.Add(building);
                }
            }
        }

        if(Application.isPlaying)
            LocalNavMeshBuilder.instance.UpdateNavMesh();

        foreach (var point in vPoints)
        {
            if (point.virtualPointType != VirtualPoint.VirtualPointType.Animals)
                continue;
            if(point.building == null)
            {
                Transform building = point.CreateBuilding(chunk.settings.decoratesRoot);
                if (building != null)
                    buildings.Add(building);
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < vPoints.Count; i++ )
            vPoints[i].Reset();
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

    public override void SetSelectTag(bool isSelect)
    {
        selectTag.SetActive(isSelect);
    }

    void OnLongPress( LongPressGesture gesture )
    {
        //Debug.Log("Long press");
    }
}