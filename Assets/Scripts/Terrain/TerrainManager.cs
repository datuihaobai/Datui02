using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using PathologicalGames;
using Game.Messenger;

public class TerrainManager : SingletonAppMonoBehaviour<TerrainManager>
{
    public const int defaultBrushType = 0;
    public const string shuijingName = "shuij";
    public const int crystalLevelMax = 3;

    public PATileTerrain tileTerrain;
    public int selectLevel;
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
                    Shuijing shuijing = CreateCrystal(crystalTile,selectLevel);
                    PaintCrystal(shuijing);
                }
                SetSelectShuijing(crystalTile.leftBottomTile.shuijing);
            }
            else if (hitShuijing != null)
            {
                SetSelectShuijing(hitShuijing);
            } 
        }
    }

    Shuijing CreateCrystal(PATileTerrain.PATile tile, int level)
    {
        PATileTerrain.PACrystalTile crystalTile = PATileTerrain.PACrystalTile.GetByTile(tileTerrain, tile);
        return CreateCrystal(crystalTile,level);
    }

    Shuijing CreateCrystal(PATileTerrain.PACrystalTile crystalTile, int level)
    {
        string shuijingPrefabName = shuijingName + level.ToString();
        GameObject shuijingGo = PoolManager.Pools["Shuijing"].Spawn(shuijingPrefabName).gameObject;
        //GameObject shuijingGo = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Terrain/Shuijing/shuij"),Vector3.zero,Quaternion.identity);
        PATileTerrainChunk chunk = tileTerrain.GetChunk(crystalTile.leftBottomTile.chunkId);
        shuijingGo.transform.SetParent(chunk.settings.crystalGo.transform);
        //shuijingGo.transform.position = tileTerrain.transform.TransformPoint(tile.position);
        shuijingGo.transform.position = crystalTile.GetShuijingPos(tileTerrain);
        Shuijing shuijing = shuijingGo.GetComponent<Shuijing>();
        shuijing.level = level;
        //shuijing.CreateBuildings(chunk.transform);
        crystalTile.leftBottomTile.shuijing = shuijing;
        shuijing.tile = crystalTile.leftBottomTile;
        PATileTerrain.PACrystal crystalData = new PATileTerrain.PACrystal(
            crystalTile.leftBottomTile.id, shuijing.level, shuijingPrefabName, RandomManager.NewSeed());
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

        RemoveSelectShuijing();

        Shuijing newShuijing = CreateCrystal(tile,newLevel);
        PaintCrystal(newShuijing);
        SetSelectShuijing(newShuijing);
    }

    public void RemoveSelectShuijing()
    {
        RemoveCrystal(selectShuijing);
    }

    public void ClearTerrain()
    {
        tileTerrain.FillTerrain(defaultBrushType);
        tileTerrain.ResetTileElement();
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
        //shuijing.RemoveBuildings();
        //RepaintAllCrystals(false);
        //PoolManager.Pools["Shuijing"].Despawn(shuijing.transform);
        //tileTerrain.settings.RemoveCrystal(shuijing.tile.id);
        //shuijing.tile.shuijing = null;
        //RepaintAllCrystals(true);

        tileTerrain.FillTerrain(defaultBrushType);
        tileTerrain.ResetTileElement();
        foreach (var crystal in tileTerrain.settings.crystals)
        {
            if (crystal.shuijing == null)
                continue;
            crystal.shuijing.RemoveBuildings();
        }
        PoolManager.Pools["Shuijing"].Despawn(shuijing.transform);
        tileTerrain.settings.RemoveCrystal(shuijing.tile.id);
        RepaintAllCrystals();
    }

    void PaintCrystal(Shuijing shuijing)
    {
        if (shuijing == null)
            return;
        int brushType = shuijing.brushType;
        
        RandomManager.instance.SetSeed(tileTerrain.settings.GetCrystal(shuijing.tile.id).randomSeed);

        if(shuijing.level == 1)
        {
            tileTerrain.PaintCrystalLevel1(shuijing.tile, brushType);
            tileTerrain.PaintCrystalLevel_Specified(shuijing.tile,brushType);
            tileTerrain.PaintTileElementLevel1(shuijing.tile,PATileTerrain.TileElementType.Fire);
        }
        else if(shuijing.level == 2)
        {
            tileTerrain.PaintTileElementLevel2(shuijing.tile, PATileTerrain.TileElementType.Fire);
            tileTerrain.PaintCrystalLevel2(shuijing.tile, brushType);
            //tileTerrain.PaintCrystalLevel2_B(shuijing.tile, brushType + 1);
        }
        else if (shuijing.level == 3)
        {
            tileTerrain.PaintCrystalLevel3(shuijing.tile, brushType);
            tileTerrain.PaintCrystalLevel3_B(shuijing.tile, brushType + 1);
            tileTerrain.PaintCrystalLevel3_C(shuijing.tile, brushType + 2);
            tileTerrain.PaintTileElementLevel3(shuijing.tile, PATileTerrain.TileElementType.Fire);
        }

        shuijing.CreateBuildings(tileTerrain);
    }

    public void RepaintAllCrystals()
    {
        foreach(var crystal in tileTerrain.settings.crystals)
            PaintCrystal(crystal.shuijing);
    }

    public string GetUpgradeTips()
    {
        if (selectShuijing == null || selectShuijing.level >= 3)
            return "";
        return string.Format("是否用5个level{0}晶核将其升级到level{1}晶核", selectShuijing.level, selectShuijing.level + 1);
    }
}