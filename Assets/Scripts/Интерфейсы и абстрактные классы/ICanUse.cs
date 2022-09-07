using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanUse
{
    public Transform transform {get;}
    public void Use(AbstractBehavior applicant);
    public void ShowOutline(bool value);
}
