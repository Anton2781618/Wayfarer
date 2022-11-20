using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]


    public class Test : Action
    {
        public SharedGameObject target;

        private Animator anim;
        private UnityEngine.AI.NavMeshAgent agent;

        public override void OnAwake()
        {
            anim = GetComponent<Animator>();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            
            anim.SetTrigger("Hit");

            return TaskStatus.Success;
        }
    }
}
