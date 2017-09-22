using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildingSelectButton : MonoBehaviour 
{
    public GameObject selectTag;
   
    public void SetSelect(bool isSelect)
    {
        if (selectTag == null)
            return;
        selectTag.SetActive(isSelect);
    }
}