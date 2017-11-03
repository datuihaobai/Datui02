using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Messenger;

public class UIHatch : MonoBehaviour 
{
    public GameObject root;

    void Awake()
    {
        Hide();
        Messenger.AddListener(UIEvent.UIEvent_ShowHatch, Show);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowHatch, Show);
    }

    public void Show()
    {
        root.SetActive(true);
    }

	public void Hide()
    {
        root.SetActive(false);
    }

    public void OnClickHatch()
    {

    }
}