using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;

//класс является системой управления всех ивентарей, основной функцианал инвентарей находится тут
public class InventoryController : MonoBehaviour
{
    //инвнтарь игрока
    public Chest playerChest;
    private ItemGrid buferGrid;
    
    //выбраный инвентарь
    public Chest selectedChest{get; set;}
    public bool IsTreid{get; set;} = false;
    private ItemGrid selectedItemGrid;
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
    public InventoryIHighLight inventoryIHighLight;
    private PLayerController player;

    private void Start() 
    {
        canvasTransform = transform.root;

        // inventoryIHighLight = GetComponent<InventoryIHighLight>();  

        player = FindObjectOfType<PLayerController>();
        
        // playerChest = player.GetComponent<Chest>();
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
        
        InsertItemOnGrid(itemToInsert, selectedItemGrid);
    }

    private void InsertItemOnGrid(InventoryItem itemToInsert, ItemGrid grid)
    {
        Vector2Int? posOnGrid = grid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null) 
        {
            Debug.Log($"На сетке {grid} нет места для {itemToInsert.itemData.title}");
            Debug.Log("ВНИМАНИЕ! Надо переделать так что бы сначало проверялась сетка на наличие места, а потом ставился итем");
            Destroy(itemToInsert.gameObject);

            return;
        }

        grid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    //создать физически итем и установить его на сетку 
    public void CreateAndInsertItem(ItemData itemData, ItemGrid grid, int amount)
    {
        Debug.Log($"Создан предмет {itemData.title}");
        
        CreateItem(itemData, grid, amount);
        
        InventoryItem itemToInsert = selectedItem;
        
        selectedItem = null;
        
        InsertItemOnGrid(itemToInsert, grid);
    }

    public Chest GetPlayerChest() => playerChest;

    private Vector2Int _oldPosition;
    private InventoryItem _itemToHighLight;

    //метод подсветки предмета
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTitleGridPosition();
        if(_oldPosition == positionOnGrid){return;}
         
        _oldPosition = positionOnGrid;
        if(selectedItem == null)
        {
            _itemToHighLight = SelectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            
            if(_itemToHighLight != null)
            {
                // StartCoroutine(WaitAndOpenInfo());
                GameManager.Instance.UIManager.GetDescriptionWindowUI().SetDescript(_itemToHighLight);

                inventoryIHighLight.Show(true);
                inventoryIHighLight.SetSize(_itemToHighLight);
                inventoryIHighLight.SetPosition(SelectedItemGrid, _itemToHighLight);
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
        
        RectTransform rect = GameManager.Instance.UIManager.GetDescriptionWindowUI().GetRectTransform();

        SetPositionPopap(rect);

        GameManager.Instance.OpenDescriptInfo(true);
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
        inventoryItem.Setup(items[selectedItemID], playerChest.GetChestGrid());
    }

    private void CreateItem(ItemData itemData, ItemGrid grid, int amount)
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        inventoryItem.Amount = amount;
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        inventoryItem.Setup(itemData, grid);
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
        
        GameManager.Instance.SwithContextMenu(false);
        
        GameManager.Instance.OpenDescriptInfo(false);
        
        
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
            
        GameObject contextMenu = GameManager.Instance.SwithContextMenu(true);

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
        Debug.Log(SelectedItemGrid + " / " + selectedItem);
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
        if(IsTreid && buferGrid.chest != SelectedItemGrid.chest)
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
        bool complete = SelectedItemGrid.PlaceItem(selectedItem, titleGridPosition.x, titleGridPosition.y, ref overlapItem);        
        
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
        if(SelectedItemGrid.chest.money < selectedItem.itemData.price)
        {
            GameManager.Instance.UIManager.GetPlayerInventoryWindowUI().NotEnoughMoneyAnimation();

            return;
        } 

        SelectedItemGrid.chest.money -= selectedItem.itemData.price;

        buferGrid.chest.money += selectedItem.itemData.price;

        buferGrid.chest.UpdateMoney();

        SelectedItemGrid.chest.UpdateMoney();

        InventoryItem buferItem = selectedItem;

        CreateAndInsertItem(buferItem.itemData, selectedItemGrid, selectedItem.Amount);

        buferItem.DestructSelf();

        selectedItem = null;
    }

    //надеть одежду 
    private void PutOnClothesOnBody(int index)
    {
        playerChest.Clothes.items[index].Prefab = SelectedItemGrid.GetItem(0, 0).itemData.prefab;
        
        playerChest.Clothes.AddItem(index);
    }

    //снять одежду
    private void TakeOffClothes(int index)
    {
        playerChest.Clothes.RemoveItem(index);

        playerChest.Clothes.items[index].Prefab = null;
    }

    //метод выкинуть предмет
    public void DropItem()
    {
        if(selectedItem)
        {
            GameManager.Instance.RemoveUsableObject(selectedItem.gameObject);

            selectedItem.DestructSelf();
            
            Instantiate(selectedItem.itemData.prefab, 
                new Vector3(player.transform.position.x + 1, player.transform.position.y + 1, player.transform.position.z),
                Quaternion.identity);
            
            selectedItem = null;
        }
    }
}
