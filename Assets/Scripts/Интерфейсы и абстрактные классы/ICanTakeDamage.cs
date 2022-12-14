using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanTakeDamage
{
    public enum States
    {
        Мертв,
        Атака,
        Патруль,
        Сблизиться,
        Диалог,
        
    }
    public void TakeDamage(AbstractBehavior enemy,  int value);
    public void Die();
    public States GetCurrentUnitState();
}
