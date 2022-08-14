using System;
using System.Collections;
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
    private bool pause = false;

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

    //метод выполняет текущую задачу
    public void CurrentAction()
    {
        if(unitCanvas.gameObject.activeSelf)RotateCanvas();

        if(pause) return;

        Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, action.ToString());
        operation.Invoke();
    }

    public void SetTarget(ICanUse newTarget) => target = newTarget;

    public void SetAnimationRun(bool value) => anim.SetBool("walk", value);

    private void FaceToTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void SetPauseOff()
    {
        pause = false;
    }

    //снять с пауза выполнение решения и включить следующее решение
    public void SetPauseOffAndNextStage()
    {
        pause = false;
        
        action = DSAction.CommandStandStill;
        
        aI.NextStage();
    }

    //поставить на паузу выполнение решения
    public void SetPauseOn()
    {
        pause = true;

        GameManager.singleton.TargetForPause = this;
    }

    //стоять на месте
    public int CommandStandStill()
    {
        anim.SetBool("walk", false);

        agent.SetDestination(this.transform.position);
        
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
            
            aI.NextStage();
            
            return 0;
        }
        
        FaceToTarget();        

        agent.SetDestination(target.transform.position);        

        SetAnimationRun(agent.remainingDistance > agent.stoppingDistance);

        anim.SetBool("Hit", agent.remainingDistance <= agent.stoppingDistance && target != null);
        
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

        SetPauseOn();
        
        return 0;
    }

    public override void Use() => CommandStartDialogue();

    private int CommandStartDialogue()
    {
        dSDialogue.StartDialogue(this);

        SetPauseOn();

        return 0;
    }

    [ContextMenu("CommandGiveMoney")]
    public int CommandGiveMoney()
    {
        chest.GiveMoney(100);
        
        SetPauseOn();
        
        return 0;
    }

    public int CommandMoveToTarget()
    {
        if(target == null)
        {
            Debug.Log("у NPC не установлен таргет");
        }

        FaceToTarget();       

        agent.SetDestination(target.transform.position);        
        
        SetAnimationRun(agent.remainingDistance > agent.stoppingDistance);

        StartCoroutine(CheckAndSwitchStage());
        
        return 0;
    }

    private IEnumerator CheckAndSwitchStage()
    {
        yield return null;

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            aI.NextStage();
        }
    }

    [ContextMenu("пуск")]
    public void Action2() => aI.StartSolution();
}