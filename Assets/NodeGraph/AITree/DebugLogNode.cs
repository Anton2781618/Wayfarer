using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogNode : AIActionNode
{
    public string messege;
    protected override void OnStart()
    {
        Debug.Log("OnStart " + messege);
    }

    protected override void OnStop()
    {
        Debug.Log("OnStop " + messege);
    }

    protected override State OnUpdate()
    {
        Debug.Log("OnUpdate " + messege);

        return State.Success;
    }
}
