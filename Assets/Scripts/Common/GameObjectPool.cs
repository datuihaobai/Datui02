using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool : MonoBehaviour
{
    public PooledGameObject originalObj;
    private List<PooledGameObject> gameObjects = new List<PooledGameObject>();

    public PooledGameObject Pop(bool needChangeParent = false)
    {
        if (gameObjects.Count == 0)
            gameObjects.Add(GameObject.Instantiate(originalObj) as PooledGameObject);

        PooledGameObject go = gameObjects[gameObjects.Count - 1];
        go.gameObject.SetActive(true);
        gameObjects.Remove(go);
        if(needChangeParent)
            go.transform.SetParent(null, false);
		go.Init ();
		return go;
    }

    public void Push(PooledGameObject gameObj, bool needChangeParent = true)
    {
        if (gameObjects.Contains(gameObj))
            return;
        if (gameObj == originalObj)
            return;
        if (needChangeParent)
            gameObj.transform.SetParent(transform, false);
        gameObj.gameObject.SetActive(false);
        gameObjects.Add(gameObj);
    }

    public static void GiveBackToPool(Transform content)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            PooledGameObject pgo = content.GetChild(i).GetComponent<PooledGameObject>();
            pgo.Push(false);
        }
    }
}
