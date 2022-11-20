using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]
    public class Eyes : Conditional
    {
        [SerializeField] private SharedGameObject returnedObject;
        private Transform headTransform;
        [SerializeField] private float viewRadiusEyes;
        [SerializeField] [Range(0, 360)] private float viewAngleEyes;
        [SerializeField] private LayerMask targetMaskForEyes;
        [SerializeField] private LayerMask obstaclMaskForEyes;
        [SerializeField] private bool useMamry = true;
        public List<Transform> mamryTargets = new List<Transform>();
        public List<Transform> visileTargets = new List<Transform>();
        
        public override void OnAwake()
        {
            headTransform = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        }

        public override TaskStatus OnUpdate()
        {
            FirndVisiblaTargets();

            if(useMamry)
            {
                if(FindTargetInMamry()) return TaskStatus.Success;
            }
            else
            {
                if(FindTargetInVisileTargets()) return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public void SetTargetMaskForEyes(LayerMask value)
        {
            targetMaskForEyes = value;
        }

        //найти таргеты в зоне видимости
        public void FirndVisiblaTargets()
        {        
            visileTargets.Clear();

            Collider[] targetsInViewRadius = Physics.OverlapSphere(headTransform.position, viewRadiusEyes, targetMaskForEyes);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform newTarget = targetsInViewRadius[i].transform;
                
                Vector3 dirToTarget = (newTarget.position - headTransform.position).normalized;

                if(Vector3.Angle(headTransform.forward, dirToTarget) < viewAngleEyes / 2)
                {
                    float distToTarget = Vector3.Distance(transform.position, newTarget.position);

                    if(!Physics.Raycast(headTransform.position, dirToTarget, distToTarget, obstaclMaskForEyes))
                    {
                        visileTargets.Add(newTarget);
                    }
                }
            }        

            foreach (var tar in visileTargets)
            {
                Debug.DrawLine(headTransform.position, tar.position, Color.red);
            }
        }

        //проверить есть ли объект в памяти
        public bool FindTargetInMamry(Transform visileTarget)
        {
            for (int i = 0; i < mamryTargets.Count; i++)
            {
                if(visileTarget == mamryTargets[i]) return true;
            }                

            return false;
        }

        public Vector3 DirFromAngle(float angleInDegriees, bool angleIsGlobal)
        {
            if(!angleIsGlobal)
            {
                angleInDegriees += headTransform.eulerAngles.y;
            }
            
            return new Vector3(Mathf.Sin(angleInDegriees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegriees * Mathf.Deg2Rad));
        }

        
        //метод запоминает увиденые объекты и после устанавливает ближайший к себе в качестве цели
        private bool FindTargetInMamry()
        {
            if(visileTargets.Count > 0)
            {
                foreach (var item in visileTargets)
                {
                    if(!FindTargetInMamry(item))
                    {
                        mamryTargets.Add(item);
                    }
                }
            }

            if(mamryTargets.Count > 0)
            {
                
                if(mamryTargets[0] == null || (targetMaskForEyes & (1 << mamryTargets[0].gameObject.layer)) == 0)
                {
                    mamryTargets.RemoveAt(0);

                    return false;
                }
                
                mamryTargets = mamryTargets.OrderBy((d) => (d.position - transform.position).sqrMagnitude).ToList();

                returnedObject.Value = mamryTargets[0].gameObject;

                return true;
            }

            return false;
        }

        //метод выбирает ближайший объект из тех что видет сейчас и после устанавливает ближайший к себе в качестве цели
        private bool FindTargetInVisileTargets()
        {
            if(visileTargets.Count > 0)
            {
                visileTargets = visileTargets.OrderBy((d) => (d.position - transform.position).sqrMagnitude).ToList();
                
                returnedObject.Value = visileTargets[0].gameObject;
                
                return true;
            }
            
            return false;
        }

        //найти ближайший объект
        private Transform FindNearestObject()
        {
            Transform buferTarget = mamryTargets[0];

            foreach (var item in mamryTargets)
            {
                if(Vector3.Distance(item.position, transform.position) < Vector3.Distance(buferTarget.position, transform.position))
                {
                    buferTarget = item;
                }
            }
            
            return buferTarget;
        }
    }
}
