using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindowUI : MonoBehaviour
{
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private List<ItemGrid> _grids; 
    // [SerializeField] private Grid inventoryController;

    public InventoryController GetInventoryController() => _inventoryController;
    public List<ItemGrid> GetInventoryGrids() => _grids;
}
