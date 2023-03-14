using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPWindowUI : MonoBehaviour
{
    [SerializeField] private Slider _HpSlider;   
    [SerializeField] private Slider _MpSlider; 

    public void InitHp(int currentHp, int maxHp)
    {
        _HpSlider.maxValue = maxHp;

        _HpSlider.value = currentHp;
    }  

    public void SetHpValue(int value)
    {
        _HpSlider.value -= value;
    }

    public void SetManaValue(int value)
    {
        _MpSlider.value -= value;
    }
}
