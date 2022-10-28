using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using static ICanTakeDamage;

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
        Unit unit = context.unit;

        if(unit.GetStateNPC() == States.Мертв) return State.Failure;
        
        if(unit.solutions.Count > 0 && context.unit.aI.currentSolution != context.unit.solutions[0])
        {
            unit.aI.currentSolution = context.unit.solutions[0];

            unit.aI.StartSolution();
        } 

        unit.ExecuteCurrentCommand();

        return State.Success;
    }
}
