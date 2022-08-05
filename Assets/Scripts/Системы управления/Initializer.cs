using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//класс служит как инициализатор объектов
public class Initializer : MonoBehaviour
{
    public static Initializer singleton;

    public List<GameObject> objectsForInit; 

    public enum InitializerNames
    {
        ХПслайдер_Плеера, МПслайдер_Плеера, Инвентарь_Плеер, Спрайт_денег_Плеер, Инвентарь_Моб, Спрайт_денег_Моб,
    }

    private void Awake() 
    {
        singleton = this;
    }
    public GameObject InitObject(InitializerNames value) 
    {
        return objectsForInit[(int)value];
    }

    public string Test()
    {
        return "dsd";
    }
}
