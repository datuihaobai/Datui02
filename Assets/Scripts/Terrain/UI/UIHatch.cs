using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHatch : MonoBehaviour 
{
    public GameObject root;

    public void Show()
    {
        root.SetActive(false);
    }

	public void Hide()
    {
        root.SetActive(false);
    }
}