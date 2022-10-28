using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Тест : ActionNode
{
    public int num = 1000;
    protected override void OnStart() 
    {
        num = 1000;
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        if(num > 0)
        {
            num --;

            return State.Running;
        }
        
        if(num <= 0)
        {
            return State.Success;
        }
        
        return State.Running;
    }
}
