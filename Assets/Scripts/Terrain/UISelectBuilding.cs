using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectBuilding : MonoBehaviour
{
    public GameObject root;
    public GameObject fireItemRoot;
    public GameObject woodItemRoot;
    public GameObject fireButtonOnGo;
    public GameObject fireButtonOffGo;
    public GameObject woodButtonOnGo;
    public GameObject woodButtonOffGo;

    void Awake()
    {
        Init();
    }

    public void Show()
    {
        root.SetActive(true);
        Init();
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    void Init()
    {
        SelectFire();
    }

    void SelectFire()
    {
        fireButtonOnGo.SetActive(true);
        fireButtonOffGo.SetActive(false);
        woodButtonOnGo.SetActive(false);
        woodButtonOffGo.SetActive(true);
        fireItemRoot.SetActive(true);
        woodItemRoot.SetActive(false);
    }

    void SelectWood()
    {
        fireButtonOnGo.SetActive(false);
        fireButtonOffGo.SetActive(true);
        woodButtonOnGo.SetActive(true);
        woodButtonOffGo.SetActive(false);
        fireItemRoot.SetActive(false);
        woodItemRoot.SetActive(true);
    }

    public void OnClickSelectFire()
    {
        SelectFire();
    }
    
    public void OnClickSelectWood()
    {
        SelectWood();
    }

    public void OnSelectBuildingFire()
    {
        TerrainManager.instance.selectBuildingType = Building.BuildingType.Nest;
        TerrainManager.instance.selectElementType = PATileTerrain.TileElementType.Fire;
        TerrainManager.instance.CreateToPlaceBuilding();
        Hide();
    }

    public void OnSelectBuildingWood()
    {
        TerrainManager.instance.selectBuildingType = Building.BuildingType.Nest;
        TerrainManager.instance.selectElementType = PATileTerrain.TileElementType.Wood;
        TerrainManager.instance.CreateToPlaceBuilding();
        Hide();
    }
}