using UnityEngine;

public class DebugLogNode : AIActionNode
{
    public string messege = "Новое";
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

        Debug.Log("BlackBoard " + blackboard.moveToPosition);

        blackboard.moveToPosition.x += 1;
        agent.transform.position += new Vector3(1, 0, 0);

        return State.Success;
    }
}
