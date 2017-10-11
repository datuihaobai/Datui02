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
    public UIBuildingSelectButton crystalButton;
    public UIBuildingSelectButton nestButton;

    void Awake()
    {
        Messenger.AddListener(UIEvent.UIEvent_ShowCrystalOption, OnShowCrystalOption);
        Messenger.AddListener(UIEvent.UIEvent_CrystalDistanceTip, OnCrystalDistanceTip);
        Messenger.AddListener(TerrainManager.TerrainManagerEvent_PlaceBuilding,OnPlaceBuilding);
        UpdateSelectLevel();
        UpdateSelectElement();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowCrystalOption, OnShowCrystalOption);
        Messenger.RemoveListener(UIEvent.UIEvent_CrystalDistanceTip, OnCrystalDistanceTip);
        Messenger.RemoveListener(TerrainManager.TerrainManagerEvent_PlaceBuilding, OnPlaceBuilding);
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

    public void OnSelectBuildingCrystal()
    {
        crystalButton.SetSelect(true);
        nestButton.SetSelect(false);
        TerrainManager.instance.SetSelectShuijing(null);
        TerrainManager.instance.selectBuildingType = Building.BuildingType.Shuijing;
        TerrainManager.instance.CreateToPlaceBuilding();
    }

    public void OnSelectBuildingNest()
    {
        crystalButton.SetSelect(false);
        nestButton.SetSelect(true);
        TerrainManager.instance.SetSelectShuijing(null);
        TerrainManager.instance.selectBuildingType = Building.BuildingType.Nest;
        TerrainManager.instance.CreateToPlaceBuilding();
    }

    public void OnCrystalUpgrade()
    {
        UICommonConfirm.ShowData showData = new UICommonConfirm.ShowData();
        showData.text = TerrainManager.instance.GetUpgradeTips();
        showData.confirmCallBack = OnConfirmUpgrade;
        commonConfirm.Show(showData);
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
        TerrainManager.instance.CreateToPlaceBuilding();
    }

    void UpdateSelectElement()
    {
        TerrainManager.instance.selectElementType = (PATileTerrain.TileElementType)(crystalElementSelect.value + 1);
        TerrainManager.instance.CreateToPlaceBuilding();
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

    void OnCrystalDistanceTip()
    {
        UICommonConfirm.ShowData showData = new UICommonConfirm.ShowData();
        showData.isHideCancel = true;
        showData.text = string.Format("水晶之间至少要间隔{0}的距离",
            TerrainManager.instance.GetMinDistanceOfCrystal());
        commonConfirm.Show(showData);
    }

    void OnPlaceBuilding()
    {
        //buildingTypeSelect.value = 0;
        crystalButton.SetSelect(false);
        nestButton.SetSelect(false);
        TerrainManager.instance.selectBuildingType = Building.BuildingType.None;
        TerrainManager.instance.CreateToPlaceBuilding();
    }
}