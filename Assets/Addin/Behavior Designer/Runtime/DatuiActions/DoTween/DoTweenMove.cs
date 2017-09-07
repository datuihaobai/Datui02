using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

[TaskCategory("Datui")]
public class DoTweenMove : Action
{
    public SharedGameObject targetGameObject;
    public SharedVector3 moveTarget;
    public SharedFloat speed;
    private GameObject prevGameObject;
    private bool moveComplete;

    public override void OnStart()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
            prevGameObject = currentGameObject;
        moveComplete = false;
        float duration = Vector3.Distance(currentGameObject.transform.position,moveTarget.Value) / speed.Value;
        currentGameObject.transform.DOMove(moveTarget.Value, duration).
            SetEase(Ease.Linear).OnComplete(() => {
                moveComplete = true;
        });
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override TaskStatus OnUpdate()
    {
        if (moveComplete)
            return TaskStatus.Success;
        else
            return TaskStatus.Running;
    }

    public override void OnReset()
    {
        targetGameObject = null;
        moveComplete = false;
    }
}