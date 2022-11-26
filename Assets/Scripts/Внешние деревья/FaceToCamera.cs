using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    [TaskCategory("Tutorial")]
    public class FaceToCamera : Action
    {
        [SerializeField] protected Canvas unitCanvas;
        public override TaskStatus OnUpdate()
        {
            if(unitCanvas.gameObject.activeSelf)RotateCanvas();

            return TaskStatus.Success;
        }

        //поварачивает канвас юнита лицом к игроку
    private void RotateCanvas()
    {
        if(unitCanvas.transform.rotation != Camera.main.transform.rotation)
        {
            unitCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }
    }
}