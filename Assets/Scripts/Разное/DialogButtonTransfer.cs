using DS;
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
        if(!ai.stage)
        {
            ai.CloseDialogueAndExitSoltuin();
            ai.StartNextStage(indexDialog);
        }
        else
        {
            ai.StartNextStage(indexDialog);
        }
    }    
}
