using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

[TaskCategory("Datui")]
public class DoTweenLookAt : Action
{
    public SharedGameObject targetGameObject;
    public SharedVector3 moveTarget;
    private GameObject prevGameObject;

    public override void OnStart()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
            prevGameObject = currentGameObject;
        currentGameObject.transform.DOLookAt(moveTarget.Value, 0.5f).SetEase(Ease.Linear);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        targetGameObject = null;
    }
}