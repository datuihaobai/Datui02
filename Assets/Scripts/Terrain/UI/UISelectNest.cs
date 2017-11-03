using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Messenger;

public class UISelectNest : MonoBehaviour 
{
    public GameObject root;

    void Awake()
    {
        Hide();
        Messenger.AddListener(UIEvent.UIEvent_ShowSelectNest, Show);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowSelectNest, Show);
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