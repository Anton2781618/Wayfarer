using DS;
using DS.Enumerations;
using UnityEngine;
using UnityEngine.UI;

public class DialogButtonTransfer : MonoBehaviour
{
    [SerializeField] private Text textButt;
    private PlayerQuest playerQuest;
    [SerializeField] private int indexDialog;
    private AI ai;

    private void Start() 
    {
        playerQuest = FindObjectOfType<PlayerQuest>();
    }
    public void Init(string textMess, int index, AI ai)
    {
        textButt.text = textMess;

        indexDialog = index;

        this.ai = ai;
    }
    public void SetChoice()
    {
        
        if(ai.stage.Choices[indexDialog].NextDialogue.DialogueType == DSDialogueType.Action)
        {
            ai.CloseDialogueAndExitSoltuin();
            
            ai.NextStage(indexDialog);
        }
        else
        {
            ai.NextStage(indexDialog);
        }
    }    
}
