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
    [SerializeField] private Image ImageMoney;
    
    private Outline outline;
    [Header("Предметы")]
    [Tooltip("Предметы в инвентаре")] [SerializeField] private List<InventoryItemInfo> inventoryItems;

    private void Start() 
    {
        outline = GetComponent<Outline>();    
        inventoryController = FindObjectOfType<InventoryController>();
    }

    public void InitChest(ItemGrid chestGrid, Image ImageMoney)
    {
        this.chestGrid = chestGrid;
        this.ImageMoney = ImageMoney;
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
        if(!isPlayerInventory)inventoryController.TheChest = this;
        chestGrid.wearer = this;
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

        ImageMoney.gameObject.SetActive(inventoryController.IsTreid || isPlayerInventory);
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

    public void GiveMoney(int value)
    {
        chestGrid.money -= value;
        inventoryController.TheChest.chestGrid.money += value;
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
