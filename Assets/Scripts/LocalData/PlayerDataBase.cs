using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class PlayerDataBase : SingletonObject<PlayerDataBase> 
{
    public int playerId;
    public EggDataBase eggDataBase = new EggDataBase();



    public JSONNode ToJson()
    {
        JSONNode jsnode = new JSONClass();

        return jsnode;
    }

    public void FromJson(JSONNode jsnode)
    {

    }
}