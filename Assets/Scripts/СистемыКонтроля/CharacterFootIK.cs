using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootIK : MonoBehaviour
{
    private Animator anim;
    [Range(0, 1)] [SerializeField] private float distanceToGround = 0;

    private void Start() 
    {
        anim = GetComponent<Animator>();    
    }

    private void OnAnimatorIK(int layerIndex) 
    {
        //левая нога
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("LeftFootCurve"));
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("RightFootCurve"));

        RaycastHit hit;
        
        Ray lFootRay = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

        if(Physics.Raycast(lFootRay, out hit, distanceToGround + 1f))
        {
            if(hit.collider != null)
            {
                Vector3 lFootPosition = hit.point;

                lFootPosition.y += distanceToGround;

                //ставим ногу на землю
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, lFootPosition);

                // anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }

        //правая нога
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        Ray rFootRay = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

        if(Physics.Raycast(rFootRay, out hit, distanceToGround + 1f))
        {
            if(hit.collider != null)
            {
                Vector3 rFootPosition = hit.point;

                rFootPosition.y += distanceToGround;

                //ставим ногу на землю
                anim.SetIKPosition(AvatarIKGoal.RightFoot, rFootPosition);

                // anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }

    }
}
