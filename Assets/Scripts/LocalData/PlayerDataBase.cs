using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class PlayerDataBase : SingletonObject<PlayerDataBase> 
{
    //public int playerId;
    public EggDataBase eggDataBase = new EggDataBase();

    public void LocalPlayerBorn()
    {
        eggDataBase.LocalPlayerBorn();
        LocalSave();
    }

    public JSONNode ToJson()
    {
        JSONNode jsnode = new JSONClass();
        jsnode["eggDataBase"] = eggDataBase.ToJson();
        return jsnode;
    }

    public void FromJson(JSONNode jsnode)
    {
        eggDataBase = new EggDataBase();
        eggDataBase.FromJson(jsnode["eggDataBase"]);
    }

    public void LocalSave()
    {
        string content = ToJson().ToString();
        string path = Application.persistentDataPath + "/datui_player";
        Debug.Log("Saved at " + path);
        File.Delete(path);
        if (string.IsNullOrEmpty(content))
            return;
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.Begin);
        sw.Write(content);
        sw.Close();
    }

    public bool LocalLoad()
    {
        string path = Application.persistentDataPath + "/datui_player";
        if (!File.Exists(path))
            return false;
        Debug.Log("Load from " + path);
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, (int)fs.Length);
        fs.Close();

        try
        {
            string content = System.Text.Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(content))
                return false;
            JSONNode jsnode = JSONNode.Parse(content);
            if (jsnode != null)
                FromJson(jsnode);
        }
        catch (System.Exception e)
        { }

        return true;
    }
}