using UnityEngine;
using System;
using System.Collections;

public class SingletonAppMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T classInstance;

	public static T instance {
		get {
			if (classInstance == null) {
                classInstance = FindObjectOfType (typeof(T)) as T;
                if (classInstance == null) {
                    classInstance = new GameObject().AddComponent<T>();
                    //Debug.LogWarning("SingletonAppMonoBehaviour not found ");
                }
			}
			return classInstance;
		}
	}

	protected virtual void Awake()
	{
		//CheckInstance();
	}

	protected bool CheckInstance()
    {
        if ( this == instance ) {
			return true;
		}
        Debug.Log("****************************** DestroyImmediate instance");
		DestroyImmediate(this);
		return false;
	}

	protected virtual void OnDestroy()
	{
		//Destroy(gameObject);
	}
}
