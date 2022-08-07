using System;
using System.Collections.Generic;
using DS.Data;
using DS.ScriptableObjects;
using UnityEngine;

[Serializable]
public class AI
{   
    
    [SerializeField] private Unit unit;
    [SerializeField] private DSDialogueContainerSO[] solutions;
    [SerializeField] private DSDialogueContainerSO currentSolution;
    private DSDialogueSO stage;
    [SerializeField] private List<DSDialogueSO>  Buferdialogues = new List<DSDialogueSO>();

    

    //метод анализирует обстановку вокруг и принимает решения как реагировать
    public void Analyzer()
    {
        unit.CurrentAction();
    }

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
    public void StartStage() => unit.SetAction(stage.Action);

    //перейти на следующий этап
    public void NextStage()
    {
        stage = stage.Choices[0].NextDialogue;
        StartStage();
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
}