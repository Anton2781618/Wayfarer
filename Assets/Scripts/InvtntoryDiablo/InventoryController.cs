using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;

//класс является системой управления всех ивентарей, основной функцианал инвентарей находится тут
public class InventoryController : MonoBehaviour
{

    //инвнтарь игрока
    private Chest playerChest;
    private ItemGrid buferGrid;
    
    //выбраный инвентарь
    public Chest selectedChest{get; set;}
    public bool IsTreid{get; set;} = false;
    private
     ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid 
    {
        get => selectedItemGrid; 
        
        set 
        {
            selectedItemGrid = value;
        
            inventoryIHighLight.SetParent(value);
        }
    }

    private InventoryItem selectedItem;    
    private InventoryItem overlapItem;    
    private RectTransform rectTransform;

    [SerializeField] private List<ItemData> items;
    [SerializeField] private GameObject itemPrefab;

    private Transform canvasTransform;
    private InventoryIHighLight inventoryIHighLight;
    private PLayerController player;

    private void Start() 
    {
        canvasTransform = transform.root;

        inventoryIHighLight = GetComponent<InventoryIHighLight>();  

        player = FindObjectOfType<PLayerController>();
        
        playerChest = player.GetComponent<Chest>();
    }

    private void Update() 
    {
        ItemIconDrah();

        if(Input.GetKeyDown(KeyCode.M))
        {
            if(selectedItem == null)
            {
                CreateRandomItem();
            }
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            InsertRandomItem();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if(SelectedItemGrid == null)
        {
            inventoryIHighLight.Show(false);

            if(Input.GetMouseButtonDown(0))
            {
                // GameManager.singleton.SwithInfoItem(false);
                // GameManager.singleton.SwithContextMenu(false);

                if(selectedItem && !IsTreid)DropItem();
            }
            return;
        }

        if(selectedItem && (SelectedItemGrid.GetGridForItemsType() & selectedItem.itemData.itemType) == 0)
        {
            inventoryIHighLight.Show(false);
            return;
        }

        HandleHighlight();
        
        if(Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }

        if(Input.GetMouseButtonDown(1))
        {
            RightMouseButtonPress();
        }
    }

    private void RotateItem()
    {
        if(selectedItem == null) {return;}

        selectedItem.Rotated();
    }

    //расположить рандомный предмет в сетку инвентаря
    private void InsertRandomItem()
    {
        if(selectedItemGrid == null) {return;}

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;

        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null) {return;}

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void InsertItemV2(InventoryItem itemToInsert, ItemGrid grid)
    {
        Vector2Int? posOnGrid = grid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null) {return;}

        grid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    //создать физически итем и установить его на сетку 
    public void CreateAndInsertItem(ItemData itemData, ItemGrid grid, int amount)
    {
        Debug.Log("!!! " + itemData.title);
        CreateItem(itemData, amount);
        
        InventoryItem itemToInsert = selectedItem;
        
        selectedItem = null;
        
        InsertItemV2(itemToInsert, grid);
    }

    public Chest GetPlayerChest() => playerChest;

    Vector2Int oldPosition;
    InventoryItem itemToHighLight;

    //метод подсветки предмета
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTitleGridPosition();
        if(oldPosition == positionOnGrid){return;}
         
        oldPosition = positionOnGrid;
        if(selectedItem == null)
        {
            itemToHighLight = SelectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            
            if(itemToHighLight != null)
            {
                // StartCoroutine(WaitAndOpenInfo());
                GameManager.singleton.GetInfoItem().GetComponent<WindowTranser>().SetTextsTransfers(itemToHighLight);

                inventoryIHighLight.Show(true);
                inventoryIHighLight.SetSize(itemToHighLight);
                inventoryIHighLight.SetPosition(SelectedItemGrid, itemToHighLight);
            }
            else
            {
                inventoryIHighLight.Show(false);
                
            }

            // GameManager.singleton.SwithInfoItem(false);
        }
        else
        {
            inventoryIHighLight.Show(SelectedItemGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y,
                                    selectedItem.WIDTH, selectedItem.HEIGHT));
            inventoryIHighLight.SetSize(selectedItem);
            inventoryIHighLight.SetPosition(SelectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    private IEnumerator WaitAndOpenInfo()
    {
        yield return new WaitForSeconds(0);
        
        RectTransform rect = GameManager.singleton.GetInfoItem().GetComponent<RectTransform>();

        SetPositionPopap(rect);
        GameManager.singleton.SwithInfoItem(true);
    }

    private void SetPositionPopap(RectTransform value)
    {
        value.position = Input.mousePosition.x + value.sizeDelta.x > Screen.width ? 
        value.position = new Vector2(Input.mousePosition.x - value.sizeDelta.x,Input.mousePosition.y) : value.position = Input.mousePosition;
    }

    //создать случайный итем
    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Setup(items[selectedItemID]);
    }

    private void CreateItem(ItemData itemData, int amount)
    {
        Debug.Log("dlskdj");
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        inventoryItem.Amount = amount;
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        inventoryItem.Setup(itemData);
    }

    //метод перемещает итем в след за мышкой
    private void ItemIconDrah()
    {
        if(selectedItem)
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    //метод выделяет итем и станавлевает его в ячейку
    private void LeftMouseButtonPress()
    {
        Vector2Int titleGridPosition = GetTitleGridPosition();
        
        GameManager.singleton.SwithContextMenu(false);
        
        GameManager.singleton.SwithInfoItem(false);
        
        if (selectedItem == null)
        {
            PickUpItem(titleGridPosition);
        }
        else
        {
            PlaceItem(titleGridPosition);
        }

        if(selectedChest) selectedChest.UpdateChestItems();

        playerChest.UpdateChestItems();
    }

    //открыть контектсное меню
    private void RightMouseButtonPress()
    {
        if (selectedItem != null) return;

        Vector2Int titleGridPosition = GetTitleGridPosition();
        InventoryItem inventoryItem = SelectedItemGrid.GetInventoryItem(titleGridPosition.x, titleGridPosition.y);
        
        if(!inventoryItem)return;
            
        GameObject contextMenu = GameManager.singleton.SwithContextMenu(true);

        contextMenu.GetComponent<ItemContextMenu>().SetTargetItem(inventoryItem);
        SetPositionPopap(contextMenu.GetComponent<RectTransform>());
        StartCoroutine(WaitAndOpenInfo()); 
    }

    //тут мы устанавливаем итем на сетку со смещением. Это для того что бы распологать итем по центру а не с краю мышки
    private Vector2Int GetTitleGridPosition()
    {
        Vector2 position = Input.mousePosition;
        
        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.titleSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.titleSizeHeight / 2;
        }

        return SelectedItemGrid.GetTitleGridPosition(position);
    }


    //поднять предмет с сетке
    private void PickUpItem(Vector2Int titleGridPosition)
    {
        selectedItem = SelectedItemGrid.SelectIteme(titleGridPosition.x, titleGridPosition.y);

        if(SelectedItemGrid.isSingle)
        {
            TakeOffClothes(selectedItem.itemData.GetItemTypeIndex());
        }

        if (selectedItem)
        {
            selectedItem.transform.SetParent(selectedItem.transform.root);
            rectTransform = selectedItem.GetComponent<RectTransform>();
            rectTransform.SetAsLastSibling();

            buferGrid = SelectedItemGrid;
        }
    }

    //расположить предмет на сетке 
    private void PlaceItem(Vector2Int titleGridPosition)
    {
        if(IsTreid && buferGrid.chestKeeper != SelectedItemGrid.chestKeeper)
        {
            PlaceItemInTrade();
        }
        else
        {
            PlaceItemIsNotTraded(titleGridPosition);
        }
    }

    //расположить вне торговли
    private void PlaceItemIsNotTraded(Vector2Int titleGridPosition)
    {
        bool complete =  SelectedItemGrid.PlaceItem(selectedItem, titleGridPosition.x, titleGridPosition.y, ref overlapItem);        
        
        if(complete)
        {
            if(SelectedItemGrid.isSingle)
            {
                PutOnClothesOnBody(selectedItem.itemData.GetItemTypeIndex());
            }
            
            selectedItem = null;
            
            if(overlapItem != null)
            {
                selectedItem = overlapItem;
            
                overlapItem = null;
            
                rectTransform = selectedItem.GetComponent<RectTransform>();
            
                rectTransform.SetAsLastSibling();
            }
        }
    }

    //расположить при торговли
    private void PlaceItemInTrade()
    {
        if(SelectedItemGrid.chestKeeper.money < selectedItem.itemData.price)
        {
            SelectedItemGrid.NotEnoughMoneyAnimation();
            return;
        } 

        SelectedItemGrid.chestKeeper.money -= selectedItem.itemData.price;
        buferGrid.chestKeeper.money += selectedItem.itemData.price;

        buferGrid.chestKeeper.UpdateMoney();
        SelectedItemGrid.chestKeeper.UpdateMoney();

        InventoryItem buferItem = selectedItem;
        CreateAndInsertItem(buferItem.itemData, selectedItemGrid, selectedItem.Amount);
        Destroy(buferItem.gameObject);
        selectedItem = null;
    }

    //надеть одежду 
    private void PutOnClothesOnBody(int index)
    {
        SelectedItemGrid.GetSetCharacter().itemGroups[0].items[index].prefab = SelectedItemGrid.GetItem(0, 0).itemData.prefab;
        SelectedItemGrid.GetSetCharacter().AddItem(SelectedItemGrid.GetSetCharacter().itemGroups[0], index);
    }

    //снять одежду
    private void TakeOffClothes(int index)
    {
        SelectedItemGrid.GetSetCharacter().RemoveItem(SelectedItemGrid.GetSetCharacter().itemGroups[0], index);
        SelectedItemGrid.GetSetCharacter().itemGroups[0].items[index].prefab = null;
    }

    //метод выкинуть предмет
    public void DropItem()
    {
        if(selectedItem)
        {
            Destroy(selectedItem.gameObject);
            
            Instantiate(selectedItem.itemData.prefab, 
            new Vector3(player.transform.position.x + 1, player.transform.position.y + 1, player.transform.position.z), Quaternion.identity);
            
            selectedItem = null;
        }
    }
}
