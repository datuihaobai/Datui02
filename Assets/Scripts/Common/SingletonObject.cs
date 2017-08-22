using UnityEngine;
using System.Collections;

public class SingletonObject<T> where T : new()
{
    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
				_instance = new T();
            return _instance;
        }
    }
}