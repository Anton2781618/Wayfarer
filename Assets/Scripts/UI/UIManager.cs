using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HelpWindowUI _helpWindowUI;
    [SerializeField] private DialogueWindowUI _dialogueWindowUI;
    [SerializeField] private PlayerInventoryWindowUI _inventoryWindowUI;
    [SerializeField] private NpcInventoryWindowUI _сhestWindowUI;
    [SerializeField] private ItemContextMenu _contextMenu;
    [SerializeField] private DescriptionWindowUI  _descriptionWindowUI;
    [SerializeField] private StatsWindowUI statsWindowUI;

    public HelpWindowUI GetHelpWindow() => _helpWindowUI;
    public DialogueWindowUI GetDialogueWindow() => _dialogueWindowUI;
    public PlayerInventoryWindowUI GetPlayerInventoryWindowUI() => _inventoryWindowUI;
    public NpcInventoryWindowUI GetNpcInventoryWindowUI() => _сhestWindowUI;
    public ItemContextMenu GetContextMenu() => _contextMenu;
    public DescriptionWindowUI  GetDescriptionWindowUI() => _descriptionWindowUI;
    public StatsWindowUI GetStatsWindowUI() => statsWindowUI;
    
}
