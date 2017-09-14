using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using Game.Messenger;

public class TerrainManager : SingletonAppMonoBehaviour<TerrainManager>
{
    public const int defaultBrushType = 0;
    public const string shuijingName = "shuij";
    public const int crystalLevelMax = 3;

    public PATileTerrain tileTerrain;
    public int selectLevel;
    public PATileTerrain.TileElementType selectElementType;
    public Shuijing selectShuijing;

    private bool isCrystalMode = false;
    private bool isOverUI = false;
    private bool isStarted = false;

    void Start()
    {
        StartCoroutine(ToStart());
    }

    IEnumerator ToStart()
    {
        ConfigDataBase.instance.StartLoad();
        if (!ConfigDataBase.instance.loadFinish)
            yield return null;
        tileTerrain.LoadTerrain();
        isStarted = true;
    }

    void Update()
    {
        if (!isStarted)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (ClickIsOverUI.instance.IsPointerOverUIObject(Input.mousePosition))
                isOverUI = true;
            else
                isOverUI = false;
        }

        if(Input.GetMouseButtonUp(0))
        {
            EditCrystal();
        }
    }

    public void ShowCrystal(bool isShow)
    {
        tileTerrain.ShowCrystal(isShow);
        isCrystalMode = isShow;
    }

    public void SaveTerrain()
    {
        tileTerrain.SaveTerrain();
    }

    public void NewTerrain()
    {
        tileTerrain.CreateTerrain();
        tileTerrain.FillTerrain(1);
    }

    bool CheckEditCrystal()
    {
        if (isOverUI)
            return false;
        if (TBPan.isMoving)
            return false;
        if (!isCrystalMode)
            return false;

        return true;
    }

    void EditCrystal()
    {
        if (!CheckEditCrystal())
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 pos;
        int x, y;
        int cameraMoveAreaLayer = LayerMask.NameToLayer("CameraMoveArea");
        int layermask = 1 << cameraMoveAreaLayer;
        layermask = ~layermask;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
        {
            PATileTerrain tt = tileTerrain.IsTerrain(hit.transform);
            Shuijing hitShuijing = hit.transform.GetComponent<Shuijing>();
            if (tt != null)
            {
                pos = tileTerrain.transform.InverseTransformPoint(hit.point);
                x = (int)Mathf.Abs(pos.x / tileTerrain.tileSize);
                y = (int)Mathf.Abs(pos.z / tileTerrain.tileSize);
                PATileTerrain.PATile tile = tileTerrain.GetTile(x, y);
                PATileTerrain.PACrystalTile crystalTile = PATileTerrain.PACrystalTile.GetByTile(tileTerrain,tile);
                if (crystalTile.leftBottomTile.shuijing == null)
                {
                    Shuijing shuijing = CreateCrystal(crystalTile,selectLevel,selectElementType);
                    //PaintCrystal(shuijing);
                    RepaintAllCrystals();
                    shuijing.CreateBuildings(tileTerrain);
                }
                SetSelectShuijing(crystalTile.leftBottomTile.shuijing);
            }
            else if (hitShuijing != null)
            {
                SetSelectShuijing(hitShuijing);
            } 
        }
    }

    Shuijing CreateCrystal(PATileTerrain.PATile tile, int level, PATileTerrain.TileElementType elementType)
    {
        PATileTerrain.PACrystalTile crystalTile = PATileTerrain.PACrystalTile.GetByTile(tileTerrain, tile);
        return CreateCrystal(crystalTile,level,elementType);
    }

    Shuijing CreateCrystal(PATileTerrain.PACrystalTile crystalTile, int level,PATileTerrain.TileElementType elementType)
    {
        string preShuijingName = "";
        if (elementType == PATileTerrain.TileElementType.Fire)
            preShuijingName = "v_";
        else if (elementType == PATileTerrain.TileElementType.Wood)
            preShuijingName = "w_";
        string shuijingPrefabName = preShuijingName + shuijingName + level.ToString();
        GameObject shuijingGo = PoolManager.Pools["Shuijing"].Spawn(shuijingPrefabName).gameObject;
        //GameObject shuijingGo = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Terrain/Shuijing/shuij"),Vector3.zero,Quaternion.identity);
        PATileTerrainChunk chunk = tileTerrain.GetChunk(crystalTile.leftBottomTile.chunkId);
        shuijingGo.transform.SetParent(chunk.settings.crystalGo.transform);
        //shuijingGo.transform.position = tileTerrain.transform.TransformPoint(tile.position);
        shuijingGo.transform.position = crystalTile.GetShuijingPos(tileTerrain);
        Shuijing shuijing = shuijingGo.GetComponent<Shuijing>();
        shuijing.level = level;
        shuijing.elementType = elementType;
        //shuijing.CreateBuildings(chunk.transform);
        crystalTile.leftBottomTile.shuijing = shuijing;
        shuijing.tile = crystalTile.leftBottomTile;
        PATileTerrain.PACrystal crystalData = new PATileTerrain.PACrystal(
            crystalTile.leftBottomTile.id, shuijing.level,elementType, shuijingPrefabName, RandomManager.NewSeed());
        crystalData.shuijing = shuijing;
        tileTerrain.settings.crystals.Add(crystalData);

        return shuijing;
    }

    void SetSelectShuijing(Shuijing shuijing)
    {
        selectShuijing = shuijing;
        Messenger.Broadcast(UIEvent.UIEvent_ShowCrystalOption);
    }

    public void UpgradeSelectShuijing()
    {
        if (selectShuijing == null)
            return;
        PATileTerrain.PATile tile = selectShuijing.tile;
        int newLevel = selectShuijing.level + 1;
        PATileTerrain.TileElementType elementType = selectShuijing.elementType;
        RemoveSelectShuijing();

        Shuijing newShuijing = CreateCrystal(tile,newLevel,elementType);
        //PaintCrystal(newShuijing);
        RepaintAllCrystals();
        newShuijing.CreateBuildings(tileTerrain);
        SetSelectShuijing(newShuijing);
    }

    public void RemoveSelectShuijing()
    {
        RemoveCrystal(selectShuijing);
    }

    public void ClearTerrain()
    {
        tileTerrain.FillTerrain(defaultBrushType);
        tileTerrain.ResetTile();
        foreach (var crystal in tileTerrain.settings.crystals)
        {
            if (crystal.shuijing == null)
                continue;
            crystal.shuijing.RemoveBuildings();
            PoolManager.Pools["Shuijing"].Despawn(crystal.shuijing.transform);
            crystal.shuijing.tile.shuijing = null;
        }

        tileTerrain.settings.ClearCrystal();
    }

    void RemoveCrystal(Shuijing shuijing)
    {
        if (shuijing == null)
            return;

        tileTerrain.FillTerrain(defaultBrushType);
        shuijing.RemoveBuildings();
        PoolManager.Pools["Shuijing"].Despawn(shuijing.transform);
        tileTerrain.settings.RemoveCrystal(shuijing.tile.id);
        shuijing.tile.shuijing = null;
        RepaintAllCrystals();
    }

    bool CalTileElement(PATileTerrain.PATile tile, Vector2 crystalPos, float centerValue, float atten,PATileTerrain.TileElementType elementType)
    {
        Vector2 tilePos = new Vector2(tile.x + 0.5f, tile.y + 0.5f);
        float distance = Vector2.Distance(crystalPos, tilePos);
        if (tile.distance.Equals(-1))
            tile.distance = distance;
        else
            tile.distance = Mathf.Min(tile.distance, distance);
        float tileElementValue = Mathf.Max(centerValue - atten * distance, 0);
        if (tileElementValue > 0f)
        {
            tile.element.AddElement(elementType, tileElementValue);
            return true;
        }
        return false;
    }

    void PaintElement(Shuijing shuijing,ref List<PATileTerrain.PATile> collectTiles)
    {
        shuijing.affectTiles.Clear();
        CrystalRangeConfigAsset.CrystalRangeConfig configData = null;
        foreach(var config in ConfigDataBase.instance.CrystalRangeConfigAsset.configs)
        {
            if (config.level == shuijing.level)
                configData = config;
        }

        if (configData == null)
            return;

        float centerValue = configData.centerValue / 100f;
        float atten = configData.atten / 100f;
        int rang = 1;
        bool outOfRange = false;
        Vector2 crystalPos = new Vector2(shuijing.tile.x + 1,shuijing.tile.y + 1);
        CalTileElement(shuijing.tile, crystalPos, centerValue, atten, shuijing.elementType);
        collectTiles.Add(shuijing.tile);
        shuijing.affectTiles.Add(shuijing.tile);
        shuijing.tile.affectShuijing = shuijing;
        while(true)
        {
            outOfRange = true;
            PATileTerrain.PATile[] tiles = tileTerrain.GetNeighboringTilesNxN(shuijing.tile,rang);
            rang += 2;
            foreach(var tile in tiles)
            {
                if (tile == null)
                    continue;
                if (CalTileElement(tile, crystalPos, centerValue, atten, shuijing.elementType))
                {
                    collectTiles.Add(tile);
                    if (tile.affectShuijing == null)
                    {
                        shuijing.affectTiles.Add(tile);
                        tile.affectShuijing = shuijing;
                    }
                    outOfRange = false;
                } 
            }
            if (outOfRange)
                break;
        }
    }

    //void PaintCrystal(Shuijing shuijing)
    //{
    //    if (shuijing == null)
    //        return;
    //    int brushType = shuijing.brushType;
        
    //    RandomManager.instance.SetSeed(tileTerrain.settings.GetCrystal(shuijing.tile.id).randomSeed);

    //    if(shuijing.level == 1)
    //    {
    //        //tileTerrain.PaintTileElementLevel1(shuijing.tile,shuijing.elementType);
    //        tileTerrain.PaintCrystalLevel1(shuijing.tile, brushType);
    //        //if(shuijing.elementType == PATileTerrain.TileElementType.Fire)
    //        //    tileTerrain.PaintCrystalLevel_Specified(shuijing.tile, brushType);
    //    }
    //    else if(shuijing.level == 2)
    //    {
    //        //tileTerrain.PaintTileElementLevel2(shuijing.tile, shuijing.elementType);
    //        tileTerrain.PaintCrystalLevel2(shuijing.tile, brushType);
    //        //tileTerrain.PaintCrystalLevel2_B(shuijing.tile, brushType + 1);
    //        //if (shuijing.elementType == PATileTerrain.TileElementType.Fire)
    //        //    tileTerrain.PaintCrystalLevel2_B_Specified(shuijing.tile,0);
    //    }
    //    else if (shuijing.level == 3)
    //    {
    //        //tileTerrain.PaintTileElementLevel3(shuijing.tile, shuijing.elementType);
    //        tileTerrain.PaintCrystalLevel3(shuijing.tile, brushType);
    //        tileTerrain.PaintCrystalLevel3_B(shuijing.tile, brushType + 1);
    //        tileTerrain.PaintCrystalLevel3_C(shuijing.tile, brushType + 2);
    //    }

    //    //shuijing.CreateBuildings(tileTerrain);
    //}

    public void RepaintAllCrystals()
    {
        tileTerrain.ResetTile();
        List<PATileTerrain.PATile> collectTiles = new List<PATileTerrain.PATile>();
        foreach (var crystal in tileTerrain.settings.crystals)
            PaintElement(crystal.shuijing,ref collectTiles);

        //foreach(var crystal in tileTerrain.settings.crystals)
        //    PaintCrystal(crystal.shuijing);
        tileTerrain.PaintTiles(ref collectTiles);

        foreach (var crystal in tileTerrain.settings.crystals)
        {
            RandomManager.instance.SetSeed(crystal.randomSeed);
            foreach (var tile in crystal.shuijing.affectTiles)
                tileTerrain.PaintATileDecal(tile);
        }
    }

    public string GetUpgradeTips()
    {
        if (selectShuijing == null || selectShuijing.level >= 3)
            return "";
        return string.Format("是否用5个level{0}晶核将其升级到level{1}晶核", selectShuijing.level, selectShuijing.level + 1);
    }
}