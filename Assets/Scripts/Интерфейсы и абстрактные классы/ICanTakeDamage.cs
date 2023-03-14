using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanTakeDamage
{
    public void TakeDamage(AbstractBehavior enemy,  int value);
    public void Die();
}
