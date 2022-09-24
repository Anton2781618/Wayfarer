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
    [SerializeField] private DSDialogueContainerSO currentSolution;
    [SerializeField] private SolutionInfo hungerSolution;
    [SerializeField] private SolutionInfo AttackSolution;
    [SerializeField] private SolutionInfo sleepSolution;
    [SerializeField] private SolutionInfo healingSolution;
    public DSDialogueSO stage{get; private set;}
    bool newDialogue = false;
    public bool isSolutionActive = false;
    public bool brain = true;
    public bool eye = true;
    public bool sort = true;
    public bool command = true;

    public void Init(Unit unit, Animator anim)
    {
        this.unit = unit;

        this.eyes.Init(mamry, anim.GetBoneTransform(HumanBodyBones.Head), unit.transform);
    }

    //метод анализирует обстановку вокруг и принимает решения как реагировать
    public void Analyzer()
    {
        if (Time.frameCount % 40 == 0)
        {
            if(brain)StatsConsumption();
        
            if(sort)AnalyzeImportanceSolutions();

            if(eye)eyes.FirndVisiblaTargets();
        }

        if(command)unit.ExecuteCurrentCommand();
    }

    public void SetAttackSolution()
    {        
        SetSolutionInList(AttackSolution, 100);
    }
    public void SethealingSolution()
    {        
        SetSolutionInList(healingSolution, 90);
    }
    public void RemoveAttackSolution()
    {        
        unit.solutions.Remove(AttackSolution);
    }

    //метод расходует статы типа голод, сон итп 
    private void StatsConsumption()
    {
        if(unit.unitStats.hunger > 0) unit.unitStats.hunger --;
        
        if(unit.unitStats.sleep > 0) unit.unitStats.sleep --;

        SetSolutionInList(hungerSolution, unit.unitStats.hunger);

        SetSolutionInList(sleepSolution, unit.unitStats.sleep);

        if(!isSolutionActive)
        {
            isSolutionActive = true;

            if(unit.solutions[0].solution != currentSolution)
            {
                currentSolution = unit.solutions[0].solution;
            }
            
            StartSolution(); 
        }
    }

    private void SetSolutionInList(SolutionInfo solution, float haracteistica)
    {
        if(solution == AttackSolution)
        {
            if(!unit.solutions.Contains(solution)) unit.solutions.Add(solution);
         
            return;
        }

        if(solution == healingSolution)
        {
            if(!unit.solutions.Contains(solution)) unit.solutions.Add(solution);

            if(unit.unitStats.curHP >= unit.unitStats.maxHP)
            {
                if(unit.solutions.Contains(solution)) unit.solutions.Remove(solution);
            }
         
            return;
        }

        if(haracteistica < 80 )
        {
            if(!unit.solutions.Contains(solution)) unit.solutions.Add(solution);

            if(solution.importance < 100) solution.importance ++;
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
    public Mamry GetMamry() => mamry;   

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

    public void StartDialogueStage()
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

[Serializable]
public class Mamry
{
    public Workplace workplace;
    public List<Transform> mamryTargets = new List<Transform>();
}