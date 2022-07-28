using System.Collections;
using System.Collections.Generic;
using DS;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueTransfer : MonoBehaviour
{
    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private Text dialogText;
    [SerializeField] private GameObject DialogTransfer2;
    [SerializeField] private Text questText2;
    [SerializeField] private DialogButtonTransfer questButt;
    [SerializeField] private GameObject sqipButt;
    private List<DialogButtonTransfer>questButtons = new List<DialogButtonTransfer>();
    private int index = 0;

    public void ShowDialogWindow(bool value)
    {
        dialogWindow.SetActive(value);
    }

    public void SetDialogText(string value)
    {
        dialogText.text = value;
    }

    public void CreateButtonsAnswers(string value, DSDialogue dSDialogue)
    {
        DialogButtonTransfer buffer = Instantiate(questButt, dialogWindow.transform);
        buffer.Init(value, index, dSDialogue);

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
