using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindowUI : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;

    public InventoryController GetInventoryController() => inventoryController;
}
