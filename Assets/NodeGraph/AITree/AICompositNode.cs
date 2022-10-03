using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICompositNode : AINode
{
    public List<AINode> children = new List<AINode>();

    public override AINode Clone()
    {
        AICompositNode node = Instantiate(this);

        node.children = children.ConvertAll(c => c.Clone());

        return node;
    }
}
