using System;
using DS;
using DS.Enumerations;
using DS.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static ICanTakeDamage;
using static Initializer;


public class Unit : AbstractBehavior
{
    private delegate int Operation();
    [SerializeField] private DSDialogue dSDialogue;
    [SerializeField] protected Canvas unitCanvas;
    [SerializeField] protected AI aI = new AI();
    [SerializeField] protected DSAction action;
    private bool wait = false;

    public override void Init()
    {
        chest.InitChest(Initializer.singleton.InitObject(InitializerNames.Инвентарь_Моб).GetComponent<ItemGrid>(),
                        Initializer.singleton.InitObject(InitializerNames.Спрайт_денег_Моб).GetComponent<Image>()
        );
    }    
    private void Update() => aI.Analyzer();

    public override void Die()
    {
        base.Die();
        SowHealthBar (false);
    }

    public override void ShowOutline(bool value)
    {
        if(state == States.Мертв)
        {
            SowHealthBar(false);    
            return;
        }
        SowHealthBar(value);
    }    

    public override void SowHealthBar(bool value)
    {
        if(unitCanvas == null)
        {
            Debug.Log("у " + transform.name + " не установлен unitCanvas");
            return;
        }
        unitCanvas.gameObject.SetActive(value);
    }

    public void RotateCanvas()
    {
        if(unitCanvas.transform.rotation != Camera.main.transform.rotation)
        {
            unitCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public States GetUnityState() => state;

    //установить действие
    public int SetAction(DSAction action)
    {
        this.action = action;
        return 0;
    }

    // public int StartAction(DSAction action)
    // {
    //     Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, action.ToString());
    //     operation.Invoke();
    //     return 0
    // }
    
    //метод выполняет текущую задачу
    public void CurrentAction()
    {
        if(unitCanvas.gameObject.activeSelf)RotateCanvas();
        Debug.Log(wait);
        if(wait) return;

        Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, action.ToString());
        operation.Invoke();
    }

    public void SetTarget(ICanUse newTarget) => target = newTarget;

    public void AnimationStates() => anim.SetBool("walk", agent.remainingDistance > agent.stoppingDistance);

    private void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //снять с пауза выполнение решения и включить следующее решение
    public void SetOffwaitAndNextStage()
    {
        aI.NextStage();
        wait = false;
    }

    //поставить на паузу выполнение решения
    public void SetWaitOn()
    {
        wait = true;
        GameManager.singleton.TargetForWaight = this;
    }

    //стоять на месте
    public int CommandStandStill()
    {
        return 0;
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

        if(buferTarget.GetCurrentUnitState() == States.Мертв)
        {
            state = States.Патруль;
            return 0;
        }
        
        AnimationStates();
        agent.SetDestination(target.transform.position);

        // agent.enabled = !anim.GetCurrentAnimatorStateInfo(0).IsName("Atack 3");

        if(agent.remainingDistance <= agent.stoppingDistance && target != null)
        {
            anim.SetBool("Hit", true);
        }
        return 0;
    }

    public int CommandRetreat()
    {
        Debug.Log("Отступаю");
        return 0;
    }

    public int CommandTrading()
    {
        chest.StartTrading();

        SetWaitOn();
        
        return 0;
    }

    public override void Use() => CommandStartDialogue();

    private int CommandStartDialogue()
    {
        dSDialogue.StartDialogue(this);

        SetWaitOn();

        return 0;
    }

    public int CommandGiveMoney()
    {
        chest.GiveMoney(100);
        return 0;
    }

    public int CommandMoveToTarget()
    {
        if( agent.isActiveAndEnabled && agent.remainingDistance <= agent.stoppingDistance && target != null)
        {
            FaceTarget();
        }

        agent.SetDestination(target.transform.position);
        AnimationStates();

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            aI.NextStage();
        }
        return 0;
    }

    

    [ContextMenu("пуск")]
    public void Action2() => aI.StartSolution();
}