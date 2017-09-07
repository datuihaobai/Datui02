using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MovementEffects;

[TaskCategory("Datui")]
public class SetSharedVector3Y : Action
{
    public SharedVector3 targetValue;
    public SharedFloat y;

    public override void OnStart()
    {
        targetValue.Value = new Vector3(targetValue.Value.x, y.Value,targetValue.Value.z);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        
    }
}
