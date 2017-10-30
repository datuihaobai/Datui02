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

    public int uId;
    public int configId;
    public int remainTime;//剩余孵化时间,单位秒
}

public class EggDataBase
{
    private const int universalEggId = 0;

    public static int localLastUId = 1001;
    List<EggData> eggs = new List<EggData>();
 
    public EggData GetEggByUId(int uId)
    {
        foreach(var egg in eggs)
        {
            if (uId == egg.uId)
                return egg;
        }
        return null;
    }

    //本地模拟玩家数据，初始给玩家5个万能蛋
    public void LocalPlayerBorn()
    {
        for (int i = 0; i < 5; i ++)
        {
            EggData newEgg = new EggData();
            newEgg.uId = localLastUId++;
            newEgg.configId = universalEggId;//万能蛋的配置id是0
            eggs.Add(newEgg);
        }
    }
}