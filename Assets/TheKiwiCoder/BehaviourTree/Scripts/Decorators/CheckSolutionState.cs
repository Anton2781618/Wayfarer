using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckSolutionState : DecoratorNode
{
    public State stateSolution = State.Success;
    protected override void OnStart() 
    {
    
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        if(context.unit.aI.stateSolution == stateSolution)
        {
            return child.Update();
        }

        return State.Failure;
    }
}
