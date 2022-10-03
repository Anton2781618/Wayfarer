using UnityEngine;

public class AIRootNode : AINode
{
    public AINode child;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    public override AINode Clone()
    {
        AIRootNode node = Instantiate(this);

        node.child = child.Clone();

        return node;
    }
}
