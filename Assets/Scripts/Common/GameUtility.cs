using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtility
{
    public static string toNumber(string str)
    {
        //不应该有等于 "" 的字符串  
        if (str == null || "".Equals(str.Trim()) || "\"\"".Equals(str.Trim()))
        {
            return "0";
        }
        return str;
    }

    public static void GetComponentsInChildrenRecursive<T>(Transform trans,ref List<T> results)
    {
        T component = trans.GetComponent<T>();
        if(component != null)
            results.Add(component);
        for (int i = 0; i < trans.childCount; i++ )
        {
            GetComponentsInChildrenRecursive<T>(trans.GetChild(i),ref results);
        }
    }

    public static void SetLayerRecursive(Transform trans,int layer)
    {
        trans.gameObject.layer = layer;
        for (int i = 0; i < trans.childCount; i++ )
        {
            Transform child = trans.GetChild(i);
            SetLayerRecursive(child,layer);
        }
    }
}