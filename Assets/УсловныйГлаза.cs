using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class УсловныйГлаза : DecoratorNode
{
    protected override void OnStart() 
    {
    
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        if(context.unit.aI.GetEyes().visileTargets.Count > 0)
        {
            child.Update();
            
            return State.Success;
        }

        return State.Failure;
    }
}
