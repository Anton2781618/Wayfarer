using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

//класс является представлением дерва
public class BehaviourTreeView : GraphView
{
    public Action<AINodeView> OnNodeSelected;
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

    //хз че за метод походу возвращает представление ноды по предку
    private AINodeView FindNodeView(AINode node)
    {
        return GetNodeByGuid(node.guid) as AINodeView;
    }

    //метод заполняет ппредставление дерева
    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;

        DeleteElements(graphElements);

        graphViewChanged += OnGraphViewChanged;

        //создаем ноды
        tree.nodes.ForEach(n => CreateNodeView(n));

        //создаем ребра
        tree.nodes.ForEach(n => 
        {
            var children = tree.GetChildren(n);

            children.ForEach(c => 
            {
                AINodeView parentView = FindNodeView(n);
                AINodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    //метод берет какие то порты! хер его знает  
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    //срабатывает каждый раз когда граф изменяется. Не понятно как, но как то удаляет ноды из списка
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

                Edge edge = elem as Edge;
                if(edge != null)
                {
                    AINodeView parentview = edge.output.node as AINodeView; 

                    AINodeView childview = edge.input.node as AINodeView; 

                    tree.RemoveChild(parentview.node, childview.node);
                }
            });
        }

        if(graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                AINodeView parentview = edge.output.node as AINodeView; 

                AINodeView childview = edge.input.node as AINodeView; 

                tree.AddChild(parentview.node, childview.node);
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

        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
    }
}
