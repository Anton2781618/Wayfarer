using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

public class AINodeView : Node
{
    public Action<AINodeView> OnNodeSelected;
    public AINode node;
    public Port input;
    public Port output;
    public AINodeView(AINode node) : base("Assets/NodeGraph/AITree/BehaviourTreeEditor/NodeVIew.uxml")
    {
        this.node = node;
        this.title= node.name;
        this.viewDataKey = node.guid;


        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
    }

    private void SetupClasses()
    {
        if(node is AIActionNode)
        {
            AddToClassList("action");
        }
        else
        if(node is AICompositNode)
        {
            AddToClassList("composit");
        }
        else
        if(node is AIDecoratorNode)
        {
            AddToClassList("decorator");
        }
        else
        if(node is AIRootNode)
        {
            AddToClassList("root");
        }
    }

    private void CreateInputPorts()
    {
        if(node is AIActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AICompositNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AIDecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AIRootNode)
        {

        }

        if(input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }
    private void CreateOutputPorts()
    {
        if(node is AIActionNode)
        {
            
        }
        else
        if(node is AICompositNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else
        if(node is AIDecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AIRootNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if(output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    //метод устанавливает позицию ноды 
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        //этот метод делает отмену ctrl+z
        Undo.RecordObject(node, "Behaviour Tree (Set Position)");

        node.position.x = newPos.xMin;
        
        node.position.y = newPos.yMin;

        //этот метод сохраняет доанные после сборки.. хз для чего он 
        EditorUtility.SetDirty(node);
    }

    //метод срабатывает когда мы выбираем ноду
    public override void OnSelected()
    {
        base.OnSelected();

        if(OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    //метод сортирует дочерние элементы в списке при физическом перемещении нодав
    public void SortChildren()
    {
        AICompositNode composit = node as AICompositNode;

        if(composit)
        {
            composit.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(AINode left, AINode right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if(Application.isPlaying)
        {
            switch(node.state)
            {
                case AINode.State.Running:
                if(node.started)
                {
                    AddToClassList("running");
                }
                break;

                case AINode.State.Failure:
                    AddToClassList("failure");
                break;
                
                case AINode.State.Success:
                    AddToClassList("success");
                break;
            }
        }
    }
}
