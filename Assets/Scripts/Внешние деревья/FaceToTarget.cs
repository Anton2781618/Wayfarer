using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class FaceToTarget : Action
    {
        public SharedGameObject target;

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            float angle = Vector3.Angle(target.Value.transform.position - transform.position, transform.forward);

            // if(angle > 20 )
            // {
                FaceToPoint(target.Value.transform.position);

                return TaskStatus.Running;
            // }

            // return TaskStatus.Success;

        }

        private void FaceToPoint(Vector3 point)
        {
            Vector3 direction = (point - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}