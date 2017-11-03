using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Messenger;

public class UIHatchEggItem : MonoBehaviour 
{
    public Image iconImg;
    public Text countText;
    public EggData eggData;

    public void SetData(EggData eggData)
    {
        this.eggData = eggData;

        iconImg.sprite = Resources.Load<Sprite>(GameDefine.UITerrainSpritePath + eggData.ConfigData.icon);
        countText.text = "X1";
    }
}