using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using static ICanTakeDamage;

public class StartSolution : ActionNode
{
    Unit unit;    
    protected override void OnStart() 
    {
        unit = context.unit;
    }

    protected override void OnStop() 
    {
    }

    protected override State OnUpdate() 
    {
        if(unit.GetStateNPC() == States.Мертв) return State.Failure;

        if(unit.solutions.Count > 0 && unit.aI.currentSolution != unit.solutions[0])
        {
            unit.aI.currentSolution = unit.solutions[0];

            unit.aI.StartSolution();
        }

        unit.ExecuteCurrentCommand();

        return State.Running;

    }
}
