using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanUse
{
    public Transform transform {get;}
    public void Use(AbstractBehavior applicant = null);
    public void ShowOutline(bool value);
}

public interface IWorkplace : ICanUse
{
    public bool WorkIsFinish {get;}
}