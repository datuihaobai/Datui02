using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Messenger;

public class UISelectCrystal : MonoBehaviour 
{
    public GameObject root;

    void Awake()
    {
        Hide();
        Messenger.AddListener(UIEvent.UIEvent_ShowSelectCrystal,OnShow);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowSelectCrystal, OnShow);
    }

    void OnShow()
    {
        Show();
    }

    public void Show()
    {
        root.SetActive(true);
    }
    
    public void Hide()
    {
        root.SetActive(false);
    }
}