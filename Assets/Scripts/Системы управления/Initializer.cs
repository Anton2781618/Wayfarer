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
        Сетка_Шлем, Сетка_Броня, Сетка_Ремень, Сетка_Штаны, Сетка_Сапоги, Сетка_Оружие, Сетка_Щит, Сетка_Кольцо,
        Сетка_Кольцо2, Сетка_Наплечники, Сетка_Ожерелье, Сетка_Инвентарь,
    }

    private void Awake() 
    {
        singleton = this;
    }
    public GameObject InitObject(InitializerNames value) 
    {
        return objectsForInit[(int)value];
    }
}
