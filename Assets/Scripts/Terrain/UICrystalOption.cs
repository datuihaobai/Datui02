using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICrystalOption : MonoBehaviour 
{
    public GameObject upgradeButton;
    public GameObject removeButton;
	
    public void Show()
    {
        if (TerrainManager.instance.selectShuijing == null)
            return;

        if (TerrainManager.instance.selectShuijing.level < TerrainManager.crystalLevelMax)
            upgradeButton.SetActive(true);
        else
            upgradeButton.SetActive(false);

        removeButton.SetActive(true);
    }

    public void Hide()
    {
        upgradeButton.SetActive(false);
        removeButton.SetActive(false);
    }
}