using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Messenger;

public class UISelectDragon : MonoBehaviour 
{
    public GameObject root;

    void Awake()
    {
        root.SetActive(false);
        Messenger.AddListener(UIEvent.UIEvent_ShowSelectDragon, OnShowSelectDragon);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(UIEvent.UIEvent_ShowSelectDragon, OnShowSelectDragon);
    }
    
    public void OnShowSelectDragon()
    {
        root.SetActive(true);
    }

    public void OnClickBg()
    {
        root.SetActive(false);
    }
}