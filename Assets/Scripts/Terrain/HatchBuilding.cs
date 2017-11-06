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

    public EggData hatchEgg = null;

    public void StartHatch(EggData hatchEgg)
    {
        if (hatchEgg == null)
            return;
        this.hatchEgg = hatchEgg;
        GameObject newEggGo = PoolManager.Pools["Shuijing"].Spawn(hatchEgg.ConfigData.prefab).gameObject;
        newEggGo.transform.SetParent(eggPos,false);
        newEggGo.GetComponent<Egg>().hatchEffect.SetActive(true);
    }

    public void FinishOrCancelHatch()
    {
        if (hatchEgg == null)
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
        hatchEgg = null;
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