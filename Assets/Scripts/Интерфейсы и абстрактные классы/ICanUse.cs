using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanUse
{
    public Transform transform {get;}
    public void Use(AbstractBehavior applicant = null);
    public void ShowOutline(bool value);
}

public interface IWorkplace
{
    public Transform transform {get;}
    public void Use(Unit worker = null);
    public void FinishWork(Unit worker = null);
    public bool PossibleToWork {get;}
}