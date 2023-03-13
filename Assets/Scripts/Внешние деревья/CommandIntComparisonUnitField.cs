using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class CommandIntComparisonUnitField : Conditional
    {
        [SerializeField] private SharedGameObject target;
        [SerializeField] private Mod mod;

        private enum Mod
        {
            Проверить_здоровье,
        }
        public override TaskStatus OnUpdate()
        {
            if(mod == Mod.Проверить_здоровье && target.Value.GetComponent<AbstractBehavior>().unitStats.curHP > 0)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
