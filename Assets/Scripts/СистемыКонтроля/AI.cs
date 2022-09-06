using System;
using System.Collections.Generic;
using DS.Data;
using DS.Enumerations;
using DS.ScriptableObjects;
using UnityEngine;

[Serializable]
public class AI
{   
    private Unit unit;
    private UIDialogueTransfer dialogueTransfer;
    [SerializeField] private Eyes eyes = new Eyes();
    [SerializeField] private DSDialogueContainerSO[] solutions;
    [SerializeField] private DSDialogueContainerSO currentSolution;
    public DSDialogueSO stage;
    bool dialogueIsStart = false;

    public void Init(Unit unit, Animator anim)
    {
        this.unit = unit;

        this.eyes.Init(anim.GetBoneTransform(HumanBodyBones.Head), unit.transform);
    }

    //метод анализирует обстановку вокруг и принимает решения как реагировать
    public void Analyzer()
    {
        unit.ExecuteCurrentCommand();

        eyes.FirndVisiblaTargets();
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

                StartNextActionStage();

                return;
            }
        }
    }  
    
    //Этот метод выбирает решение которое ИИ примит
    public void СhooseSolution()
    {
        currentSolution = solutions[0];

        stage = currentSolution.UngroupedDialogues[0];
    }

    //старт этапа
    public void StartNextActionStage() => unit.SetAction(stage.Action, stage.ModelDate);
    
    //перейти на следующий этап
    public void StartNextStage(int choiceIndex)
    {
        if(!stage || !stage.Choices[choiceIndex].NextDialogue) 
        {
            ExitSoltuin();

            return;
        }        

        if(!dialogueIsStart)stage = stage.Choices[choiceIndex].NextDialogue;

        if(stage.DialogueType == DSDialogueType.Action)
        {
            StartNextActionStage();
        }
        else
        {
            dialogueIsStart = false;

            StartNextDialogueStage();
        }

        if(!stage.Choices[0].NextDialogue) ExitSoltuin();      
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

        dialogueIsStart = true;
    }

    public void StartNextDialogueStage()
    {
        Debug.Log("создаю кнопки");

        dialogueTransfer.SetDialogText(stage.Text);

        dialogueTransfer.ClearButtons();

        stage.Choices.ForEach(t => dialogueTransfer.CreateButtonsAnswers(t.Text, this));
    }

    private void ExitSoltuin()
    {
        Debug.Log("Решение выполнено!");

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

    public Vector3 DirFromAngle(float angleInDegriees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegriees += headTransform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegriees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegriees * Mathf.Deg2Rad));
    }
}