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
    [SerializeField] private DSDialogueContainerSO currentSolution;
    [SerializeField] private SolutionInfo hungerSolution;
    [SerializeField] private SolutionInfo sleepSolution;
    public DSDialogueSO stage{get; private set;}
    bool newDialogue = false;
    bool isSolutionActive = false;
    public bool brain = true;
    public bool eye = true;
    public bool sort = true;
    public bool command = true;

    public void Init(Unit unit, Animator anim)
    {
        this.unit = unit;

        this.eyes.Init(anim.GetBoneTransform(HumanBodyBones.Head), unit.transform);
    }

    //метод анализирует обстановку вокруг и принимает решения как реагировать
    public void Analyzer()
    {
        if(brain)StatsConsumption();
        
        if(sort)AnalyzeImportanceSolutions();

        if(command)unit.ExecuteCurrentCommand();

        if(eye)eyes.FirndVisiblaTargets();
    }

    //метод расходует статы типа голод, сон итп 
    private void StatsConsumption()
    {
        if(unit.unitStats.hunger > 0) unit.unitStats.hunger -= Time.deltaTime;
        
        if(unit.unitStats.sleep > 0) unit.unitStats.sleep -= Time.deltaTime;

        SetSolutionInList(hungerSolution, unit.unitStats.hunger, 1);

        SetSolutionInList(sleepSolution, unit.unitStats.sleep, 2);

        if(!isSolutionActive)
        {
            // isSolutionActive = true;

            if(unit.solutions[0].solution != currentSolution)
            {
                currentSolution = unit.solutions[0].solution;

                StartSolution(); 
            }
        }
    }

    private void SetSolutionInList(SolutionInfo solution, float haracteistica, int moificator = 2)
    {
        if(haracteistica < 95)
        {
            if(!unit.solutions.Contains(solution)) unit.solutions.Add(solution);

            if(solution.importance < 100) solution.importance += Time.deltaTime / moificator;
        }
        else
        {
            solution.importance = 0;

            if(unit.solutions.Contains(solution)) unit.solutions.Remove(solution);
        }
    }

    //метод анализирует важность решений и сортирует список по важности
    public void AnalyzeImportanceSolutions()
    {
        unit.solutions.Sort(SortByImportance);
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

    public Eyes GetEyes() => eyes;   

    #region [rgba(30,106,143, 0.05)] Управление событиями и диалогами -------------------------------------------------------------------------------------------------------//
    
    //старт решения
    public void StartSolution()
    {
        foreach (var item in currentSolution.UngroupedDialogues)
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
    public void StartNextActionStage() => unit.SetAction(stage.Action, stage.ModelDate);

    private void StartStage()
    {
        if(stage.DialogueType == DSDialogueType.Action)
        {
            StartNextActionStage();
        }
        else
        {
            StartNextDialogueStage();
        }
    }

    //перейти на следующий этап
    public void NextStage(int choiceIndex)
    {   
        if(!newDialogue) stage = stage.Choices[choiceIndex].NextDialogue;

        if(stage == null) 
        {
            Debug.Log("Решение выполнено!");
            
            isSolutionActive = false;

            return;
        }

        StartStage();
    }
 
    //старт диалога
    public void StartDialogue(DSDialogueContainerSO dialogueContainer)
    {
        currentSolution = dialogueContainer;

        dialogueTransfer = GameManager.singleton.GetDialogueTransfer();
        
        GameManager.singleton.SwithCameraEnabled(false);

        GameManager.singleton.SetIsControlingPlayer(false);

        Cursor.visible = true;
        
        Cursor.lockState = CursorLockMode.None;

        foreach (var dialogueStage in currentSolution.UngroupedDialogues)
        {
            if(dialogueStage.IsStartingDialogue)stage = dialogueStage;
        }
        
        dialogueTransfer.ShowDialogWindow(true);

        newDialogue = true;
    }

    public void StartNextDialogueStage()
    {
        Debug.Log("создаю кнопки");

        newDialogue = false;

        dialogueTransfer.SetDialogueText(stage.Text);

        dialogueTransfer.ClearButtons();

        stage.Choices.ForEach(t => dialogueTransfer.CreateButtonsAnswers(t.Text, this));
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

[Serializable]
public class Eyes
{
    private Transform headTransform;
    private Transform myTransform;
    [SerializeField] private float viewRadiusEyes;
    [SerializeField] [Range(0, 360)] private float viewAngleEyes;
    [SerializeField] private LayerMask targetMaskForEyes;
    [SerializeField] private LayerMask obstaclMaskForEyes;
    public List<Transform> visileTargets = new List<Transform>();
    public List<Transform> mamryTargets = new List<Transform>();

    public void Init(Transform headTrans, Transform unitTran)
    {
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
        for (int i = 0; i < mamryTargets.Count; i++)
        {
            if(visileTarget == mamryTargets[i]) return true;
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