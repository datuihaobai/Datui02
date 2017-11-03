﻿using System.Collections;
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

        if (selectEggData.remainTime.Equals(0f))
            return;

        selectEggData.remainTime -= Time.deltaTime;
        if (selectEggData.remainTime < 0)
            selectEggData.remainTime = 0;
    }

    public void Show(int hatchId)
    {
        this.currentHatchId = hatchId;
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
            remainTimeText.text = "选择一个蛋来孵化";
        else
        {
            selectEggImg.gameObject.SetActive(true);
            selectEggImg.sprite = Resources.Load<Sprite>(GameDefine.UITerrainSpritePath + selectEggData.ConfigData.icon);
            if (selectEggData.remainTime == 0)
                remainTimeText.text = "点击孵化按钮";
            else
                remainTimeText.text = GameUtility.GetTimeStringMMSS(selectEggData.remainTime * 1000f);
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
        selectEggData = selectItem.eggData;
        selectEggData.Hatch(currentHatchId);
        Refresh();
    }

    public void OnClickHatch()
    {

    }

    public void OnClickCancel()
    {

    }
}