using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomManager : SingletonObject<RandomManager>
{
    public void SetSeed(int seed)
    {
        //Debug.Log("RandomManager SetSeed seed = " + seed);
        Random.InitState(seed);
    }

    public int Range(int min,int max)
    {
        return Random.Range(min, max);
    }

    public static int NewSeed()
    {
        System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        return System.Convert.ToInt32(ts.TotalSeconds);
    }
}