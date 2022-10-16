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

    public BlackBoard blackboard = new BlackBoard();

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
        
        Undo.RecordObject(this, "Behaviour Tree (добавить)");

        nodes.Add(node);

        if(!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (добавить)");

        AssetDatabase.SaveAssets();
        
        return node;
    }

    //метод удаляет ноду
    public void DeleteNode(AINode node)
    {
        Undo.RecordObject(this, "Behaviour Tree (удалить)");

        nodes.Remove(node);

        Undo.DestroyObjectImmediate(node);
        
        AssetDatabase.SaveAssets();
    }

    //метод добавляет ребро в ноде
    public void AddChild(AINode parent, AINode child)
    {
        AIDecoratorNode decorator = parent as AIDecoratorNode;        
        if(decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (добавить)");
            decorator.child = child;
            EditorUtility.SetDirty(decorator);
        }

        AIRootNode rootnode = parent as AIRootNode;        
        if(rootnode)
        {
            Undo.RecordObject(rootnode, "Behaviour Tree (добавить)");
            rootnode.child = child;
            EditorUtility.SetDirty(rootNode);
        }

        AICompositNode composit = parent as AICompositNode;        
        if(composit)
        {
            Undo.RecordObject(composit, "Behaviour Tree (добавить)");
            composit.children.Add(child);
            EditorUtility.SetDirty(composit);
        }
    }

    //метод уберает ребро в ноде
    public void RemoveChild(AINode parent, AINode child)
    {
        AIDecoratorNode decorator = parent as AIDecoratorNode;
        
        if(decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (удалить)");
            decorator.child = null;
            EditorUtility.SetDirty(decorator);
        }

        AIRootNode rootNode = parent as AIRootNode;
        
        if(rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (удалить)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }

        AICompositNode composit = parent as AICompositNode;
        
        if(composit)
        {
            Undo.RecordObject(composit, "Behaviour Tree (удалить)");
            composit.children.Remove(child);
            EditorUtility.SetDirty(composit);
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

    //метод проходит в глубину, начиная с корневого элемента
    public void Travers(AINode node, System.Action<AINode> visiter)
    {
        if(node)
        {
            visiter.Invoke(node);

            var children = GetChildren(node);
            
            children.ForEach((n)=> Travers(n, visiter));
        }
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);

        tree.rootNode = tree.rootNode.Clone();

        tree.nodes = new List<AINode>();

        Travers(tree.rootNode, (n)=>
        {
            tree.nodes.Add(n);
        });

        return tree;
    }

    //метод назначает скрипт моба каждому узлу
    public void Bind(AbstractBehavior agent)
    {
        Travers(rootNode, node =>
        {
            node.agent = agent;
            
            node.blackboard = blackboard;
        });
    }
}
