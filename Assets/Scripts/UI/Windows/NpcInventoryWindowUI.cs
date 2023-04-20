using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInventoryWindowUI : MonoBehaviour
{
    [SerializeField] private ItemGrid _npcGrid; 

    public ItemGrid GetNpcInventoryGrid() => _npcGrid;
}
