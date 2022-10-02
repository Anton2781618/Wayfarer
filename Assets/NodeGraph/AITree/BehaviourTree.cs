using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

//класс является представлением скриптбл обджекта дерева с базовыми методами
[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public AINode rootNode;    
    public AINode.State treeState = AINode.State.Running;
    public List<AINode> nodes = new List<AINode>(); 

    public AINode.State Update()
    {
        if(rootNode.state == AINode.State.Running)
        {
            treeState = rootNode.Update();

        }
        
        return treeState;
    }

    public AINode CreateNode(System.Type type)
    {
        AINode node = ScriptableObject.CreateInstance(type) as AINode;
        
        node.name = type.Name;
        
        node.guid = GUID.Generate().ToString();
        
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        
        AssetDatabase.SaveAssets();
        
        return node;
    }

    public void DeleteNode(AINode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
}
