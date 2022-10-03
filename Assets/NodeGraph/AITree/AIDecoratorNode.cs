using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIDecoratorNode : AINode
{
    public AINode child;

    public override AINode Clone()
    {
        AIDecoratorNode node = Instantiate(this);

        node.child = child.Clone();

        return node;
    }
}

