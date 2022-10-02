using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;

//класс является представлением дерва
public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits>{} 
    BehaviourTree tree;
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeGraph/AITree/BehaviourTreeEditor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet); 
    }

    //метод заполняет ппредставление дерева
    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;

        DeleteElements(graphElements);

        graphViewChanged += OnGraphViewChanged;

        tree.nodes.ForEach(n => CreateNodeView(n));
    }

    //не понятно как, но как то удаляет ноды из списка
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem => 
            {
                AINodeView nodeView = elem as AINodeView;

                if(nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }
            });
        }
        return graphViewChange;
    }

    //метод создает контекстное меню
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //дает базовые спасобности
        base.BuildContextualMenu(evt);

        {
            var types = TypeCache.GetTypesDerivedFrom<AIActionNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a)=> CreateNode(type));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<AICompositNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a)=> CreateNode(type));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<AIDecoratorNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a)=> CreateNode(type));
            }
        }
    }

    private void CreateNode(System.Type type)
    {
        AINode node = tree.CreateNode(type);

        CreateNodeView(node);
    }

    private void CreateNodeView(AINode node)
    {
        AINodeView nodeView = new AINodeView(node);
        AddElement(nodeView);
    }
}
