using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workplace : MonoBehaviour, IWorkplace
{
    //точка позиция работы
    public Transform workPoint;
    public bool WorkIsFinish {get; private set;} = true;
    
    private float workTime = 60;

    public enum workStage
    {
        Не_работает,
        Работает   
    }

    public void ShowOutline(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void Use(AbstractBehavior applicant)
    {
        if(workTime <= 0)StartWork();

        Debug.Log(workTime + " !!!!!!!!");

        UpdateWorkProcess();
    }

    private void StartWork()
    {        
        WorkIsFinish = false;

        workTime = 60;
    }

    private void UpdateWorkProcess()
    {
        if(!WorkIsFinish)
        {
            if(workTime > 0)
            {
                workTime -= Time.deltaTime;
            }
            else
            {
                WorkIsFinish = true;
            }
        }
    }

    
}
