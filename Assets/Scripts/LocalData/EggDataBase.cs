using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EggData
{
    public static int localLastUId = 1001;

    public enum ElementType
    {
        Universal = 0,
        Fire = 1,
        Wood = 2,
    }

    public int uId;
    public int configId;
    public int remainTime;//剩余孵化时间,单位秒

    public EggData()
    { }

    public EggData(int configId)
    {
        uId = localLastUId++;
        this.configId = configId;
        remainTime = ConfigDataBase.instance.EggConfigAsset.GetById(configId).time;
    }

    public JSONNode ToJson()
    {
        JSONNode jsnode = new JSONClass();
        jsnode["uId"] = uId.ToString();
        jsnode["configId"] = configId.ToString();
        jsnode["remainTime"] = remainTime.ToString();
        return jsnode;
    }

    public void FromJson(JSONNode jsnode)
    {
        uId = jsnode["uId"].AsInt;
        configId = jsnode["configId"].AsInt;
        remainTime = jsnode["remainTime"].AsInt;
        localLastUId = uId + 1;
    }
}

public class EggDataBase
{
    private const int universalEggId = 1;
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
            EggData newEgg = new EggData(universalEggId);
            eggs.Add(newEgg);
        }
    }

    public JSONNode ToJson()
    {
        JSONNode jsnode = new JSONClass();
        JSONNode eggNodeArray = new JSONArray();
        foreach (var egg in eggs)
            eggNodeArray.Add(egg.ToJson());
        jsnode["eggs"] = eggNodeArray;
        return jsnode;
    }

    public void FromJson(JSONNode jsnode)
    {
        eggs.Clear();
        foreach (var eggNode in jsnode["eggs"].Childs)
        {
            EggData newEgg = new EggData();
            newEgg.FromJson(eggNode);
            eggs.Add(newEgg);
        }
    }
}