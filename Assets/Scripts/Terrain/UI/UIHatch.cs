using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Messenger;

public class UIHatch : MonoBehaviour 
{
    public GameObject root;
    public GameObjectPool itemPool;
    public Transform itemRoot;
    public Image selectEggImg;
    public Text remainTimeText;

    private EggData selectEggData = null;
    private int currentHatchId = -1;
    private HatchBuilding currentHatchBuilding = null;
    private bool finished = false;

    void Awake()
    {
        Hide();
        Messenger<int>.AddListener(UIEvent.UIEvent_ShowHatch, Show);
    }

    void OnDestroy()
    {
        Messenger<int>.RemoveListener(UIEvent.UIEvent_ShowHatch, Show);
    }

    void Update()
    {
        if (selectEggData == null)
            return;

        if (finished)
            return;

        selectEggData.remainTime -= Time.deltaTime;
        if (selectEggData.remainTime < 0)
            selectEggData.remainTime = 0;

        if (selectEggData.remainTime.Equals(0f))
        {
            remainTimeText.text = "点击孵化按钮";
            finished = true;
        }
        else
            remainTimeText.text = GameUtility.GetTimeStringHHMMSS(selectEggData.remainTime * 1000f);
    }

    public void Show(int hatchId)
    {
        this.currentHatchId = hatchId;
        this.currentHatchBuilding = TerrainManager.instance.tileTerrain.GetHatchById(currentHatchId);
        root.SetActive(true);
        Refresh();
    }

    void Refresh()
    {
        GameObjectPool.GiveBackToPool(itemRoot);
        foreach (var eggData in PlayerDataBase.instance.eggDataBase.eggs)
        {
            if (eggData.hatchId >= 0)
            {
                if (eggData.hatchId == currentHatchId)
                    selectEggData = eggData;
                continue;
            }

            UIHatchEggItem eggItem = itemPool.Pop().GetComponent<UIHatchEggItem>();
            eggItem.transform.SetParent(itemRoot, false);
            eggItem.SetData(eggData);
        }

        if (selectEggData == null)
        {
            remainTimeText.text = "选择一个蛋来孵化";
            selectEggImg.gameObject.SetActive(false);
        }
        else
        {
            selectEggImg.gameObject.SetActive(true);
            selectEggImg.sprite = Resources.Load<Sprite>(GameDefine.UITerrainSpritePath + selectEggData.ConfigData.icon);
        }
    }

	public void Hide()
    {
        root.SetActive(false);
    }

    public void OnSelectItem(UIHatchEggItem selectItem)
    {
        if (selectEggData != null)
            return;

        if (currentHatchBuilding == null)
            return;
        if (!currentHatchBuilding.CheckHatchEgg(selectItem.eggData))
            return;
        
        selectEggData = selectItem.eggData;
        selectEggData.StartHatch(currentHatchId);
        currentHatchBuilding.StartHatch(selectEggData);
        Refresh();
        finished = false;
    }

    public void OnClickHatch()
    {
        if (selectEggData == null)
            return;
        if (selectEggData.remainTime > 0)
            return;
        PlayerDataBase.instance.eggDataBase.FinishHatch(selectEggData);
        currentHatchBuilding.FinishOrCancelHatch();
        Messenger<int>.Broadcast(UIEvent.UIEvent_HatchEgg,currentHatchId);
        selectEggData = null;
        Refresh();
    }

    public void OnClickCancel()
    {
        if (selectEggData == null)
            return;

        selectEggData.CancelHatch();
        selectEggData = null;
        currentHatchBuilding.FinishOrCancelHatch();
        Refresh();
    }

    public void OnClickSpeedUp()
    {
        if (selectEggData == null)
            return;
        selectEggData.remainTime = selectEggData.remainTime * 0.8f;
    }
}