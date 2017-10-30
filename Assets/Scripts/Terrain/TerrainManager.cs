using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using Game.Messenger;
using DG.Tweening;

public class TerrainManager : SingletonAppMonoBehaviour<TerrainManager>
{
    public enum TerrainCommonKey
    {
        MinDistanceOfCrystal = 1,//水晶之间的最小距离
        MinIgnoreElementValue = 2,//地表融合时忽略较小属性的最小差值
        GenerateEggsCountPerDay = 3,//地图上每天随机生成蛋的数量
        MaxEggsCount = 4,//地图上存在蛋的最大数量
    }

    public const string TerrainManagerEvent_PlaceBuilding = "TerrainManagerEvent_PlaceBuilding "; 
    public const int defaultBrushType = 0;
    public const string shuijingName = "shuij";
    public const string nestName = "nest_01";
    public const int crystalLevelMax = 3;

    public PATileTerrain tileTerrain;
    public Transform mainCameraRoot;
    public Transform mainCameraZoom;
    public int selectLevel;
    public PATileTerrain.TileElementType selectElementType;
    public Building.BuildingType selectBuildingType;
    public Shuijing selectShuijing;

    public Building toPlaceBuilding = null;

    private int minIgnoreElementValue = -1;//配置缓存
    private int minDistanceOfCrystal = -1;

    private bool isCrystalMode = false;
    private bool isOverUI = false;
    private bool isStarted = false;
    private bool isOverToPlaceBuilding = false;

    public int terrainChunkLayermask;
    private int editCrystalLayerMask;
    private int buildingLayer;
    private int buildingLayerMask;
    private int toPlaceBuildingLayer;
    private int toPlaceBuildingLayerMask;
    private int animalsLayerMask;

    void Start()
    {
        terrainChunkLayermask = LayerMask.NameToLayer("TerrainChunk");
        terrainChunkLayermask = 1 << terrainChunkLayermask;

        buildingLayer = LayerMask.NameToLayer("Building");
        buildingLayerMask = 1 << buildingLayer;

        toPlaceBuildingLayer = LayerMask.NameToLayer("ToPlaceBuilding");
        toPlaceBuildingLayerMask = 1 << toPlaceBuildingLayer;

        animalsLayerMask = LayerMask.NameToLayer("Animals");
        animalsLayerMask = 1 << animalsLayerMask;

        editCrystalLayerMask = terrainChunkLayermask | buildingLayerMask;

        StartCoroutine(ToStart());
    }

    void OnDestroy()
    {
        PlayerDataBase.instance.LocalSave();
    }

    IEnumerator ToStart()
    {
        ConfigDataBase.instance.StartLoad();
        if (!ConfigDataBase.instance.loadFinish)
            yield return null;
        //tileTerrain.GenerateTilePaintSamples();
        PlayerBorn();
        tileTerrain.LoadTerrain();
        isStarted = true;
    }

    /// <summary>
    /// 本地模拟初始化玩家数据，后面应该由服务器下发玩家数据
    /// </summary>
    void PlayerBorn()
    {
        bool isNewPlayer = !PlayerDataBase.instance.LocalLoad();
        if (isNewPlayer)
            PlayerDataBase.instance.LocalPlayerBorn();                                         
    }

    void Update()
    {
        //logTimer += Time.deltaTime;

        if (!isStarted)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (ClickIsOverUI.instance.IsPointerOverUIObject(Input.mousePosition))
                isOverUI = true;
            else
                isOverUI = false;
            if (IsPointerOverToPlaceBuilding())
                isOverToPlaceBuilding = true;
            else
                isOverToPlaceBuilding = false;
        }

        if (Input.GetMouseButton(0) && isOverToPlaceBuilding && toPlaceBuilding != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit, Mathf.Infinity, terrainChunkLayermask);
            toPlaceBuilding.transform.position = hit.point;
        
