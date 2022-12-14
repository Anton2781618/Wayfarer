using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    private Animator anim;

    private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIKPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    private float lastPelviusPositionY, lastRightFootPositionY, lastLeftFootPositionY;
    [Header("Cистема позицианирования рук")]
    [SerializeField] private Transform lookpos;
    [Range(0, 1)] [SerializeField] private float WeightPos = 1;

    [Header("Cистема позицианирования ног")]
    [SerializeField] private bool enabledIk;
    [SerializeField] private bool useRotationIk = false;
    [SerializeField] private bool useSpineIk = false;
    [SerializeField] private bool showDebug = true;
    [SerializeField] private LayerMask enviromantLayer;
    [SerializeField] private float pelviusOffset = 0f;
    [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)] [SerializeField] private float raycastDownDystance = 1.5f;
    
    [Range(0, 1)] [SerializeField] private float pelviusUpAndDownSpeed = 0.25f;
    [Range(0, 1)] [SerializeField] private float feedToIkPositionSpeed = 0.5f;

    private string leftFootAnimVariableName = "LeftFootCurve";
    private string rightFootAnimVariableName = "RightFootCurve";

    

   
    private Vector3 lookPos;
     [Header("Позвоночник")]
    [SerializeField] private float lookIkWeight;
    [SerializeField] private float bodyIkWeight;
    [SerializeField] private float headIkWeight;
    [SerializeField] private float eyesIkWeight;
    [SerializeField] private float clampWeight;

    private void Start() 
    {
        anim = GetComponent<Animator>();    
    }

    private void FixedUpdate() 
    {
        if(enabledIk == false) { return; }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if(showDebug)Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 5, Color.red);
        lookPos = ray.GetPoint(10);

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        //найти рейкаст на земле для позиции
        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIkRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation);
    }

    private void Update() 
    {
        if(enabledIk == false) { return; }
    }

    private void OnAnimatorIK(int layerIndex) 
    {
        if(enabledIk == false) { return; }

        if(useSpineIk)
        {
            anim.SetLookAtWeight(lookIkWeight, bodyIkWeight, headIkWeight, eyesIkWeight, clampWeight);
            anim.SetLookAtPosition(lookPos);
        }

        MovePelviusHeight();
        // MovePelviusHeight2();

        //правая стопа ik позиция и поворот
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));

        MoveFeetToIkPoin(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIkRotation, ref lastRightFootPositionY);
        MoveFeetToIkPoin(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);

        if(useRotationIk)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName) / 2);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName) / 2);
        }

        anim.SetIKPositionWeight(AvatarIKGoal.RightHand,WeightPos);
        // anim.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
        anim.SetIKPosition(AvatarIKGoal.RightHand,lookpos.position);
        // anim.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);


    }

    private void MoveFeetToIkPoin(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPosition)
    {
        Vector3 targetIkPosition = anim.GetIKPosition(foot);

        if(positionIkHolder != Vector3.zero)
        {
            targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariable = Mathf.Lerp(lastFootPosition, positionIkHolder.y, feedToIkPositionSpeed);
            targetIkPosition.y += yVariable;

            lastLeftFootPositionY = yVariable;

            targetIkPosition = transform.TransformPoint(targetIkPosition); 

            anim.SetIKRotation(foot, rotationIkHolder);

        }
        anim.SetIKPosition(foot, targetIkPosition);
    }

    //метод двигает корень персонажа в след за коллайдером с ограниченой скоростью
    private void MovePelviusHeight()
    {
        if(rightFootIKPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelviusPositionY == 0)
        {
            lastPelviusPositionY = anim.bodyPosition.y;
            return;
        }

        float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
        float rOffsetPosition = rightFootIKPosition.y - transform.position.y;

        float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

        Vector3 newPelvisePosition = anim.bodyPosition + Vector3.up * totalOffset;

        newPelvisePosition.y = Mathf.Lerp(lastPelviusPositionY, newPelvisePosition.y, pelviusUpAndDownSpeed);

        anim.bodyPosition = newPelvisePosition;

        lastPelviusPositionY = anim.bodyPosition.y;
    }

// [Range(0, 1)] public float HipsWeight = 0.75f;
// float FalloffWeight;
//     private void MovePelviusHeight2()
//     {
//         //Get height
//         float LeftOffset = leftFootIkPosition.y - anim.transform.position.y;
//         float RightOffset = rightFootIKPosition.y - anim.transform.position.y;
//         float TotalOffset = (LeftOffset < RightOffset) ? LeftOffset : RightOffset;

//         //Get hips position
//         Vector3 NewPosition = anim.bodyPosition;
//         float NewHeight = TotalOffset * (HipsWeight * FalloffWeight);
//         lastPelviusPositionY = Mathf.Lerp(lastPelviusPositionY, NewHeight, pelviusUpAndDownSpeed);
//         NewPosition.y += lastPelviusPositionY + pelviusOffset;

//         //Set position
//         anim.bodyPosition = NewPosition;
//     }

    //метод определяет положение ног
    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPosition, ref Quaternion feetIkRotation)
    {
        //секция расчта луча
        RaycastHit feetOutHit;

        if(showDebug)
        {
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDystance + heightFromGroundRaycast), Color.yellow);
        }

        if(Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDystance + heightFromGroundRaycast, enviromantLayer))
        {
            //находим позицию ног из небесной позиции
            feetIkPosition = fromSkyPosition;
            feetIkPosition.y = feetOutHit.point.y + pelviusOffset;
            feetIkRotation = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }

        feetIkPosition = Vector3.zero;
    }

    private void AdjustFeetTarget(ref Vector3 feetPosition, HumanBodyBones foot)
    {
        feetPosition = anim.GetBoneTransform(foot).position;
        feetPosition.y = transform.position.y + heightFromGroundRaycast;
    }
}
