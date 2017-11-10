using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class HatchBuilding : Building 
{
    public enum HatchType
    {
        Fire,
        Wood,
    }
    public int hatchId = -1;
    public HatchType hatchType;
    public Transform eggPos;

    public EggData hatchEggData = null;

    public void StartHatch(EggData hatchEggData)
    {
        if (hatchEggData == null)
            return;
        this.hatchEggData = hatchEggData;
        GameObject newEggGo = PoolManager.Pools["Shuijing"].Spawn(hatchEggData.ConfigData.prefab).gameObject;
        newEggGo.transform.SetParent(eggPos,false);
        Egg newEgg = newEggGo.GetComponent<Egg>();
        newEgg.eggData = hatchEggData;
        newEgg.hatchEffect.SetActive(true);
    }

    public void FinishOrCancelHatch()
    {
        if (hatchEggData == null)
            return;
        for (int i = 0; i < eggPos.childCount; i++)
        {
            Transform child = eggPos.GetChild(i);
            Egg egg = child.GetComponent<Egg>();
            if (egg == null)
                continue;
            egg.hatchEffect.SetActive(false);
            PoolManager.Pools["Shuijing"].Despawn(child);
        } 
        hatchEggData = null;
    }

    public bool CheckHatchEgg(EggData checkEggData)
    {
        EggData.ElementType eggElementType = (EggData.ElementType)checkEggData.ConfigData.elementType;
        if (eggElementType == EggData.ElementType.Universal)
            return true;
        else if (eggElementType == EggData.ElementType.Fire && hatchType == HatchType.Fire)
            return true;
        else if (eggElementType == EggData.ElementType.Wood && hatchType == HatchType.Wood)
            return true;
        return false;
    }
}