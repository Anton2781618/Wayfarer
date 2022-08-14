using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using CartoonHeroes;

//класс является представлением места для хранения предметов (сундук или инвентарь игрока или торговца)
public class Chest : MonoBehaviour, ICanUse
{
    private InventoryController inventoryController;
    [SerializeField] private ItemGrid chestGrid;
    
    public int money = 500;//{get; set;} = 500;
    
    private Outline outline;
    [Header("Предметы")]
    [Tooltip("Предметы в инвентаре")] [SerializeField] private List<InventoryItemInfo> inventoryItems;

    private void Start() 
    {
        outline = GetComponent<Outline>();    
        inventoryController = FindObjectOfType<InventoryController>();
        UpdateMoney();
    }

    public void InitChest(ItemGrid chestGrid)
    {
        this.chestGrid = chestGrid;
    }
    
    public void ShowOutline(bool value)
    {
        outline.enabled = value;
    }  

    public void Use()
    {
        OpenChest(false);
        inventoryController.GetPlayerChest().OpenPlayerInventory();
    }

    public void UpdateMoney()
    {
        if(!chestGrid || !chestGrid.moneyText) return;

        chestGrid.moneyText.text = money.ToString();
    }

    public void StartTrading()
    {        
        inventoryController.IsTreid = true;
        Use();
    }

    public void OpenPlayerInventory()
    {
        OpenChest(true);
    }

    private void OpenChest(bool isPlayerInventory)
    {
        if(!isPlayerInventory)inventoryController.selectedChest = this;
        chestGrid.chest = this;
        this.TryGetComponent(out chestGrid.abstractBehavior);
        
        ClearItems();
        StartCoroutine(WaightSec(isPlayerInventory));
    }

    private IEnumerator WaightSec(bool isPlayerInventory)
    {
        yield return null;

        inventoryController.SelectedItemGrid = chestGrid;
        
        InsertItems();
        
        inventoryController.SelectedItemGrid = null;

        chestGrid.ImageMoney.gameObject.SetActive(inventoryController.IsTreid || isPlayerInventory);

        if(!isPlayerInventory)GameManager.singleton.OpenChest();
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

    //добавить предмет в список
    public void AddItemToChest(InventoryItemInfo item)
    {
        inventoryItems.Add(item);
    }

    //метод убирает из списка итемов в инвентаре определенный итем 
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

    private void ClearItems()
    {
        foreach (Transform item in chestGrid.transform)
        {
           if(item.gameObject.layer == 5) Destroy(item.gameObject);
        }
    }

    private void InsertItems()
    {
        foreach (InventoryItemInfo item in inventoryItems)
        {
           inventoryController.CreateAndInsertItem(item.itemData, chestGrid, item.Amount);
        }
    }

    public ItemGrid GetChestGrid()
    {
        return chestGrid;
    }

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
}
