using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using static CheckParameters;
using System;

public class MakeADecision : ActionNode
{
    [Tooltip("Увеличивать важность пропорционально")]
    [SerializeField] private bool increaseImportance;
    [SerializeField] private Parameter parameter = Parameter.Здоровье;
    [SerializeField] private SolutionInfo hungerSolution;

    protected override void OnStart() 
    {
        if(!context.unit.solutions.Contains(hungerSolution)) context.unit.solutions.Add(hungerSolution);
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        if(hungerSolution.importance < 100 && increaseImportance)
        {
            
            hungerSolution.importance = Operation(parameter);
        } 
        
        return State.Success;
    }

    private float Operation(Parameter par)
    {

        return par switch
        {
            Parameter.голод => CalculatePercent(context.unit.unitStats.hunger, 100),

            Parameter.Здоровье => CalculatePercent(context.unit.unitStats.curHP,  context.unit.unitStats.maxHP),
            
            Parameter.сон => CalculatePercent(context.unit.unitStats.sleep, 100),
            
            Parameter => throw new ArgumentException("Передан недопустимый аргумент")
        };
    }

    private float CalculatePercent(float parametr, float maxBorder)
    {
        return 100 - (parametr / maxBorder) * 100f;
    }
}
