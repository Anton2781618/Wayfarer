using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]


    public class Test : Action
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Inactive;
        }
    }
}
