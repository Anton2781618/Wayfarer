using UnityEngine;


namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]

    public class AttackBehavior : Action
    {
        [SerializeField] private Tactics tactics = Tactics.Одиночный_удар;

        [SerializeField] private SharedGameObject target;

        [SerializeField] private string[] attaksArray;

        public enum Tactics
        {
            Одиночный_удар,
            Блок,
        }
     
        private Animator anim;
        private UnityEngine.AI.NavMeshAgent agent;

        // public float cooldown = 2f;
        private float nextHit = 0f;

        public override void OnStart()
        {
            anim = GetComponent<Animator>();
            
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            if(tactics == Tactics.Одиночный_удар)
            {
                Block();
                RandomHit();
            }
            else
            if(tactics == Tactics.Блок)
            {
                Block();
            }

            

            if(IsAttackTarget(gameObject))
            {
                foreach (var clipInfo in gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0))
                    {
                        Debug.Log(clipInfo.clip.name);
                    }
                return TaskStatus.Running;
            } 
            else
            {
                return TaskStatus.Success;

            }

        }

        private void RandomHit()
        {
            anim.SetTrigger(attaksArray[Random.Range(0, attaksArray.Length - 1)]);
        }

        private void Block()
        {
            if(!IsAttackTarget(target.Value)) return;

            anim.SetTrigger("Block");
        }
        
        private bool IsAttackTarget(GameObject targetAttack)
        {
            
            foreach (var attackName in attaksArray)
            {
                if(targetAttack.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(attackName))
                {
                    return true;
                }
            }

            return false;
        }

        private void FaceToPoint(Vector3 point)
        {
            Vector3 direction = (point - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

    }
}
