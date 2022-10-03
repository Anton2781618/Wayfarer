using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class AINodeView : Node
{
    public Action<AINodeView> OnNodeSelected;
    public AINode node;
    public Port input;
    public Port output;
    public AINodeView(AINode node)
    {
        this.node = node;
        this.title= node.name;
        this.viewDataKey = node.guid;


        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
    }

    private void CreateInputPorts()
    {
        if(node is AIActionNode)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AICompositNode)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AIDecoratorNode)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AIRootNode)
        {

        }

        if(input != null)
        {
            input.portName = "";
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
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else
        if(node is AIDecoratorNode)
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else
        if(node is AIRootNode)
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if(output != null)
        {
            output.portName = "";
            outputContainer.Add(output);
        }
    }

    //метод устанавливает позицию ноды 
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
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
}
