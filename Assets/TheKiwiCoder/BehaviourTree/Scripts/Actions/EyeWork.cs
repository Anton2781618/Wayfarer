using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class EyeWork : ActionNode
{
    protected override void OnStart() 
    {
    
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        context.unit.aI.GetEyes().FirndVisiblaTargets();
        
        if(context.unit.aI.GetEyes().visileTargets.Count == 0)return State.Failure;

        return State.Success;
    }
}
