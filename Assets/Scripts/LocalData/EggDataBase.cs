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
    public float remainTime;//剩余孵化时间,单位秒
    public int hatchId;//孵化器id

    public EggConfigAsset.EggConfig  ConfigData
    {
        get 
        {
            if (configData == null)
                configData = ConfigDataBase.instance.EggConfigAsset.GetById(configId);
            return configData;
        }
    }
    private EggConfigAsset.EggConfig configData = null;

    public EggData()
    { }

    public EggData(int configId)
    {
        uId = localLastUId++;
        this.configId = configId;
        remainTime = -1;
        hatchId = -1;
    }

    public void StartHatch(int hatchId)
    {
        this.hatchId = hatchId;
        remainTime = ConfigData.time;
    }

    public void CancelHatch()
    {
        hatchId = -1;
        remainTime = -1;
    }

    public JSONNode ToJson()
    {
        JSONNode jsnode = new JSONClass();
        jsnode["uId"] = uId.ToString();
        jsnode["configId"] = configId.ToString();
        jsnode["remainTime"] = ((int)remainTime).ToString();
        jsnode["hatchId"] = hatchId.ToString();
        return jsnode;
    }

    public void FromJson(JSONNode jsnode)
    {
        uId = jsnode["uId"].AsInt;
        configId = jsnode["configId"].AsInt;
        remainTime = jsnode["remainTime"].AsInt;
        hatchId = jsnode["hatchId"].AsInt;
        if (localLastUId < uId + 1)
            localLastUId = uId + 1;
    }
}

public class EggDataBase
{
    private const int universalEggId = 1;
    public List<EggData> eggs = new List<EggData>();
 
    public void AddEgg(EggData addEggData)
    {
        eggs.Add(addEggData);
    }

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

    public void FinishHatch(EggData finishedEgg)
    {
        eggs.Remove(finishedEgg);
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