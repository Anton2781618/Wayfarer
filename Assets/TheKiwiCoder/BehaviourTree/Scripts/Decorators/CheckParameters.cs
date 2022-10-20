using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;

public class CheckParameters : DecoratorNode
{
    public enum Parameter
    {
        Здоровье, 
        сон, 
        голод
    }

    public Parameter parameter = Parameter.Здоровье;    
    public int minimumValue = 80;
    
    protected override void OnStart() 
    {
    }

    protected override void OnStop() 
    {
    }

    protected override State OnUpdate() 
    {
        return DoOperation(parameter);
    }

    private State DoOperation(Parameter par)
    {
        State result = par switch
        {
            Parameter.голод => Calculate(context.unit.unitStats.hunger),

            Parameter.Здоровье => Calculate(context.unit.unitStats.curHP),
            
            Parameter.сон => Calculate(context.unit.unitStats.sleep),
            
            Parameter => throw new ArgumentException("Передан недопустимый аргумент")
        };
        return result;
    }

    private State Calculate(float atribut)
    {
        if(atribut <= minimumValue)
        {
            return child.Update();
        }
        
        return State.Failure;
    }
}
