using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SubtractParameters : ActionNode
{
    [SerializeField] private bool sleep = false;
    [SerializeField] private bool hunger = false;
    [SerializeField] private bool health = false;
    protected override void OnStart() 
    {
        
    }

    protected override void OnStop() 
    {

    }

    protected override State OnUpdate() 
    {
        UnitStats unitStats = context.unit.unitStats;

        if(unitStats.sleep > 0 && sleep) unitStats.sleep --;

        if(unitStats.hunger > 0 && hunger) unitStats.hunger --;

        if(unitStats.curHP > 0 && health) unitStats.hunger --;

        return State.Success;
    }
}
