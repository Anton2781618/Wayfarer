using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//класс представляет из себя систему определения ячейки для тыкания в нее
[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   private InventoryController inventoryController;
   [SerializeField] private ItemGrid itemGrid;

    private void Start() 
    {
        inventoryController = GameManager.Instance.UIManager.GetPlayerInventoryWindowUI().GetInventoryController();

        itemGrid = GetComponent<ItemGrid>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = itemGrid;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = null;
    }
}
