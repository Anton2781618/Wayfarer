using System;
using System.Collections.Generic;
using DS.Enumerations;
using DS.ScriptableObjects;
using UnityEngine;

[Serializable]
public class AI
{   
    private Unit unit;
    private UIDialogueTransfer dialogueTransfer;
    [SerializeField] private Eyes eyes = new Eyes();
    [SerializeField] private Mamry mamry = new Mamry();
    [SerializeField] private Hearing hearing = new Hearing();
    [SerializeField] private SolutionInfo AttackSolution;
    public SolutionInfo currentSolution;// {get; set;}
    public DSDialogueSO stage{get; private set;}
    private bool newDialogue = false;

    public void Init(Unit unit, Animator anim)
    {
        this.unit = unit;

        this.eyes.Init(mamry, anim.GetBoneTransform(HumanBodyBones.Head), unit.transform);
    }

    public void SetAttackSolution()
    {        
        if(!unit.solutions.Contains(AttackSolution)) 
        {
            unit.solutions.Add(AttackSolution);
        }
    }

    public SolutionInfo GetCurrentSolution() => currentSolution;
    public Eyes GetEyes() => eyes;   
    public Mamry GetMamry() => mamry;   
    public Hearing GetHearing() => hearing;   

    #region [rgba(30,106,143, 0.05)] Управление событиями и диалогами -------------------------------------------------------------------------------------------------------//
    
    //старт решения
    public void StartSolution()
    {
        foreach (var item in currentSolution.solution.UngroupedDialogues)
        {
            if(item.IsStartingDialogue) 
            {   
                stage = item;

                StartStage();

                return;
            }
        }
    }  
    
    //старт этапа
    public void StartActionStage() => unit.SetAction(stage.Action, stage.ModelDate);

    private void StartStage()
    {
        if(stage.DialogueType == DSDialogueType.Action)
        {
            StartActionStage();
        }
        else
        {
            StartDialogueStage();
        }
    }

    //перейти на следующий этап
    public void NextStage(int choiceIndex)
    {   
        if(!newDialogue) stage = stage.Choices[choiceIndex].NextDialogue;

        if(stage == null) 
        {
            Debug.Log("Решение выполнено!");

            unit.solutions.Remove(currentSolution);

            currentSolution = null;

            return;
        }

        StartStage();
    }
 
    //старт диалога
    public void StartDialogue(DSDialogueContainerSO dialogueContainer)
    {
        SolutionInfo newSolution = new SolutionInfo(100, dialogueContainer);

        currentSolution = newSolution;
        
        unit.solutions.Add(currentSolution);

        dialogueTransfer = GameManager.singleton.GetDialogueTransfer();
        
        GameManager.singleton.SwithCameraEnabled(false);

        GameManager.singleton.SetIsControlingPlayer(false);

        Cursor.visible = true;
        
        Cursor.lockState = CursorLockMode.None;

        foreach (var dialogueStage in currentSolution.solution.UngroupedDialogues)
        {
            if(dialogueStage.IsStartingDialogue)stage = dialogueStage;
        }
        
        dialogueTransfer.ShowDialogWindow(true);

        newDialogue = true;
    }

    public void StartDialogueStage()
    {
        Debug.Log("создаю кнопки");

        newDialogue = false;

        dialogueTransfer.SetDialogueText(stage.Text);

        dialogueTransfer.ClearButtons();

        // stage.Choices.ForEach(t => dialogueTransfer.CreateButtonsAnswers(t.Text, this));
    }

    public void CloseDialogueAndExitSoltuin()
    {
        Debug.Log("Конец диалога");
        
        GameManager.singleton.SwithCameraEnabled(true);

        GameManager.singleton.SetIsControlingPlayer(true);

        GameManager.singleton.GetDialogueTransfer().ShowDialogWindow(false);

        GameManager.singleton.CloseAllUiPanels();
    }        

#endregion Управление событиями и диалогом КОНЕЦ ------------------------------------------------------------------------------------------------------------------------//
}

//класс описывает работу глаз
[Serializable]
public class Eyes
{
    private Mamry mamry;
    private Transform headTransform;
    private Transform myTransform;
    [SerializeField] private float viewRadiusEyes;
    [SerializeField] [Range(0, 360)] private float viewAngleEyes;
    [SerializeField] private LayerMask targetMaskForEyes;
    [SerializeField] private LayerMask obstaclMaskForEyes;
    public List<Transform> visileTargets = new List<Transform>();
    

    public void Init(Mamry mamry, Transform headTrans, Transform unitTran)
    {
        this.mamry = mamry;

        headTransform = headTrans;

        myTransform = unitTran;
    }

    public void SetTargetMaskForEyes(LayerMask value)
    {
        targetMaskForEyes = value;
    }
    
    public void FirndVisiblaTargets()
    {        
        visileTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(headTransform.position, viewRadiusEyes, targetMaskForEyes);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform newTarget = targetsInViewRadius[i].transform;
            
            Vector3 dirToTarget = (newTarget.position - headTransform.position).normalized;

            if(Vector3.Angle(headTransform.forward, dirToTarget) < viewAngleEyes / 2)
            {
                float distToTarget = Vector3.Distance(myTransform.position, newTarget.position);

                if(!Physics.Raycast(headTransform.position, dirToTarget, distToTarget, obstaclMaskForEyes))
                {
                    visileTargets.Add(newTarget);
                }
            }
        }        

        foreach (var tar in visileTargets)
        {
            Debug.DrawLine(headTransform.position, tar.position, Color.red);
        }
    }

    //запомнить о существовании объекта
    public bool SetTargetToMamry(Transform visileTarget)
    {
        for (int i = 0; i < mamry.mamryTargets.Count; i++)
        {
            if(visileTarget == mamry.mamryTargets[i]) return true;
        }                

        return false;
    }

    public Vector3 DirFromAngle(float angleInDegriees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegriees += headTransform.eulerAngles.y;
        }
        
        return new Vector3(Mathf.Sin(angleInDegriees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegriees * Mathf.Deg2Rad));
    }
}

//класс описывает работу слуха
[Serializable]
public class Hearing
{
    public List<GameObject> hearObjectsList;
}

//класс описывает работу памяти
[Serializable]
public class Mamry
{
    //рабочее место
    public Workplace workplace;

    //таргеты 
    public List<Transform> mamryTargets = new List<Transform>();

    //группа 
    public List<Unit> groupMembers;
}