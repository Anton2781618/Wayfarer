using UnityEngine;
using UnityEngine.UI;

namespace DS
{
    using DS.Enumerations;
    using ScriptableObjects;

    public class DSDialogue : MonoBehaviour
    {
        /* Диалоговые Scriptable Objects */
        [SerializeField] private DSDialogueContainerSO dialogueContainer;
        [SerializeField] private DSDialogueGroupSO dialogueGroup;
        [SerializeField] private DSDialogueSO dialogue;

        /* Фильтры */
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;

        /* Индексы */
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;

        public int Choice {get; set;} = 0;
        private UIDialogueTransfer dialogueTransfer;

        public Unit targetUnit;

        public void StartDialogue(Unit unit)
        {
            targetUnit = unit;            
            dialogueTransfer = GameManager.singleton.GetDialogueTransfer();
            
            GameManager.singleton.SwithCameraEnabled(false);
            GameManager.singleton.SetIsControlingPlayer(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
           
            foreach (var item in dialogueContainer.UngroupedDialogues)
            {
                if(item.IsStartingDialogue)dialogue = item;
            }
            
            dialogueTransfer.ShowDialogWindow(true);
            Next();
        }

        public void Next()
        {         
            if(dialogue.DialogueType == DSDialogueType.Action && dialogue.Action == DSAction.ExitTheDialog) 
            {                    
                ExitTheDialog();
                return;
            }

            if(dialogue.DialogueType == DSDialogueType.Action)
            {
                SetChoice(targetUnit.SetAction(dialogue.Action, dialogue.ModelDate.itemData));
            }
            else
            {
                dialogueTransfer.SetDialogText(dialogue.Text);

                dialogueTransfer.ClearButtons();

                dialogue.Choices.ForEach(t => dialogueTransfer.CreateButtonsAnswers(t.Text, this));
            }
        }

        public void SetChoice(int index)
        {
            dialogue = dialogue.Choices[Choice = index].NextDialogue;
            Next();
        }

        private void ExitTheDialog()
        {
            GameManager.singleton.SwithCameraEnabled(true);
            GameManager.singleton.SetIsControlingPlayer(true);
            GameManager.singleton.GetDialogueTransfer().ShowDialogWindow(false);
            GameManager.singleton.CloseAllUiPanels();
        }        
    }
}