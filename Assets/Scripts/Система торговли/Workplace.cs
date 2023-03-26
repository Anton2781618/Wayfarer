using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workplace : MonoBehaviour, IWorkplace
{
    //точка позиция работы
    public Transform workPoint;
    [SerializeField] private Work _work = Work.Blacksmith_work;
    public bool PossibleToWork {get; private set;} = true;

    public enum Work
    {
        Blacksmith_work,
        Farmer_work,   
    }

    public void ShowOutline(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void Use(Unit worker)
    {
        // Debug.Log("работаю !!!!!!!!");

        StartWork(worker);
    }

    private void StartWork(Unit worker)
    {        
        Debug.Log("Начал работать");
        
        worker.SetAnimationWork(_work.ToString(), true);
    }

    public void FinishWork(Unit worker)
    {
        Debug.Log("Закочнить работу");

        PossibleToWork = true;
        
        worker.SetAnimationWork(_work.ToString(), false);
    }

    private void UpdateWorkProcess()
    {
    }
}
