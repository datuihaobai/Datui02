using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MovementEffects;

[TaskCategory("Datui")]
public class MoveForword : Action
{
    public SharedGameObject targetGameObject;
    public SharedFloat delay;
    public SharedFloat distance;
    public SharedFloat duration;
    private GameObject prevGameObject;
    private bool moveComplete;

    public override void OnStart()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
            prevGameObject = currentGameObject;
        Timing.RunCoroutine(MoveByCurve());
    }

    IEnumerator<float> MoveByCurve()
    {
        moveComplete = false;
        yield return Timing.WaitForSeconds(delay.Value);
        Vector3 startPos = prevGameObject.transform.position;
        Vector3 endPos = prevGameObject.transform.forward * distance.Value + prevGameObject.transform.position;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            prevGameObject.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime);
            normalizedTime += Time.deltaTime / duration.Value;
            yield return Timing.WaitForOneFrame;
        }
        moveComplete = true;
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
