using BehaviorDesigner.Runtime.Tasks;
using DS;
using DS.Enumerations;
using UnityEngine;
using UnityEngine.UI;

public class DialogButtonTransfer : MonoBehaviour
{
    [SerializeField] private Text _textButt;
    [SerializeField] private int _indexDialog;
    private Brain _brain;

    public void Init(string textMess, int index, Brain brain)
    {
        _textButt.text = textMess;

        _indexDialog = index;

        this._brain = brain;
    }
    public void SetChoice()
    {      
        if(_brain.stage.Choices[_indexDialog].NextDialogue == null || _brain.stage.Choices[_indexDialog].NextDialogue.DialogueType == DSDialogueType.Action)
        {
            _brain.CloseDialogueAndExitSoltuin();
            
            _brain.NextStage(_indexDialog);
        }
        else
        {
            _brain.NextStage(_indexDialog);
        }
    }    
}
