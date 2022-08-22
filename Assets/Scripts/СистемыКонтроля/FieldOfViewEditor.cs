using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (Unit))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI() 
    {
        
        Unit fow = (Unit)target;
        Transform head = fow.anim.GetBoneTransform(HumanBodyBones.Head);
        Handles.color = Color.white;    
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(head.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(head.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.visileTargets)
        {
            Handles.DrawLine(fow.anim.GetBoneTransform(HumanBodyBones.Head).position, visibleTarget.transform.position);
        }
    }
}
