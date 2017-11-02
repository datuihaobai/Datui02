using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public static string GetTimeStringHHMMSS(float t)
    {
        int hour = (int)(t / (1000 * 60 * 60));
        int min = (int)((t - hour * (1000 * 60 * 60)) / (1000 * 60));
        int sec = (int)((t - hour * (1000 * 60 * 60) - min * (1000 * 60)) / 1000);
        return string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
    }

    public static string GetTimeStringHHMM(float t)
    {
        int hour = (int)(t / (1000 * 60 * 60));
        int min = (int)((t - hour * (1000 * 60 * 60)) / (1000 * 60));
        //		int sec = (int)((t - hour*(1000*60*60) - min*(1000*60))/1000);
        return string.Format("{0:00}:{1:00}", hour, min);
    }

    public static string GetTimeStringMMSS(float t)
    {
        int hour = (int)(t / (1000 * 60 * 60));
        int min = (int)((t - hour * (1000 * 60 * 60)) / (1000 * 60));
        int sec = (int)((t - hour * (1000 * 60 * 60) - min * (1000 * 60)) / 1000);
        return string.Format("{0:00}:{1:00}", min, sec);
    }

    public static double ConvertDateTimeInt(System.DateTime time)
    {
        double intResult = 0;
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        intResult = (time - startTime).TotalMilliseconds;
        return intResult;
    }

    public static DateTime ConvertIntDatetime(double utc)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        startTime = startTime.AddMilliseconds(utc);
        //startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )  
        return startTime;
    }
}