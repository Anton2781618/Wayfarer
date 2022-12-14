using BehaviorDesigner.Runtime.Tasks;
using DS;
using DS.Enumerations;
using UnityEngine;
using UnityEngine.UI;

public class DialogButtonTransfer : MonoBehaviour
{
    [SerializeField] private Text textButt;
    [SerializeField] private int indexDialog;
    private DialogNode dialogNode;

    public void Init(string textMess, int index, DialogNode dialogNode)
    {
        textButt.text = textMess;

        indexDialog = index;

        this.dialogNode = dialogNode;
    }
    public void SetChoice()
    {      
        dialogNode.choise = indexDialog;

        dialogNode.next = true;
    }    
}
