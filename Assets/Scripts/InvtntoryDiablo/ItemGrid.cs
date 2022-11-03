using System.Collections;
using CartoonHeroes;
using UnityEngine;
using UnityEngine.UI;
using static ItemData;

//класс представляет из себя сетку с ячейками и данные о размерах
//устанавливается на UI сетки
public class ItemGrid : MonoBehaviour
{

    public const float titleSizeWidth = 32;
    public const float titleSizeHeight = 32;

    //это ссылка на того чей инвентарь
    public Chest chestKeeper {get; set;}
    public AbstractBehavior abstractBehavior;
    
    public Text moneyText;
    public Image ImageMoney;
    private InventoryItem[,] inventoryItemSlot;

    private RectTransform rectTransform;
    
    private Vector2 positionOnTheGrid = new Vector2();
    private Vector2Int titeGridPosition = new Vector2Int();

    [SerializeField] private int GridSizeWidth = 20; 
    [SerializeField] private int GridSizeHeight = 10; 
    [SerializeField] private ItemType gridForItemsType;
    [SerializeField] private SetCharacter setCharacter;

    private int TextX; 
    private int TextY;

    //поле определяет сетка для одного предмета или нет
    public bool isSingle = false;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();    
        Init(GridSizeWidth,GridSizeHeight);
    }

    public SetCharacter GetSetCharacter()
    {
        return setCharacter;
    }

    public ItemType GetGridForItemsType()
    {
        return gridForItemsType;
    }

    //устанавлмвает начальный размер сетки
    private void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * titleSizeWidth, height * titleSizeHeight);
        rectTransform.sizeDelta = size;
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    //метод возвращает позицию на ячейки по сетке
    public Vector2Int GetTitleGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
    
        titeGridPosition.x = (int)(positionOnTheGrid.x / titleSizeWidth);
        titeGridPosition.y = (int)(positionOnTheGrid.y / titleSizeHeight);

        return titeGridPosition;
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int heght = GridSizeHeight - itemToInsert.HEIGHT + 1;
        int wight = GridSizeWidth - itemToInsert.WIDTH + 1; 
        
        for (int y = 0; y < heght; y++)
        {
            for (int x = 0; x < wight ; x++)
            {
                if(CheckAvailabeSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y); 
                }
            }
        }

        return null;
    }

    //метод установить итем в слот
    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        // не можем расположить итем если он хотябы частично за сеткой
        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }

        if (OverLapTheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            if(!overlapItem.itemData.isSingle &&  !inventoryItem.itemData.isSingle)
            {
                UpdateAmount(inventoryItem, overlapItem);
            }
            CleanGridReference(overlapItem);
        }
        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    private void UpdateAmount(InventoryItem inventoryItem, InventoryItem overlapItem)
    {
        inventoryItem.amauntText.text = (inventoryItem.Amount += overlapItem.Amount).ToString();
        Destroy(overlapItem.gameObject);
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        Vector2 positionItem = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = positionItem;
    }

    public void NotEnoughMoneyAnimation()
    {
        StartCoroutine(MoneyAnimation());
    }

    private IEnumerator MoneyAnimation()
    {
        moneyText.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        moneyText.color = Color.black;    
        yield return new WaitForSeconds(0.2f);
        moneyText.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        moneyText.color = Color.black;        
        

        
    }
    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 positionItem = new Vector2();
        positionItem.x = posX * titleSizeWidth + titleSizeWidth * inventoryItem.WIDTH / 2;
        positionItem.y = -(posY * titleSizeHeight + titleSizeHeight * inventoryItem.HEIGHT / 2);
        return positionItem;
    }

    private bool CheckAvailabeSpace(int posX, int posY, int width, int height)
    {        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(inventoryItemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool OverLapTheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if(overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        if(overlapItem != inventoryItemSlot[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    //метод поднять итем
    public InventoryItem SelectIteme(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null) { return null; }

        CleanGridReference(toReturn);

        return toReturn;
    }

    public InventoryItem GetInventoryItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    private void CleanGridReference(InventoryItem toReturn)
    {
        for (int ix = 0; ix < toReturn.WIDTH; ix++)
        {
            for (int iy = 0; iy < toReturn.HEIGHT; iy++)
            {
                inventoryItemSlot[toReturn.onGridPositionX + ix, toReturn.onGridPositionY + iy] = null;
            }
        }
    }

    //метод проверка на позицию итема что все его части внутри сетки
    private bool PositionCheck(int posX, int posY)
    {
        if(posX < 0|| posY < 0)
        {
            return false;
        }

        if(posX >= GridSizeWidth || posY >= GridSizeHeight)
        {
            return false;
        }

        return true;
    }

    //проверка границ сетки, если позиция итема + его самая дальяя часть за сеткой то фалс
    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false) {return false;}

        posX += width - 1;
        posY += height - 1;

        if(PositionCheck(posX, posY) == false) {return false;}

        return true;
    }
}
