using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SortSolutionsByImportance : ActionNode
{
    protected override void OnStart() 
    {
    
    }

    protected override void OnStop() 
    {
    
    }

    protected override State OnUpdate() 
    {
        AnalyzeImportanceSolutions();
        
        return State.Success;
    }

    //метод анализирует важность решений и сортирует список по важности
    public void AnalyzeImportanceSolutions()
    {
        context.unit.solutions.Sort(SortByImportance);
    }

    private int SortByImportance(SolutionInfo a, SolutionInfo b)
    {
        if(a.importance < b.importance)
        {
            return 1;
        }
        else
        if (a.importance > b.importance)
        {
            return -1;
        }

        return 0;
    }
}
