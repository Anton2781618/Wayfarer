using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckMyGroupe : DecoratorNode
{
    [SerializeField] private OperationWhisGroup operationWhisGroup = OperationWhisGroup.Проверить_все_ли_умерли ;
    public enum OperationWhisGroup
    {
        Проверить_все_ли_умерли
    }

    protected override void OnStart() 
    {
    
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        if(operationWhisGroup == OperationWhisGroup.Проверить_все_ли_умерли)
        {
            int dieSum = 0;
            int group = context.unit.aI.GetMamry().groupMembers.Count;

            for (int i = 0; i < group; i++)
            {
                if(context.unit.aI.GetMamry().groupMembers[i].GetStateNPC() == ICanTakeDamage.States.Мертв)
                {
                    dieSum ++;
                }
                
            }

            if(dieSum == group)
            {
                return child.Update();
            }
        }
        
        return State.Failure;
    }
}
