using System;
using DS;
using DS.Enumerations;
using UnityEngine;
using static ICanTakeDamage;

public class Unit : AbstractBehavior
{
    private delegate int Operation();
    [SerializeField] private DSDialogue dSDialogue;
    private void Update() 
    {
        Controller();
        if(unitCanvas.gameObject.activeSelf)RotateCanvas();
    }

    public int DelegatOperation(DSAction action)
    {
        Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, action.ToString());
        return operation.Invoke();

    }
    public void RotateCanvas()
    {
        if(unitCanvas.transform.rotation != Camera.main.transform.rotation)
        {
            unitCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public States GetUnityState()
    {
        return state;
    }
    
    private void Controller()
    {
        if( agent.isActiveAndEnabled && agent.remainingDistance <= agent.stoppingDistance && target != null)
        {
            // if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Tree"))
            // {
                FaceTarget();    
            // }
        }

        if(state == States.Атака && target != null)
        {
            ArttackTarget();
            
        }
    }

    public void SetTarget(ICanUse newTarget)
    {
        target = newTarget;
    }

    

    public void AnimationStates()
    {
        anim.SetBool("walk", agent.remainingDistance > agent.stoppingDistance);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //метод команда атакавать таргет (привязана к системе диалогов)
    public int CommandAttackTheTarget()
    {
        if(target == null)
        {
            Debug.Log("У " + transform.name + " не установлен таргет");
            return 0;
        }
        AbstractBehavior buferTarget = target as AbstractBehavior;
       
        if(buferTarget.GetCurrentUnitState() != States.Мертв)
        {
            state = States.Атака;
        }
        return 0;
    }

    private void ArttackTarget()
    {
        AbstractBehavior buferTarget = target as AbstractBehavior;

        if(buferTarget.GetCurrentUnitState() == States.Мертв)
        {
            state = States.Патруль;
            return;
        }
        
        AnimationStates();
        agent.SetDestination(target.transform.position);

        // agent.enabled = !anim.GetCurrentAnimatorStateInfo(0).IsName("Atack 3");

        if(agent.remainingDistance <= agent.stoppingDistance && target != null)
        {
            anim.SetBool("Hit", true);
        }
    }

    public int CommandRetreat()
    {
        Debug.Log("Retreat--хохо");
        return 0;
    }

    public int CommandTrading()
    {
        chest.StartTrading();
        return 0;
    }
    public int CommandGiveMoney()
    {
        chest.GiveMoney(100);
        return 0;
    }

    public override void Use()
    {
        StartDialogue();
    }

    private void StartDialogue()
    {
        if(state == States.Мертв)return;
        
        dSDialogue.StartDialogue(this);
    }
}
