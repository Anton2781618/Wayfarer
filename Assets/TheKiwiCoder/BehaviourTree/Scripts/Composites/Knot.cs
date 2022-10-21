using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Knot : CompositeNode
{
    protected override void OnStart() 
    {
    
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        foreach (var child in children)
        {
            child.Update();
        }

        return State.Success;
    }
}
