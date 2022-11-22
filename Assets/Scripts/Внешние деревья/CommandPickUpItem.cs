using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class CommandPickUpItem : Action
    {
        public SharedGameObject target;
        private AbstractBehavior unit;
        private Animator anim;
        private UnityEngine.AI.NavMeshAgent agent;


        public override void OnStart()
        {
            anim = GetComponent<Animator>();
            
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            
            unit = GetComponent<AbstractBehavior>();
        }


        public override TaskStatus OnUpdate()
        {
            PickUpItem();

            return TaskStatus.Success;
        }

        private void PickUpItem()
        {
            target.Value.GetComponent<ICanUse>().Use(unit);
        }
    }
}
