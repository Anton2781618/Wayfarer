using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]


    public class Test : Action
    {
        public SharedGameObject target;
        public Animator animator;
        public UnityEngine.AI.NavMeshAgent agent;
        public float speed;
        public float speedDampTime;

        public int m_SpeedId = 0;

        public override void OnAwake()
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            animator = GetComponent<Animator>();

            // agent.updateRotation = false;

            m_SpeedId = Animator.StringToHash("Run Blend");

            
        }

         public override void OnStart()
        {
            // agent.angularSpeed = angularSpeed.Value;
        #if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                    navMeshAgent.Resume();
        #else
                    agent.isStopped = false;
        #endif
            agent.SetDestination(target.Value.transform.position);
        }

        public override TaskStatus OnUpdate()
        {
            
            if (HasArrived()) 
            {
                Debug.Log("stop");

                return TaskStatus.Success;
            }

            speed = agent.desiredVelocity.magnitude;

            agent.SetDestination(target.Value.transform.position);

            animator.SetFloat(m_SpeedId, speed, speedDampTime, Time.deltaTime);

            return TaskStatus.Running;
        }
        

        private void OnAnimatorMove()
		{
			agent.velocity = animator.deltaPosition / Time.deltaTime;

			// transform.rotation = animator.rootRotation;
		}

        private bool HasArrived()
        {
            // The path hasn't been computed yet if the path is pending.
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

               
    }
}
