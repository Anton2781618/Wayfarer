using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class AttackBehavior : Action
    {
        public SharedGameObject target;

        private Animator anim;
        private UnityEngine.AI.NavMeshAgent agent;

        public float cooldown = 2f;
        public float nextHit = 0f;

        public override void OnAwake()
        {
            anim = GetComponent<Animator>();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }


        public override TaskStatus OnUpdate()
        {
            if(Vector3.Distance(transform.position, target.Value.transform.position) <= agent.stoppingDistance)
            {
                FaceToPoint(target.Value.transform.position);

                Attack();

                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        private void Attack()
        {
            if(Time.time >= nextHit)
            {
                nextHit = Time.time + cooldown;

                anim.SetTrigger("Hit");
            }
        }

        private void FaceToPoint(Vector3 point)
        {
            Vector3 direction = (point - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

    }
}
