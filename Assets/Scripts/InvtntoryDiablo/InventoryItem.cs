using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemData;

[Serializable]
public class InventoryItem : MonoBehaviour, ICanUse
{    
    public ItemData itemData;
    public int onGridPositionX;  
    public int onGridPositionY; 
    public bool rotated = false;
    public int Amount = 0;
    public RectTransform rectItemHighLight;
    public Text amauntText;
    public delegate void MyDelegate();
    private Dictionary<ItemType,MyDelegate> delegatesDict;
    private AbstractBehavior applicant;
    public ItemGrid _grid;
    

    private void Start() 
    {
        GameManager.Instance.RegistrateUnit(this);

        InitDict();
    }

    public void InitDict() 
    {
        delegatesDict = new Dictionary<ItemType, MyDelegate>();
        
        delegatesDict.Add(ItemType.Шлем, UseHelmet);
        delegatesDict.Add(ItemType.Броня, UseArmor);
        delegatesDict.Add(ItemType.Ремень, UseBelt);
        delegatesDict.Add(ItemType.Штаны, UseTrousers);
        delegatesDict.Add(ItemType.Сапоги, UseBoots);
        delegatesDict.Add(ItemType.Оружие, UseWeapon);
        delegatesDict.Add(ItemType.Щит, UseShild);
        delegatesDict.Add(ItemType.Кольцо, UseRing);
        delegatesDict.Add(ItemType.Ожерелье, UseNecklace);
        delegatesDict.Add(ItemType.Наплечники, UseShoulder);
        delegatesDict.Add(ItemType.Зелье_здоровья, UseHealthPotion);
        delegatesDict.Add(ItemType.Зелье_маны, UseManaPotion);
        delegatesDict.Add(ItemType.Золотые_монеты, UseMoney);
        delegatesDict.Add(ItemType.Еда, UseFood);
    }

    public int HEIGHT
    {
       get
       {
           if(rotated == false)
           {
               return itemData.height;
           }
           return itemData.width;
       }
    }

   public int WIDTH
   {
       get
       {
           if(rotated == false)
           {
               return itemData.width;
           }
           return itemData.height;
       }
   }

    internal void Setup(ItemData itemData, ItemGrid grid)
    {
        this.itemData = itemData;

        _grid = grid;

        GetComponent<Image>().sprite = itemData.itemIcon; 

        Vector2 size = new Vector2();

        size.x = itemData.width * ItemGrid.titleSizeWidth;

        size.y = itemData.height * ItemGrid.titleSizeHeight;

        GetComponent<RectTransform>().sizeDelta = size;

        rectItemHighLight.sizeDelta = size;

        amauntText.gameObject.SetActive(!itemData.isSingle);

        amauntText.GetComponent<RectTransform>().sizeDelta = size;

        UpdateAmountItem();
    }

    //установить сетку владельца
    public void SetGrid(ItemGrid grid) => _grid = grid;

    public void UpdateAmountItem()
    {
        amauntText.text = Amount.ToString();
    }

    internal void Rotated()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.rotation = Quaternion.Euler(0, 0, rotated ? 90f : 0f);
    }

    public void Use(AbstractBehavior applicant)
    {
        this.applicant = applicant;

        delegatesDict[itemData.itemType].Invoke();
    }

    public void DestructSelf()
    {
        if(this == null) return;

        Destroy(gameObject);
        
        GameManager.Instance.RemoveUsableObject(gameObject);

        _grid.chest.RemoveAtChestGrid(this);

        GameManager.Instance.SwithContextMenu(false);
        
        GameManager.Instance.OpenDescriptInfo(false);
    }

    public void ShowOutline(bool value)
    {
        throw new NotImplementedException();
    }

    private void UseHelmet()
    {
        Debug.Log("шлем");
    }

    private void UseArmor()
    {
        Debug.Log("броня");
    }

    //ремень
    private void UseBelt()
    {
        Debug.Log("ремень");
    }

    //Штаны
    private void UseTrousers()
    {
        Debug.Log("Штаныень");
    }
    
    //Сапоги
    private void UseBoots()
    {
        Debug.Log("Сапоги");
    }

    //Оружие
    private void UseWeapon()
    {
        Debug.Log("Оружие");
    }

    //Щит
    private void UseShild()
    {
        Debug.Log("Щит");
    }

    //Кольцо
    private void UseRing()
    {
        Debug.Log("Кольцо");
    }

    //Ожерелье
    private void UseNecklace()
    {
        Debug.Log("Ожерелье");
    }

    //еда
    private void UseFood()
    {
        Debug.Log("использована еда");

        applicant.unitStats.hunger += Amount;

        DestructSelf();
    }

    //Наплечники
    private void UseShoulder()
    {
        Debug.Log("Наплечники");
    }

    private void UseHealthPotion()
    {
        applicant.Healing(Amount);

        DestructSelf();

        Debug.Log("Использовал Зелье здоровья");
    }

    private void UseManaPotion()
    {
        applicant.RestoreMana(Amount);

        DestructSelf();

        Debug.Log("Использовал Зелье Маны");
    }

    //деньги
    private void UseMoney()
    {
        applicant.Chest.money += Amount;

        applicant.Chest.UpdateMoney();
        
        DestructSelf();

        Debug.Log("Добавлены деньги");
    }
}
