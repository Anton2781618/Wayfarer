using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class MoveToPoint : Conditional
    {
        [SerializeField] private float speedDampTime;
        [SerializeField] private SharedGameObject target;
        [SerializeField] private SharedVector3 targetPosition;
        private Animator animator;
        private UnityEngine.AI.NavMeshAgent agent;
        private float speed;
        private int m_SpeedId = 0;


        public override void OnAwake()
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            animator = GetComponent<Animator>();

            agent.updateRotation = false;

            m_SpeedId = Animator.StringToHash("Run Blend");
        }

        public override void OnStart()
        {
            if(target.Value != null )
            {
                agent.SetDestination(target.Value.transform.position);
            }
        }

        

        public override TaskStatus OnUpdate()
        {
            if(target.Value == null )
            {
                return TaskStatus.Failure;
            }

            if (HasArrived()) 
            {
                animator.SetFloat(m_SpeedId, 0, speedDampTime, Time.deltaTime);

                if(animator.GetFloat(m_SpeedId) <= 0.3) return TaskStatus.Success;
            }

            speed = agent.desiredVelocity.magnitude;

            if(agent.path.corners.Length > 1)
            {
                FaceToPoint(agent.path.corners[1]);
            }

            animator.SetFloat(m_SpeedId, speed, speedDampTime, Time.deltaTime);

            return TaskStatus.Running;
        }

        private void FaceToPoint(Vector3 point)
        {
            Vector3 direction = (point - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        private bool HasArrived()
        {
            agent.SetDestination(Target());
         
            float remainingDistance;
            
            if (agent.pathPending) 
            {
                remainingDistance = float.PositiveInfinity;
            }
            else
            {
                remainingDistance = agent.remainingDistance;
            }

            return remainingDistance <= agent.stoppingDistance;
        }
        private Vector3 Target()
        {
            if (target.Value != null) 
            {
                return target.Value.transform.position;
            }

            return targetPosition.Value;
        }
    }
}
