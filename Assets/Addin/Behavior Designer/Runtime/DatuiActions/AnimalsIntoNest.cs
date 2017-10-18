using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MovementEffects;

[TaskCategory("Datui")]
public class AnimalsIntoNest : Action 
{
    public SharedGameObject targetGameObject;
    private GameObject prevGameObject;
    private Animals animals;

    public override void OnStart()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
            prevGameObject = currentGameObject;

        animals = currentGameObject.GetComponent<Animals>();
        if (animals == null)
            return;
        animals.facade.SetActive(false);
        animals.nestEffect.SetActive(true);
        Timing.RunCoroutine(HideEffect());
    }

    IEnumerator<float> HideEffect()
    {
        yield return Timing.WaitForSeconds(0.5f);
        animals.nestEffect.SetActive(false);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}