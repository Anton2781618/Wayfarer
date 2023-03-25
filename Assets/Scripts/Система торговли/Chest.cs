using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using CartoonHeroes;
using static ItemData;

//класс является представлением места для хранения предметов (сундук или инвентарь игрока или торговца)
public class Chest : MonoBehaviour, ICanUse
{
    [SerializeField] private List<ItemGrid> grids;
    private InventoryController inventoryController;
    [SerializeField] private ItemGrid chestGrid;
    
    
    public int money = 500;//{get; set;} = 500;
    
    private Outline outline;
    [Header("Предметы")]
    [Tooltip("Предметы в инвентаре")] [SerializeField] private List<InventoryItemInfo> inventoryItems;

    public enum InfoGrid
    {
        Шлем,
        Броня,
        Ремень,
        Штаны,
        Сапоги,
        Оружие,
        Щит,
        Кольцо,
        Кольцо2,
        Наплечники,
        Ожерелье,
        Инвентарь,
    }

    private void Start() 
    {
        outline = GetComponent<Outline>();

        //регистрирует себя только в слуае если это сундук
        if(gameObject.layer == LayerMask.NameToLayer("Chest"))
        {
            GameManager.Instance.RegistrateUnit(this);
        }
        
        inventoryController = GameManager.Instance.UIManager.GetInventoryWindowUI().GetInventoryController();
        
        UpdateMoney();
    }

    public void InitGrid(ItemGrid chestGrid) => this.chestGrid = chestGrid;
    public void InitGrids(List<ItemGrid> chestGrids)
    {
        grids = chestGrids;
     
        chestGrid = grids[(int)InfoGrid.Инвентарь];
    }

    public void ShowOutline(bool value) => outline.enabled = value;

    public void Use(AbstractBehavior applicant)
    {
        OpenChest(false);

        inventoryController.GetPlayerChest().OpenPlayerInventory();
    }

    public void UpdateMoney()
    {
        if(!chestGrid || !chestGrid.moneyText) return;

        chestGrid.moneyText.text = money.ToString();
    }

    public void StartTrading(AbstractBehavior applicant)
    {        
        inventoryController.IsTreid = true;

        Use(applicant);
    }

    public void OpenPlayerInventory() => OpenChest(true);

    private void OpenChest(bool isPlayerInventory)
    {
        if(!isPlayerInventory)inventoryController.selectedChest = this;

        chestGrid.chestKeeper = this;

        this.TryGetComponent(out chestGrid.abstractBehavior);
        
        ClearItems();

        StartCoroutine(WaightSecAndOpenChest(isPlayerInventory));
    }

    private IEnumerator WaightSecAndOpenChest(bool isPlayerInventory)
    {
        yield return null;

        inventoryController.SelectedItemGrid = chestGrid;
        
        InsertAllInventoryItems();
        
        inventoryController.SelectedItemGrid = null;

        chestGrid.ImageMoney.gameObject.SetActive(inventoryController.IsTreid || isPlayerInventory);

        if(!isPlayerInventory)GameManager.Instance.OpenChest();
    }

    public void UpdateChestItems()
    {
        inventoryItems.Clear();

        foreach (Transform item in chestGrid.transform)
        {
            if(item.gameObject.layer == 5)
            {
                InventoryItem buferItem = item.transform.GetComponent<InventoryItem>();
        
                inventoryItems.Add(new InventoryItemInfo(buferItem.itemData, buferItem.Amount));
            }
        }
    }

    //получить итем из мнвентаря
    public InventoryItemInfo GetInventoryItem(ItemData itemData)
    {
        foreach (var item in inventoryItems)
        {
            if(item.itemData == itemData) return item;
        }

        return null;
    }

    //проверяет есть ли в инвентаре такой предмет по scriptable object
    public bool CheckInventoryForItems(ItemData itemData)
    {
        foreach (var item in inventoryItems)
        {
            if(item.itemData == itemData) return true;
        }

        return false;
    }

    //проверяет есть ли в инвентаре такой предмет по типу
    public int CheckInventoryForItemsType(ItemType itemType)
    {
        foreach (var item in inventoryItems)
        {
            if(itemType.HasFlag(item.itemData.itemType)) return 0;
        }

        return 1;
    }

    //взять любой предмет инвентаря по типу предмета 
    public InventoryItemInfo GetInventoryForItemType(ItemType itemType)
    {
        foreach (var item in inventoryItems)
        {
            if(itemType.HasFlag(item.itemData.itemType)) return item;
        }

        return null;
    }

    //добавить предмет в сундук предмет по  
    public void AddItemToChest(InventoryItemInfo item) => inventoryItems.Add(item);

    //добавить предмет в сундук по scriptable object
    public void AddItemToChest(ItemData itemData) => inventoryItems.Add(new InventoryItemInfo(itemData, itemData.benefit));

    // метод убирает из списка итемов в инвентаре определенный итем 
    public void RemoveAtChestGrid(InventoryItem item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].itemData == item.itemData && inventoryItems[i].Amount == item.Amount)
            {
                inventoryItems.RemoveAt(i);

                return;
            }
        }
    }
    public void RemoveAtChestGrid(InventoryItemInfo item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].itemData == item.itemData && inventoryItems[i].Amount == item.Amount)
            {
                inventoryItems.RemoveAt(i);
                return;
            }
        }
    }

    //удалить UI объекты из слоя инвентаря
    private void ClearItems()
    {
        foreach (Transform item in chestGrid.transform)
        {
            GameManager.Instance.RemoveUsableObject(item.gameObject);

            if(item.gameObject.layer == 5) Destroy(item.gameObject);
        }
    }

    //взять итемы из списка и создать физически
    private void InsertAllInventoryItems()
    {
        foreach (InventoryItemInfo item in inventoryItems)
        {
           inventoryController.CreateAndInsertItem(item.itemData, chestGrid, item.Amount);
        }
    
        for (int i = 0; i < grids.Count; i++)
        {
            GameObject buferPrefab = grids[i].GetSetCharacter().itemGroups[0].items[i].prefab;

            if(buferPrefab != null)
            {
                inventoryController.CreateAndInsertItem( buferPrefab.GetComponent<ItemOnstreet>().GetItemData(), grids[i], 0);
            }
        }
    }

    public ItemGrid GetChestGrid() => chestGrid;

    public void ReceiveMoney(Chest targetChest, int value)
    {
        money += value;

        UpdateMoney();

        targetChest.money -= value;

        targetChest.UpdateMoney();
    }
}

[System.Serializable]
public class InventoryItemInfo
{
    public ItemData itemData;
    public int Amount = 0;
    public int onGridPositionX;  
    public int onGridPositionY;

    public InventoryItemInfo(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        Amount = amount;
    }

    public void Use(AbstractBehavior applicant)
    {
        InventoryItem test = new InventoryItem();

        test.itemData = itemData;
        
        test.Amount = Amount;

        test.InitDict();
        
        test.Use(applicant);
    }
}
