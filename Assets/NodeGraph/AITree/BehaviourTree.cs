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

    //метод создает ноду
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

    //метод удаляет ноду
    public void DeleteNode(AINode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    //метод добавляет ребро в ноде
    public void AddChild(AINode parent, AINode child)
    {
        AIDecoratorNode decorator = parent as AIDecoratorNode;
        
        if(decorator)
        {
            decorator.child = child;
        }

        AIRootNode rootnode = parent as AIRootNode;
        
        if(rootnode)
        {
            rootnode.child = child;
        }

        AICompositNode composit = parent as AICompositNode;
        
        if(composit)
        {
            composit.children.Add(child);
        }
    }

    //метод уберает ребро в ноде
    public void RemoveChild(AINode parent, AINode child)
    {
        AIDecoratorNode decorator = parent as AIDecoratorNode;
        
        if(decorator)
        {
            decorator.child = null;
        }

        AIRootNode rootNode = parent as AIRootNode;
        
        if(rootNode)
        {
            rootNode.child = null;
        }

        AICompositNode composit = parent as AICompositNode;
        
        if(composit)
        {
            composit.children.Remove(child);
        }
    }

    // метод возвращяет ребра ноды 
    public List<AINode> GetChildren(AINode parent)
    {
        List<AINode> children = new List<AINode>();
        
        AIDecoratorNode decorator = parent as AIDecoratorNode;
        
        if(decorator && decorator.child != null)
        {
            children.Add(decorator.child);
        }

        AIRootNode rootNode = parent as AIRootNode;
        
        if(rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }

        AICompositNode composit = parent as AICompositNode;
        
        if(composit)
        {
            return composit.children;
        }

        return children;
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);

        tree.rootNode = tree.rootNode.Clone();

        return tree;
    }
}
