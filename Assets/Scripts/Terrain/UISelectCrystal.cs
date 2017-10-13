using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Messenger;

public class UISelectCrystal : MonoBehaviour 
{
    public GameObject root;
    private PATileTerrain.PATile selectTile;

    void Awake()
    {
        Hide();
        Messenger<PATileTerrain.PATile>.AddListener(UIEvent.UIEvent_ShowSelectCrystal,OnShow);
    }

    void OnDestroy()
    {
        Messenger<PATileTerrain.PATile>.RemoveListener(UIEvent.UIEvent_ShowSelectCrystal, OnShow);
    }

    void OnShow(PATileTerrain.PATile tile)
    {
        this.selectTile = tile;
        Show();
    }

    public void Show()
    {
        root.SetActive(true);
    }
    
    public void Hide()
    {
        root.SetActive(false);
    }

    public void OnSelectFire()
    {
        if (selectTile == null)
            return;
        TerrainManager.instance.CreateNewCrystal(selectTile,1,PATileTerrain.TileElementType.Fire);
        Hide();
    }
    
    public void OnSelectWood()
    {
        if (selectTile == null)
            return;
        TerrainManager.instance.CreateNewCrystal(selectTile, 1, PATileTerrain.TileElementType.Wood);
        Hide();
    }
}