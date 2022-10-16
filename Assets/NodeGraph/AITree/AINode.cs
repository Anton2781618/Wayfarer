using UnityEngine;

public abstract class AINode : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public BlackBoard blackboard;
    [HideInInspector] public AbstractBehavior agent;

    public State Update() 
    {
        if(!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();

        if(state == State.Failure ||state == State.Success)
        {
            OnStop();
            started = false;
        }

        return state;
    }

    public virtual AINode Clone()
    {
        return Instantiate(this);
    }
    
    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
