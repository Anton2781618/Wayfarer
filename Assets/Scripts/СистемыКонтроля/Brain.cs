using System;
using System.Collections.Generic;
using DS.Enumerations;
using DS.ScriptableObjects;
using UnityEngine;
using Unity.VisualScripting;

[Serializable]
public class Brain
{   
    public SolutionInfo currentSolution {get; set;}
    public DSDialogueSO stage{get; private set;}
    private Unit _unit;
    private bool newDialogue = false;
    private DialogueWindowUI _dialogueWindowUI;
    [SerializeField] private Eyes _eyes = new Eyes();
    [SerializeField] private Mamry _mamry = new Mamry();
    [SerializeField] private Hearing _hearing = new Hearing();

    public void Init(Unit unit, Animator anim)
    {
        _dialogueWindowUI = GameManager.Instance.GetDialogWindow();

        this._unit = unit;

        this._eyes.Init(_mamry, anim.GetBoneTransform(HumanBodyBones.Head), unit.transform);
    }

    //метод анализирует обстановку вокруг, видит слышит 
    public void Analyzer()
    {
        if (Time.frameCount % 40 == 0)
        {
            // if(brain)StatsConsumption();
        
            // if(sort)AnalyzeImportanceSolutions();

            _eyes.FirndVisiblaTargets();
        }
        _unit.ExecuteCurrentCommand();
    }

    //запустить решение
    public void StartAction(DSDialogueContainerSO solution)
    {
        SolutionInfo newSolution = new SolutionInfo(100, solution);

        currentSolution = newSolution;
        
        // _unit.solutions.Add(currentSolution);

        StartSolution();
    }

    public Eyes GetEyes() => _eyes;   

    public Mamry GetMamry() => _mamry;   
    
    public Hearing GetHearing() => _hearing;   

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

    //старт диалога
    public void StartDialogue(DSDialogueContainerSO dialogueContainer)
    {
        SolutionInfo newSolution = new SolutionInfo(100, dialogueContainer);

        currentSolution = newSolution;
        
        // _unit.solutions.Add(currentSolution);

        GameManager.Instance.BlockPlayerControl(true, false);

        foreach (var dialogueStage in currentSolution.solution.UngroupedDialogues)
        {
            if(dialogueStage.IsStartingDialogue)stage = dialogueStage;
        }
        
        _dialogueWindowUI.ShowDialogWindow(true);

        newDialogue = true;
    }
    
    //старт этапа
    public void StartActionStage() => _unit.SetAction(stage.Action, stage.ModelDate);

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

            // _unit.solutions.Remove(currentSolution);

            currentSolution = null;

            CustomEvent.Trigger(_unit.gameObject, "ReturnToIdle");
            // if(_unit.solutions.Count == 0)
            // {

            // } 
            // else
            // {
            //     currentSolution = _unit.solutions[0];

            //     StartSolution();
            // }

            return;
        }

        StartStage();
    }

    public void StartDialogueStage()
    {
        Debug.Log("создаю кнопки");

        newDialogue = false;

        GameManager.Instance.BlockPlayerControl(true, false);

        _dialogueWindowUI.ShowDialogWindow(true);
        
        _dialogueWindowUI.SetDialogueText(stage.Text);

        _dialogueWindowUI.ClearButtons();

        stage.Choices.ForEach(t => _dialogueWindowUI.CreateButtonsAnswers(t.Text, this));
    }

    public void CloseDialogue()
    {
        Debug.Log("Конец диалога");

        GameManager.Instance.BlockPlayerControl(false, true);
        
        GameManager.Instance.GetDialogWindow().ShowDialogWindow(false);

        GameManager.Instance.CloseAllUiPanels();
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