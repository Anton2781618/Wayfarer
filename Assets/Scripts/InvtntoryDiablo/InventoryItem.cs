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
    public int Amount = 0;
    public RectTransform rectItemHighLight;
    public Text amauntText;
    public delegate void MyDelegate();
    private Dictionary<ItemType,MyDelegate> delegatesDict;

    private AbstractBehavior applicant;

    private void Start() 
    {
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

    public int onGridPositionX;  
    public int onGridPositionY;  

    public bool rotated = false;

    internal void Setup(ItemData itemData)
    {
        this.itemData = itemData;

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

    private void DestructSelf(ItemGrid itemGrid)
    {
        if(this == null) return;
        
        Destroy(gameObject);

        itemGrid.chestKeeper.RemoveAtChestGrid(this);

        GameManager.singleton.SwithContextMenu(false);
        
        GameManager.singleton.SwithInfoItem(false);
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

        DestructSelf(applicant.Chest.GetChestGrid());
    }

    //Наплечники
    private void UseShoulder()
    {
        Debug.Log("Наплечники");
    }

    private void UseHealthPotion()
    {
        applicant.Healing(Amount);

        DestructSelf(applicant.Chest.GetChestGrid());

        Debug.Log("Использовал Зелье здоровья");
    }

    private void UseManaPotion()
    {
        ItemGrid buferGrid = transform.parent.GetComponent<ItemGrid>();

        buferGrid.abstractBehavior.RestoreMana(Amount);

        DestructSelf(buferGrid);

        Debug.Log("Использовал Зелье Маны");
    }

    //деньги
    private void UseMoney()
    {
        ItemGrid buferGrid = transform.parent.GetComponent<ItemGrid>();

        buferGrid.chestKeeper.money += Amount;

        buferGrid.chestKeeper.UpdateMoney();
        
        DestructSelf(buferGrid);
    }
}
