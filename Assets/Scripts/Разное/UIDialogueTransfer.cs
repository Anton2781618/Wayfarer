using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueTransfer : MonoBehaviour
{
    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private Text dialogText;
    [SerializeField] private DialogButtonTransfer questButt;
    private List<DialogButtonTransfer>questButtons = new List<DialogButtonTransfer>();
    private int index = 0;

    public void ShowDialogWindow(bool value)
    {
        dialogWindow.SetActive(value);
    }

    public void SetDialogueText(string value)
    {
        dialogText.text = value;
    }

    public void CreateButtonsAnswers(string value, DialogNode dialogNode)
    {
        DialogButtonTransfer buffer = Instantiate(questButt, dialogWindow.transform);
        
        buffer.Init(value, index, dialogNode);

        questButtons.Add(buffer);

        buffer.gameObject.SetActive(true);
        
        buffer.transform.position += new Vector3(0,-40 * index + 20,0);

        index ++;
    }

    public void ClearButtons()
    {        
        questButtons.ForEach(t => Destroy(t.gameObject));
        
        questButtons.Clear();
        
        index = 0;
    }
}
