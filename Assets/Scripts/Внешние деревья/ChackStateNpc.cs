using UnityEngine;
using static ICanTakeDamage;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class ChackStateNpc : Conditional
    {
        private Unit unit;
        
        public override void OnStart()
        {
            unit = GetComponent<Unit>();
        }
        public override TaskStatus OnUpdate()
        {
            if(unit.GetStateNPC() != States.Мертв)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}