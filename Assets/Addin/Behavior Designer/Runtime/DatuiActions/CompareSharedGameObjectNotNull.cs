using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    [TaskCategory("Datui")]
    public class CompareSharedGameObjectNoNull : Conditional
    {
        public SharedGameObject variable;

        public override TaskStatus OnUpdate()
        {
            return variable.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            variable = null;
        }
    }
}