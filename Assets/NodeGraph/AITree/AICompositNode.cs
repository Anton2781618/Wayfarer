using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICompositNode : AINode
{
    public List<AINode> children = new List<AINode>();
}
