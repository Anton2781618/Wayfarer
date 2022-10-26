using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    //название
    public string title;
    public string description;
    public int price = 0;

    //показатель пользы. Например для зелья лечения это сколько хп востановит, а для брони это какая защита
    public int benefit = 0;

    public int width = 1;
    public int height = 1;

    public Sprite itemIcon;
    public GameObject prefab;

    //этот префаб одежды которая наденется непосредственно на
    public GameObject prefabForPutOn;
    public ItemType itemType;

    public Vector3 pos;
    
    public bool isSingle = true; 

    [Flags]
    public enum ItemType
    {
        Шлем = 1 << 0, 
        Броня = 1 << 1, 
        Ремень = 1 << 2, 
        Штаны = 1 << 3,
        Сапоги = 1 << 4,
        Оружие = 1 << 5,
        Щит = 1 << 6,
        Кольцо = 1 << 7,
        Ожерелье = 1 << 8,
        Наплечники = 1 << 9,
        Зелье_здоровья = 1 << 10,
        Зелье_маны = 1 << 11,
        Золотые_монеты = 1 << 12,
        Еда = 1 << 13,
    }
}
