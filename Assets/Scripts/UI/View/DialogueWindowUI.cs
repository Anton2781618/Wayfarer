using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueWindowUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private Text dialogText;
    [SerializeField] private DialogButtonUI questButt;
    private List<DialogButtonUI>questButtons = new List<DialogButtonUI>();
    private int index = 0;

    public void ShowDialogWindow(bool value) => dialogWindow.SetActive(value);

    public void SetDialogueText(string value) => dialogText.text = value;

    public void CreateButtonsAnswers(string value, Brain brain)
    {
        DialogButtonUI buffer = Instantiate(questButt, dialogWindow.transform);
        
        buffer.Init(value, index, brain);

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
