using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggData
{
    public enum ElementType
    {
        Universal = 0,
        Fire = 1,
        Wood = 2,
    }

    public int id;
}

public class EggDataBase
{
    List<EggData> eggs = new List<EggData>();
    
}