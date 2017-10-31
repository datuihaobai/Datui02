using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Messenger;

public class UISelectNest : MonoBehaviour 
{
    public GameObject root;

    void Awake()
    {
        root.SetActive(false);
        Messenger.AddListener(UIEvent.UIEvent_ShowSelectNest, OnShowSelectNest);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowSelectNest, OnShowSelectNest);
    }

    public void Show()
    {
        root.SetActive(true);
    }

    void Hide()
    {
        root.SetActive(false);
    }

    public void OnClose()
    {
        Hide();
    }

    void OnShowSelectNest()
    {
        Show();
    }
}