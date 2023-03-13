using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class DialogNode : Composite
    {
        public TaskStatus executionStatus = TaskStatus.Inactive;
        public List<string> answers;
        public bool next = false;
        public bool StartNode = false;

        public SharedBool dialog = false;
        public int choise = 0; 

        public UIDialogueTransfer dialogueTransfer;

        public override int CurrentChildIndex()
        {
            return choise;
        }

        public override void OnStart()
        {
            if(StartNode) return;

            StartNode = true;

            dialogueTransfer.ShowDialogWindow(true);

            GameManager.singleton.BlockPlayerControl(true); 


            dialogueTransfer.SetDialogueText(NodeData.Comment);

            dialogueTransfer.ClearButtons();

            // answers.ForEach(answer => dialogueTransfer.CreateButtonsAnswers(answer, this));
        }

        public override bool CanExecute()
        {
            // Мы можем продолжать выполнение до тех пор, пока 

            return next ;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // обновите статус выполнения после завершения дочернего элемента.
            if(childStatus == TaskStatus.Inactive)
            {
                next = false;
                
                dialog.Value = false;
                
                NodeData.ExecutionStatus = TaskStatus.Failure;    
                
                executionStatus = NodeData.ExecutionStatus;   

                GameManager.singleton.BlockPlayerControl(false); 

                dialogueTransfer.ShowDialogWindow(false);
        
                StartNode = false;
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
