using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class StartSolution : ActionNode
{
    protected override void OnStart() 
    {
    }

    protected override void OnStop() 
    {
    }

    protected override State OnUpdate() 
    {
        if(context.unit.aI.currentSolution != context.unit.solutions[0])
        {
            context.unit.aI.currentSolution = context.unit.solutions[0];

            context.unit.aI.StartSolution();
        } 

        context.unit.ExecuteCurrentCommand();

        return State.Success;
    }
}