            //if(logTimer > 3f)
            //{ 
            //    logTimer = 0;
            //    Debug.Log("hit.point " + hit.point);
            //}
        }

        if (Input.GetMouseButtonUp(0))
        {
            CheckSelectAnimals();
            EditCrystal();
            EditBuilding();
        }
    }

    void CheckSelectAnimals()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, animalsLayerMask))
        {
            Messenger.Broadcast(UIEvent.UIEvent_ShowSelectDragon);
        }
    }

    public void ShowCrystal(bool isShow)
    {
        tileTerrain.ShowCrystal(isShow);
        isCrystalMode = isShow;
        if (!isShow)
            SetSelectShuijing(null);
    }

    public void RemoveToPlaceBuilding()
    {
        if (toPlaceBuilding == null)
            return;
        PoolManager.Pools["Shuijing"].Despawn(toPlaceBuilding.transform);
        toPlaceBuilding = null;
    }

    public void CreateToPlaceBuilding()
    {
        RemoveToPlaceBuilding();
        if (selectBuildingType == Building.BuildingType.None)
            toPlaceBuilding = null;
        else if (selectBuildingType == Building.BuildingType.Shuijing)
        {
            toPlaceBuilding = CreateCrystal(selectLevel,selectElementType);
            toPlaceBuilding.SetSelectTag(true);
        }
        else if (selectBuildingType == Building.BuildingType.Nest)
        {
            toPlaceBuilding = CreateNest(selectElementType);
        }
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

    bool CheckEditBuilding()
    {
        if (isOverUI)
            return false;
        if (TBPan.isMoving)
            return false;
        if (isCrystalMode)
            return false;

        return true;
    }

    public int GetTerrainCommon(TerrainCommonKey key)
    {
        foreach (var config in ConfigDataBase.instance.TerrainCommonConfigAsset.configs)
        {
            if (config.key == (int)key)
                return config.value;
        }

        return -1;
    }

    public int GetMinIgnoreElementValue()
    {
        return 0;
        //if (minIgnoreElementValue == -1)
        //    minIgnoreElementValue = GetTerrainCommon(TerrainCommonKey.MinIgnoreElementValue);

        //return minIgnoreElementValue;
    }

    public int GetMinDistanceOfCrystal()
    {
        if (minDistanceOfCrystal == -1)
            minDistanceOfCrystal = GetTerrainCommon(TerrainCommonKey.MinDistanceOfCrystal);

        return minDistanceOfCrystal;
    }

    bool CheckCrystalDistance(PATileTerrain.PATile newTile,Shuijing shuijing)
    {
        float minDistance = GetMinDistanceOfCrystal();
        foreach(var crystal in tileTerrain.settings.crystals)
        {
            if (crystal.shuijing.elementType == shuijing.elementType)
                continue;

            PATileTerrain.PATile tile = tileTerrain.GetTile(crystal.id);
            float distance = tile.Distance(newTile);
            if (distance < minDistance)
                return false;
        }
        
        return true;
    }

    void EditBuilding()
    {
        if (!CheckEditBuilding())
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos;
        int x, y;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, editCrystalLayerMask))
        {
            PATileTerrain tt = tileTerrain.IsTerrain(hit.transform);
            NestBuilding hitNest = hit.transform.GetComponent<NestBuilding>();
            HatchBuilding hitHatch = hit.transform.GetComponent<HatchBuilding>();
            if (tt != null)
            {
                pos = tileTerrain.transform.InverseTransformPoint(hit.point);
                x = (int)Mathf.Abs(pos.x / tileTerrain.tileSize);
                y = (int)Mathf.Abs(pos.z / tileTerrain.tileSize);
                PATileTerrain.PATile tile = tileTerrain.GetTile(x, y);
                PATileTerrain.PABuildingTile buildingTile = PATileTerrain.PABuildingTile.GetByTile(tileTerrain, tile);

                if (toPlaceBuilding is NestBuilding && buildingTile.keyTile.affectShuijing != null)
                {
                    NestBuilding nest = toPlaceBuilding as NestBuilding;
                    PlaceNest(nest, buildingTile);
                    toPlaceBuilding = null;
                    //Messenger.Broadcast(TerrainManagerEvent_PlaceBuilding);
                }
            }
            else if (hitNest != null && toPlaceBuilding == null)
                Messenger.Broadcast(UIEvent.UIEvent_ShowSelectNest);
            else if (hitHatch != null && toPlaceBuilding == null)
                Debug.Log("hitHatch != null ");
        }
    }

    void EditCrystal()
    {
        if (!CheckEditCrystal())
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos;
        int x, y;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, editCrystalLayerMask))
        {
            PATileTerrain tt = tileTerrain.IsTerrain(hit.transform);
            Shuijing hitShuijing = hit.transform.GetComponent<Shuijing>();
            if (tt != null)
            {
                if(toPlaceBuilding == null)
                {
                    SetSelectShuijing(null);
                    Messenger<PATileTerrain.PATile>.Broadcast(UIEvent.UIEvent_ShowSelectCrystal, null);
                }
                else
                {
                    pos = tileTerrain.transform.InverseTransformPoint(hit.point);
                    x = (int)Mathf.Abs(pos.x / tileTerrain.tileSize);
                    y = (int)Mathf.Abs(pos.z / tileTerrain.tileSize);
                    PATileTerrain.PATile tile = tileTerrain.GetTile(x, y);
                    PATileTerrain.PABuildingTile buildingTile = PATileTerrain.PABuildingTile.GetByTile(tileTerrain, tile);

                    Shuijing shuijing = toPlaceBuilding as Shuijing;
                    if (!CheckCrystalDistance(buildingTile.keyTile,shuijing))
                    {
                        Messenger.Broadcast(UIEvent.UIEvent_CrystalDistanceTip);
                        return;
                    }

                    PlaceCrystal(shuijing, buildingTile);
                    shuijing.SetSelectTag(false);
                    RepaintAllCrystals();
                    //PaintCrystal(shuijing);
                    toPlaceBuilding = null;
                    //SetSelectShuijing(shuijing);
                    Messenger.Broadcast(TerrainManagerEvent_PlaceBuilding);
                }
            }
            else if (hitShuijing != null && toPlaceBuilding == null)
            {
                SetSelectShuijing(hitShuijing);
                MoveCameraToPos(hitShuijing.transform, MoveToCrystalCallBack);
            }
            else
                SetSelectShuijing(null);
        }
    }

    void MoveToCrystalCallBack()
    {
        Messenger.Broadcast(UIEvent.UIEvent_ShowCrystalOption);
    }

    public delegate void MoveCallBack();

    void MoveCameraToPos(Transform targetTrans,MoveCallBack moveCallBack = null)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainChunkLayermask))
        {
            //Camera.main.transform.position = targetTrans.position + (-Camera.main.transform.forward * hit.distance);
            Vector3 targetPos = targetTrans.position + 
                (-Camera.main.transform.forward * (hit.distance + mainCameraZoom.localPosition.z));
            targetPos.y = mainCameraRoot.position.y;
            mainCameraRoot.DOMove(targetPos, 0.2f).
                OnComplete(() => {
                    if (moveCallBack != null)
                        moveCallBack();
            });
        }
    }

    Shuijing CreateCrystal(PATileTerrain.PATile tile, int level, PATileTerrain.TileElementType elementType)
    {
        PATileTerrain.PABuildingTile buildingTile = PATileTerrain.PABuildingTile.GetByTile(tileTerrain, tile);
        Shuijing shuijing = CreateCrystal(level, elementType);
        return PlaceCrystal(shuijing, buildingTile);
    }

    //Shuijing CreateCrystal(PATileTerrain.PACrystalTile crystalTile, int level,PATileTerrain.TileElementType elementType)
    Shuijing CreateCrystal(int level,PATileTerrain.TileElementType elementType)
    {
        string preShuijingName = "";
        if (elementType == PATileTerrain.TileElementType.Fire)
            preShuijingName = "v_";
        else if (elementType == PATileTerrain.TileElementType.Wood)
            preShuijingName = "w_";
        string shuijingPrefabName = preShuijingName + shuijingName + level.ToString();
        GameObject shuijingGo = PoolManager.Pools["Shuijing"].Spawn(shuijingPrefabName).gameObject;
        GameUtility.SetLayerRecursive(shuijingGo.transform,toPlaceBuildingLayer);
        Shuijing shuijing = shuijingGo.GetComponent<Shuijing>();
        shuijing.level = level;
        shuijing.elementType = elementType;
        shuijing.prefabName = shuijingPrefabName;
        shuijing.Reset();
        //shuijing.SetSelectTag(true);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2,0));
        Physics.Raycast(ray, out hit, Mathf.Infinity, terrainChunkLayermask);
        shuijing.transform.position = hit.point;

        return shuijing;
    }

    public Shuijing PlaceCrystal(Shuijing shuijing, PATileTerrain.PABuildingTile buildingTile)
    {
        PATileTerrainChunk chunk = tileTerrain.GetChunk(buildingTile.keyTile.chunkId);
        shuijing.gameObject.transform.SetParent(chunk.settings.crystalGo.transform);
        shuijing.gameObject.transform.position = buildingTile.GetBuildingPos(tileTerrain);
        buildingTile.keyTile.shuijing = shuijing;
        shuijing.tile = buildingTile.keyTile;
        GameUtility.SetLayerRecursive(shuijing.transform,buildingLayer);
        PATileTerrain.PACrystalBuilding crystalBuildingData = new PATileTerrain.PACrystalBuilding(
            buildingTile.keyTile.id, shuijing.level, shuijing.elementType, shuijing.prefabName, RandomManager.NewSeed());
        crystalBuildingData.shuijing = shuijing;
        tileTerrain.settings.crystals.Add(crystalBuildingData);

        return shuijing;
    }

    NestBuilding CreateNest(PATileTerrain.TileElementType elementType)
    {
        string preName = "";
        if (elementType == PATileTerrain.TileElementType.Fire)
            preName = "v_";
        else if (elementType == PATileTerrain.TileElementType.Wood)
            preName = "w_";
        string nestPrefabName = preName + nestName;
        GameObject nestGo = PoolManager.Pools["Shuijing"].Spawn(nestPrefabName).gameObject;
        GameUtility.SetLayerRecursive(nestGo.transform,toPlaceBuildingLayer);
        NestBuilding nest = nestGo.GetComponent<NestBuilding>();
        nest.elementType = elementType;
        nest.prefabName = nestPrefabName;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Physics.Raycast(ray, out hit, Mathf.Infinity, terrainChunkLayermask);
        nest.transform.position = hit.point;

        return nest;
    }

    void PlaceNest(NestBuilding nest, PATileTerrain.PABuildingTile buildingTile)
    {
        Shuijing belongShuijing = buildingTile.keyTile.affectShuijing;
        if (belongShuijing == null)
            return;

        PATileTerrainChunk chunk = tileTerrain.GetChunk(buildingTile.keyTile.chunkId);
        nest.gameObject.transform.SetParent(chunk.settings.buildingsRoot.transform);
        nest.gameObject.transform.position = buildingTile.GetBuildingPos(tileTerrain);
        nest.tile = buildingTile.keyTile;
        //nest.belongShuijing = belongShuijing;
        GameUtility.SetLayerRecursive(nest.transform, buildingLayer);
        PATileTerrain.PABuilding buildingData = new PATileTerrain.PABuilding(
            buildingTile.keyTile.id, nest.elementType, nest.prefabName);
        tileTerrain.settings.GetCrystalBuilding(belongShuijing.tile.id).AddBuilding(buildingData);
        buildingData.belongShuijingId = belongShuijing.tile.id;
        belongShuijing.buildings.Add(nest.transform);
    }

    public void SetSelectShuijing(Shuijing shuijing)
    {
        if (selectShuijing != null)
            selectShuijing.SetSelectTag(false);
        selectShuijing = shuijing;
        if (selectShuijing != null)
            selectShuijing.SetSelectTag(true);
        else
            Messenger.Broadcast(UIEvent.UIEvent_ShowCrystalOption);
    }

    public void UpgradeSelectShuijing()
    {
        if (selectShuijing == null)
            return;
        PATileTerrain.PATile tile = selectShuijing.tile;
        int newLevel = selectShuijing.level + 1;
        PATileTerrain.TileElementType elementType = selectShuijing.elementType;
        RemoveCrystal(selectShuijing);
        Shuijing shuijing =  CreateCrystal(tile,newLevel,elementType);
        //PaintCrystal(shuijing);
        RepaintAllCrystals();
        SetSelectShuijing(null);
    }

    public void RemoveSelectShuijingAndRepaintAll()
    {
        RemoveCrystal(selectShuijing);
        tileTerrain.FillTerrain(defaultBrushType);
        RepaintAllCrystals();
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

        //tileTerrain.FillTerrain(defaultBrushType);
        shuijing.RemoveBuildings();
        PoolManager.Pools["Shuijing"].Despawn(shuijing.transform);
        tileTerrain.settings.RemoveCrystal(shuijing.tile.id);
        shuijing.tile.shuijing = null;
        //RepaintAllCrystals();
    }

    bool CalTileElement(PATileTerrain.PATile tile, Vector2 crystalPos, float centerValue, float atten,PATileTerrain.TileElementType elementType)
    {
        //tile.Reset();
        Vector2 tilePos = new Vector2(tile.x + 0.5f, tile.y + 0.5f);
        float distance = Vector2.Distance(crystalPos, tilePos);
        //if (tile.distance.Equals(-1))
        //    tile.distance = distance;
        //else
        //    tile.distance = Mathf.Min(tile.distance, distance);
        float tileElementValue = Mathf.Max(centerValue - atten * distance, 0);
        if (tileElementValue > 0f)
        {
            tile.element.AddElement(elementType, tileElementValue);
            return true;
        }
        return false;
    }
    // includeMore == true 为了优化结算 多收集一圈tile 
    void PaintElement(Shuijing shuijing, ref Dictionary<int, PATileTerrain.PATile> collectTiles,bool includeMore = false)
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
        bool hasValue = false;
        Vector2 crystalPos = new Vector2(shuijing.tile.x + 1,shuijing.tile.y + 1);
        CalTileElement(shuijing.tile, crystalPos, centerValue, atten, shuijing.elementType);
        if (!collectTiles.ContainsKey(shuijing.tile.id))
            collectTiles.Add(shuijing.tile.id,shuijing.tile);

        if (!shuijing.affectTiles.ContainsKey(shuijing.tile.id))
            shuijing.affectTiles.Add(shuijing.tile.id,shuijing.tile);
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
                hasValue = CalTileElement(tile, crystalPos, centerValue, atten, shuijing.elementType);
                if (hasValue)
                {
                    if (!collectTiles.ContainsKey(tile.id))
                        collectTiles.Add(tile.id, tile);
                    if (!shuijing.affectTiles.ContainsKey(tile.id))
                        shuijing.affectTiles.Add(tile.id,tile);
                    if (tile.affectShuijing == null)
                        tile.affectShuijing = shuijing;
                    outOfRange = false;
                } 
                else
                {
                    if (includeMore && !collectTiles.ContainsKey(tile.id))
                        collectTiles.Add(tile.id, tile);
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

    //public void PaintCrystal(Shuijing shuijing)
    //{
    //    Dictionary<int, PATileTerrain.PATile> collectTiles = new Dictionary<int, PATileTerrain.PATile>();
    //    // 设置属性值
    //    PaintElement(shuijing, ref collectTiles,true);

    //    //设置地表贴图
    //    tileTerrain.PaintTiles(ref collectTiles,true);

    //    //设置贴花
    //    //RepaintAllDecalAndRebuildAll();

    //    PaintDecals(ref collectTiles);
    //    shuijing.RemoveBuildings();
    //    shuijing.CreateBuildings(tileTerrain);
    //}

    public void RepaintAllCrystals()
    {
        tileTerrain.ResetTile();
        //List<PATileTerrain.PATile> collectTiles = new List<PATileTerrain.PATile>();
        Dictionary<int, PATileTerrain.PATile> collectTiles = new Dictionary<int, PATileTerrain.PATile>();
        // 设置属性值
        foreach (var crystal in tileTerrain.settings.crystals)
            PaintElement(crystal.shuijing, ref collectTiles);

        //设置地表贴图
        tileTerrain.PaintTiles(ref collectTiles);

        //设置贴花
        RepaintDecals();

        //设置建筑
        RecreateBuildings();
    }

    void RepaintDecals()
    {
        foreach (var crystal in tileTerrain.settings.crystals)
        {
            RandomManager.instance.SetSeed(crystal.randomSeed);
            foreach (var tile in crystal.shuijing.affectTiles.Values)
                tileTerrain.PaintATileDecal(tile);
        }
    }

    void RecreateBuildings()
    {
        foreach (var crystal in tileTerrain.settings.crystals)
            crystal.shuijing.CreateBuildings(tileTerrain);
    }


    public void GenerateEggs()
    {
        int generateMaxCount = GetTerrainCommon(TerrainCommonKey.GenerateEggsCountPerDay);
        int generateCount = 0;

        foreach (var crystal in tileTerrain.settings.crystals)
        {
            foreach (var tile in crystal.shuijing.affectTiles.Values)
            {
                int randomValue = RandomManager.instance.Range(0,100);
                if (randomValue < 90)
                    continue;
                if(tile.IsFireTile())
                {

                }
            }
        }
    }

    public string GetUpgradeTips()
    {
        if (selectShuijing == null || selectShuijing.level >= 3)
            return "";
        return string.Format("是否用5个level{0}晶核将其升级到level{1}晶核", selectShuijing.level, selectShuijing.level + 1);
    }

    public bool IsPointerOverToPlaceBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, toPlaceBuildingLayerMask))
            return true;

        return false;
    }
}