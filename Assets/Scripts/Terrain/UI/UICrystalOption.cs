using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICrystalOption : MonoBehaviour 
{
    public GameObject root;
    public GameObject upgradeButton;
    public GameObject removeButton;

    public void Show()
    {
        if (TerrainManager.instance.selectShuijing == null)
        {
            Hide();
            return;
        }
        root.SetActive(true);
        if (TerrainManager.instance.selectShuijing.level < TerrainManager.crystalLevelMax)
            upgradeButton.SetActive(true);
        else
            upgradeButton.SetActive(false);

        removeButton.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}