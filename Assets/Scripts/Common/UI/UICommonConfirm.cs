using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICommonConfirm : MonoBehaviour 
{
    public delegate void CallBack();
    public Text messageText;
    public CallBack confirmCallBack;
    public CallBack cancelCallBack;

    public void OnConfirm()
    {
        if (confirmCallBack != null)
            confirmCallBack();
        gameObject.SetActive(false);
    }
    
	public void OnCancel()
    {
        if (cancelCallBack != null)
            cancelCallBack();
        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        messageText.text = text;
    }
}