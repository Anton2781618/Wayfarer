using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//класс занимается подсветкой зоны куда мы будем помещать итем
public class InventoryIHighLight : MonoBehaviour
{
    [SerializeField] private RectTransform highLighter;

    //показать подсветку
    public void Show(bool value)
    {
        highLighter.gameObject.SetActive(value);
        
        highLighter.SetAsFirstSibling();
    }

    //устанавить размер подсветки
    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.WIDTH * ItemGrid.titleSizeWidth;
        size.y = targetItem.HEIGHT * ItemGrid.titleSizeHeight;
        highLighter.sizeDelta = size;
    }

    //установить позицию подсветки
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem,
                targetItem.onGridPositionX,
                targetItem.onGridPositionY);

        highLighter.localPosition = pos;

    }

    //установить позицию подсветки
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highLighter.localPosition = pos;
    }

    //установить родителя подсветки
    public void SetParent(ItemGrid targetGrid)
    {
        if(targetGrid == null){return; }
        highLighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }
}
