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
        Сон, 
        Голод,
        Видит_объектов
    }
    public enum UnitOperation
    {
        Больше,
        Меньше,
        Равно,
        Не_равно
    }

    public Parameter parameter = Parameter.Здоровье;    
    public UnitOperation operation = UnitOperation.Больше;    
    public int minimumValue = 80;
    
    protected override void OnStart() 
    {
    }

    protected override void OnStop() 
    {
    }

    protected override State OnUpdate() 
    {
        if(DoOperation(parameter))
        {
            return child.Update();
        }

        return State.Failure;
        
    }

    private bool DoOperation(Parameter par)
    {
        return par switch
        {
            Parameter.Голод => Calculate(context.unit.unitStats.hunger, operation),

            Parameter.Здоровье => Calculate(context.unit.unitStats.curHP, operation),
            
            Parameter.Сон => Calculate(context.unit.unitStats.sleep, operation),

            Parameter.Видит_объектов => Calculate(context.unit.aI.GetEyes().visileTargets.Count, operation),
            
            Parameter => throw new ArgumentException("Передан недопустимый аргумент")
        };
    }

    bool Calculate(int atribut, UnitOperation operation)
    {
        return operation switch
        {
            UnitOperation.Больше => atribut > minimumValue,
        
            UnitOperation.Меньше => atribut > minimumValue,

            UnitOperation.Равно => atribut == minimumValue,

            UnitOperation.Не_равно => atribut != minimumValue,
        
            UnitOperation => throw new ArgumentException("Передан недопустимый аргумент")
        };

        
    }
}
