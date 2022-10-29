using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class OutputMessage : ActionNode
{
    public string massage;
    protected override void OnStart() 
    {
    
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        if(context.unit.textMesh.transform.rotation != Camera.main.transform.rotation)
        {
            context.unit.textMesh.transform.rotation = Camera.main.transform.rotation;
        }

        context.unit.textMesh.text = massage;

        return State.Success;
    }
}
