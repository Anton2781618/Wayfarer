using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]


    public class Test : Conditional
    {
        [SerializeField] private SharedGameObject target;

        public override TaskStatus OnUpdate()
        {
                Debug.Log(target.Value.GetComponent<AbstractBehavior>().unitStats.curHP);
            if(target.Value.GetComponent<AbstractBehavior>().unitStats.curHP > 0)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
