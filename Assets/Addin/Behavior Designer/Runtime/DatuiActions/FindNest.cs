using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Datui")]
public class FindNest : Action
{
    public SharedInt nestId;
    [RequiredField]
    public SharedGameObject storeValue;

    public override TaskStatus OnUpdate()
    {
        GameObject[] findGos = GameObject.FindGameObjectsWithTag("Nest");

        foreach(var findGo in findGos)
        {
            int buildingId = findGo.GetComponent<Building>().buildingId;
            if (buildingId == nestId.Value)
            {
                storeValue.Value = findGo;
                break;
            } 
        }

        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        nestId.Value = 0;
        storeValue.Value = null;
    }
}
