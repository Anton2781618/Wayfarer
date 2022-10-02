using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public AINode rootNode;
    public AINode.State treeState = AINode.State.Running;

    public AINode.State Update()
    {
        if(rootNode.state == AINode.State.Running)
        {
            treeState = rootNode.Update();

        }
        
        return treeState;
    }
}
