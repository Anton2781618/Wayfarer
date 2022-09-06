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
    [SerializeField] private Eyes eyes = new Eyes();
    [SerializeField] private DSDialogueContainerSO[] solutions;
    [SerializeField] private DSDialogueContainerSO currentSolution;
    public DSDialogueSO stage;
    [SerializeField] private List<DSDialogueSO>  Buferdialogues = new List<DSDialogueSO>();

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

#region [rgba(30,106,143, 0.05)] Управление событиями -------------------------------------------------------------------------------------------------------//
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
    
    //Этот метод выбирает решение которое ИИ примит
    public void СhooseSolution()
    {
        currentSolution = solutions[0];
        stage = currentSolution.UngroupedDialogues[0];
    }

    //старт этапа
    public void StartStage() => unit.SetAction(stage.Action, stage.ModelDate);

    //перейти на следующий этап
    public void NextStage(int dialogueIndex)
    {
        if(!stage || !stage.Choices[dialogueIndex].NextDialogue) 
        {
            ExitSoltuin();
            return;
        }

        if(stage.Choices[dialogueIndex].NextDialogue.DialogueType == DSDialogueType.MultipleChoice)
        {
            StartDialogue();
            return;
        }

        stage = stage.Choices[dialogueIndex].NextDialogue;

        StartStage();
    }

    //метод выходит из решения
    private void ExitSoltuin()
    {
        Debug.Log("Решение выполнено!");
    }

    public Eyes GetEyes()
    {
        return eyes;
    }

    //расмотреть варианты ноды
    public void TestAction()
    {
        
        // dialogue.Choices.Sort();
        foreach (var choice in stage.Choices)
        {
            Debug.Log("стою на ноде " + stage.DialogueName);

            if(choice.NextDialogue == null)
            {
                Debug.Log("У ноды путой порт " + stage.DialogueName + " Выход!");
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
        Buferdialogues.Add(stage);
        stage = choice.NextDialogue;
        TestAction();
    }

    public void StepBack()
    {
        Debug.Log("у ноды " + stage.DialogueName +  " все варианты закрыты, ставлю отметку что сюда больше нельзя ходить! Шаг назад");
        stage.Text = "нет";
        if(Buferdialogues.Count > 0)Buferdialogues.Remove(stage);
        
        if(Buferdialogues.Count == 0)
        {
            Debug.Log("все варианты перебраны, выхода нет! Отмена операции");
            Buferdialogues.Clear();
            return;
        }
        
        stage = Buferdialogues[Buferdialogues.Count - 1];
        
        TestAction();
    }

    #endregion Управление событиями КОНЕЦ --------------------------------------------------------------------------------------------------------------------//

#region [rgba(30,106,143, 0.05)] Управление диалогом --------------------------------------------------------------------------------------------------------//

        public int Choice {get; set;} = 0;
        private UIDialogueTransfer dialogueTransfer;

        public void SetDialog(DSDialogueContainerSO dialogueContainer)
        {
            currentSolution = dialogueContainer;
        }

        public void SetChoice(int index)
        {
            stage = stage.Choices[Choice = index].NextDialogue;
            Next();
        }

        public void StartDialogue()
        {
            dialogueTransfer = GameManager.singleton.GetDialogueTransfer();
            
            GameManager.singleton.SwithCameraEnabled(false);

            GameManager.singleton.SetIsControlingPlayer(false);

            Cursor.visible = true;
            
            Cursor.lockState = CursorLockMode.None;
           
            foreach (var item in currentSolution.UngroupedDialogues)
            {
                if(item.IsStartingDialogue)stage = item;
            }
            
            dialogueTransfer.ShowDialogWindow(true);
            
            Next();
        }

        public void Next()
        {      
            if(!stage || !stage.Choices[Choice].NextDialogue) 
            {
                ExitTheDialog();
                return;
            }   
            // if(stage.DialogueType == DSDialogueType.Action && stage.Action == DSAction.ExitTheDialog) 
            // {                    
            //     ExitTheDialog();
            //     return;
            // }

            if(stage.DialogueType == DSDialogueType.Action)
            {
                SetChoice(unit.SetAction(stage.Action, stage.ModelDate));
            }
            else
            {
                Debug.Log("qpweq2123231232");
                dialogueTransfer.SetDialogText(stage.Text);

                dialogueTransfer.ClearButtons();

                stage.Choices.ForEach(t => dialogueTransfer.CreateButtonsAnswers(t.Text, this));
            }
        }

        private void ExitTheDialog()
        {
            Debug.Log("Выхожу из диалога");
            GameManager.singleton.SwithCameraEnabled(true);
            GameManager.singleton.SetIsControlingPlayer(true);
            GameManager.singleton.GetDialogueTransfer().ShowDialogWindow(false);
            GameManager.singleton.CloseAllUiPanels();
        }        

#endregion Управление диалогом КОНЕЦ ------------------------------------------------------------------------------------------------------------------------//
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