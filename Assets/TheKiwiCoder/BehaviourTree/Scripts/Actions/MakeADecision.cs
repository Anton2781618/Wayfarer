using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using static CheckParameters;
using System;

public class MakeADecision : ActionNode
{
    [Tooltip("Нужно ли увеличивать важность пропорционально")]
    [SerializeField] private bool increaseImportance;
    [SerializeField] private Parameter parameter = Parameter.Здоровье;
    [SerializeField] private SolutionInfo solution;

    protected override void OnStart() 
    {
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        if(!context.unit.solutions.Contains(solution)) context.unit.solutions.Add(solution);
        
        if(solution.importance < 100 && increaseImportance)
        {            
            solution.importance = Operation(parameter);
            
        } 
        
        return State.Success;
    }

    private float Operation(Parameter par)
    {

        return par switch
        {
            Parameter.Голод => CalculatePercent(context.unit.unitStats.hunger, 100),

            Parameter.Здоровье => CalculatePercent(context.unit.unitStats.curHP,  context.unit.unitStats.maxHP),
            
            Parameter.Сон => CalculatePercent(context.unit.unitStats.sleep, 100),
            
            Parameter => throw new ArgumentException("Передан недопустимый аргумент")
        };
    }

    private float CalculatePercent(float parametr, float maxBorder)
    {
        return 100 - (parametr / maxBorder) * 100f;
    }
}
