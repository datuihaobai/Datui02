using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICommonConfirm : MonoBehaviour 
{
    public class ShowData
    {
        public string text;
        public CallBack confirmCallBack = null;
        public CallBack cancelCallBack = null;
        public bool isHideCancel = false;
    }

    public delegate void CallBack();
    public Text messageText;
    public GameObject confirmButtonGo;
    public GameObject cancelButtonGo;
    private CallBack confirmCallBack;
    private CallBack cancelCallBack;

    public void Show(ShowData data = null)
    {
        gameObject.SetActive(true);
        if(data != null)
        {
            if (data.isHideCancel)
                cancelButtonGo.SetActive(false);
            else
                cancelButtonGo.SetActive(true);
            messageText.text = data.text;
            confirmCallBack = data.confirmCallBack;
            cancelCallBack = data.cancelCallBack;
        }
        else
        {
            confirmCallBack = null;
            cancelCallBack = null;
        }
    }

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
}