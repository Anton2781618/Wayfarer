using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeView : UnityEditor.Experimental.GraphView.Node
{
    public AINode node;
    public AINodeView(AINode node)
    {
        this.node = node;
        this.title= node.name;
        this.viewDataKey = node.guid;


        style.left = node.position.x;
        style.left = node.position.y;
    }

    //метод устанавливает позицию ноды 
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
    }
}
