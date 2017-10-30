using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using MovementEffects;

[TaskCategory("Datui")]
public class CheckMoveTarget : Action
{
    public SharedGameObject targetGameObject;
    private GameObject prevGameObject;
    private Animals animals;
    public SharedVector3 storeHitPoint;

    public override TaskStatus OnUpdate()
    {
        GameObject terrainGo = GameObject.FindGameObjectWithTag("SurfaceTerrain");
        PATileTerrain tileTerrain = terrainGo.GetComponent<PATileTerrain>();

        Vector3 pos = tileTerrain.transform.InverseTransformPoint(storeHitPoint.Value);
        int x = (int)Mathf.Abs(pos.x / tileTerrain.tileSize);
        int y = (int)Mathf.Abs(pos.z / tileTerrain.tileSize);
        PATileTerrain.PATile tile = tileTerrain.GetTile(x, y);

        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        if (currentGameObject != prevGameObject)
            prevGameObject = currentGameObject;

        animals = currentGameObject.GetComponent<Animals>();
        if (animals == null)
            return TaskStatus.Failure;

        if (animals.elementType == Animals.ElementType.Fire)
        {
            if (tile.element.FireValue > 0 && tile.element.WoodValue == 0)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
        else if (animals.elementType == Animals.ElementType.Wood)
        {
            if (tile.element.WoodValue > 0 && tile.element.FireValue == 0)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        return TaskStatus.Success;
    }
}