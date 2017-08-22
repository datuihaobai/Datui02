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
}