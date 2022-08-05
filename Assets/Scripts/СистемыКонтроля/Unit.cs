using System;
using System.Collections.Generic;
using DS;
using DS.Data;
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

    public override void Init()
    {
        chest.InitChest(Initializer.singleton.InitObject(InitializerNames.Инвентарь_Моб).GetComponent<ItemGrid>(),
                        Initializer.singleton.InitObject(InitializerNames.Спрайт_денег_Моб).GetComponent<Image>()
        );
    }

    private void Update() 
    {
        Controller();
        if(unitCanvas.gameObject.activeSelf)RotateCanvas();
    }

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

    public int DelegatOperation(DSAction action)
    {
        Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, action.ToString());
        return operation.Invoke();

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

    [ContextMenu("пуск")]
    public void st()
    {
        aI.ConsiderOptions();
    }
}

[Serializable]
public class AI
{
    [SerializeField] private DSDialogueContainerSO dialogueContainer;
    [SerializeField] private DSDialogueSO dialogue;
    [SerializeField] private DSDialogueSO Buferdialogue;
    [SerializeField] private List<DSDialogueSO>  Buferdialogues = new List<DSDialogueSO>();
    


    //выбрать вариант
    public void ChooseOption()
    {
        
    }

    //расмотреть варианты ноды
    public void ConsiderOptions()
    {
        
        // dialogue.Choices.Sort();
        foreach (var choice in dialogue.Choices)
        {
            Debug.Log("стою на ноде " + dialogue.DialogueName);

            if(choice.NextDialogue == null)
            {
                Debug.Log("У ноды путой порт " + dialogue.DialogueName + " Выход!");
                Buferdialogues.Clear();
                return;
            }
            
            if(choice.NextDialogue.Text == "да")//
            {
                NextNode(choice);
                return;                
            }
            else Debug.Log("Нода  " + choice.NextDialogue.DialogueName + " заблокирована! Выполнить метод невозможно! перехожу к следующему варианту");

        }

        StepBack();
    }

    public void NextNode(DSDialogueChoiceData choice)
    {
        Debug.Log("есть проход к ноде " + choice.NextDialogue.DialogueName + ", прохожу");
        Buferdialogues.Add(dialogue);
        dialogue = choice.NextDialogue;
        ConsiderOptions();
    }

    public void StepBack()
    {
        Debug.Log("у ноды " + dialogue.DialogueName +  " все варианты закрыты, ставлю отметку что сюда больше нельзя ходить! Шаг назад");
        dialogue.Text = "нет";
        if(Buferdialogues.Count > 0)Buferdialogues.Remove(dialogue);
        
        if(Buferdialogues.Count == 0)
        {
            Debug.Log("все варианты перебраны, выхода нет! Отмена операции");
            Buferdialogues.Clear();
            return;
        }
        
        dialogue = Buferdialogues[Buferdialogues.Count - 1];
        
        ConsiderOptions();
    }
}
