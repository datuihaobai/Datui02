using UnityEngine;
using System.Collections;

public class PooledGameObject : MonoBehaviour 
{
    public GameObjectPool pool;
	public bool autoRecycle;
	public float autoRecycleTime;
	private float recycleTimer;

	public void Init()
	{
		ResetTimer ();
	}

	private void ResetTimer()
	{
		recycleTimer = autoRecycleTime;
	}

	void Update()
	{
		if(!autoRecycle)
			return;

		recycleTimer -= Time.deltaTime;
		if(recycleTimer <= 0)
			Push();
	}

    public void Push(bool needChangeParent = true)
    {
        pool.Push(this,needChangeParent);
    }
}