using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Messenger;

public class UITerrainRoot : MonoBehaviour 
{
    public GameObject crystalOnGo;
    public GameObject crystalOffGo;
    public GameObject saveButtonGo;
    public GameObject clearButtonGo;
    public Dropdown crystalLevelSelect;
    public UICrystalOption crystalOption;
    public UICommonConfirm commonConfirm;

    void Awake()
    {
        Messenger.AddListener(UIEvent.UIEvent_ShowCrystalOption, OnShowCrystalOption);
        UpdateSelectLevel();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowCrystalOption, OnShowCrystalOption);
    }

    public void OnClickCrystalOn()
    {
        TerrainManager.instance.ShowCrystal(true);
        crystalOnGo.SetActive(false);
        crystalOffGo.SetActive(true);
        saveButtonGo.SetActive(true);
        clearButtonGo.SetActive(true);
        crystalLevelSelect.gameObject.SetActive(true);
    }

    public void OnClickCrystalOff()
    {
        TerrainManager.instance.ShowCrystal(false);
        crystalOnGo.SetActive(true);
        crystalOffGo.SetActive(false);
        saveButtonGo.SetActive(false);
        clearButtonGo.SetActive(false);
        crystalLevelSelect.gameObject.SetActive(false);
        crystalOption.Hide();
    }
    
    public void OnClickSave()
    {
        TerrainManager.instance.SaveTerrain();
    }

    public void OnClickClear()
    {
        TerrainManager.instance.ClearTerrain();
    }

    public void OnClickCreate()
    {
        TerrainManager.instance.NewTerrain();
        OnClickSave();
    }

    public void OnSelectLevelChanged()
    {
        UpdateSelectLevel();
    }

    public void OnCrystalUpgrade()
    {
        commonConfirm.gameObject.SetActive(true);
        commonConfirm.confirmCallBack = OnConfirmUpgrade;
        commonConfirm.SetText(TerrainManager.instance.GetUpgradeTips());
    }

    void OnConfirmUpgrade()
    {
        TerrainManager.instance.UpgradeSelectShuijing();
        crystalOption.Show();
    }

    public void OnCrystalRemove()
    {
        TerrainManager.instance.RemoveSelectShuijing();
        crystalOption.Hide();
    }

    void UpdateSelectLevel()
    {
        TerrainManager.instance.selectLevel = crystalLevelSelect.value + 1;
    }

    void OnShowCrystalOption()
    {
        crystalOption.Show();
    }
}