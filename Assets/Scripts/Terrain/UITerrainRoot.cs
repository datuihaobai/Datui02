using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Messenger;

public class UITerrainRoot : MonoBehaviour 
{
    public GameObject crystalRoot;
    public GameObject crystalOnGo;
    public Dropdown crystalLevelSelect;
    public Dropdown crystalElementSelect;
    public Dropdown buildingTypeSelect;

    public UICrystalOption crystalOption;
    public UICommonConfirm commonConfirm;

    void Awake()
    {
        Messenger.AddListener(UIEvent.UIEvent_ShowCrystalOption, OnShowCrystalOption);
        Messenger.AddListener(TerrainManager.TerrainManagerEvent_PlaceBuilding,OnPlaceBuilding);
        UpdateSelectLevel();
        UpdateSelectElement();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowCrystalOption, OnShowCrystalOption);
    }

    public void OnClickCrystalOn()
    {
        TerrainManager.instance.ShowCrystal(true);
        crystalOnGo.SetActive(false);
        crystalRoot.SetActive(true);
    }

    public void OnClickCrystalOff()
    {
        TerrainManager.instance.ShowCrystal(false);
        crystalOnGo.SetActive(true);
        crystalRoot.SetActive(false);
        crystalOption.Hide();
        buildingTypeSelect.value = 0;
    }
    
    public void OnClickSave()
    {
        TerrainManager.instance.SaveTerrain();
    }

    public void OnClickClear()
    {
        TerrainManager.instance.ClearTerrain();
    }

    public void OnSelectLevelChanged()
    {
        UpdateSelectLevel();
    }

    public void OnSelectElementChanged()
    {
        UpdateSelectElement();
    }

    public void OnSelectBuildingTypeChanged()
    {
        UpdateSelectBuildingType();
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

    void UpdateSelectElement()
    {
        TerrainManager.instance.selectElementType = (PATileTerrain.TileElementType)(crystalElementSelect.value + 1);
    }

    void UpdateSelectBuildingType()
    {
        TerrainManager.instance.selectBuildingType = (Building.BuildingType)buildingTypeSelect.value;
        TerrainManager.instance.CreateToPlaceBuilding();
    }

    void OnShowCrystalOption()
    {
        crystalOption.Show();
    }

    void OnPlaceBuilding()
    {
        buildingTypeSelect.value = 0;
    }
}