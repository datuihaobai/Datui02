using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchBuilding : Building 
{
    public enum HatchType
    {
        Fire,
        Wood,
    }

    public int hatchId = -1;
    public HatchType hatchType;

    public bool CheckHatchEgg(EggData checkEggData)
    {
        EggData.ElementType eggElementType = (EggData.ElementType)checkEggData.ConfigData.elementType;
        if (eggElementType == EggData.ElementType.Universal)
            return true;
        else if (eggElementType == EggData.ElementType.Fire || hatchType != HatchType.Fire)
            return false;
        else if (eggElementType == EggData.ElementType.Wood || hatchType != HatchType.Wood)
            return false;
        return false;
    }
}