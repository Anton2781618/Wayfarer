using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    [SerializeField] private GameObject[] uiWindows;
    private PLayerController pLayerController;
    private InventoryController inventoryController;
    private AbstractBehavior targetHumanForHelp;
    private Transform targetItemForHelp;
    [SerializeField] private LayerMask triggerMaskForHelp;
    public CinemachineBrain cameraControll {get; private set;}
    public bool isControlingPlayer {get; private set;} = true;
    private UIDialogueTransfer uIDialogueTransfer;

    //когда мы торгуем или ведем диалог сюда устанавливаем собеседника, для того что бы снять с него флаг ожедания
    public Unit TargetForWaight{get; set;}

    public enum windowsUI { HelpUI, InventoryUI, StatsUI, ChestUI, ContextMenuUI, InfoItemUI, DialogUI }

    private void Start() 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        inventoryController = GetComponent<InventoryController>();
        singleton = this;
        pLayerController = FindObjectOfType<PLayerController>();     

        //тут мы ставим главного героя владельцем его инвентаря  
        inventoryController.GetPlayerChest().GetChestGrid().wearer = pLayerController;

        cameraControll = FindObjectOfType<CinemachineBrain>();

        uIDialogueTransfer = uiWindows[(int)windowsUI.DialogUI].GetComponent<UIDialogueTransfer>();

        CloseAllUiPanels();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryController.IsTreid = false;

            CollUIPanel(uiWindows[(int)windowsUI.InventoryUI], !uiWindows[(int)windowsUI.InventoryUI].activeSelf);
            CollUIPanel(uiWindows[(int)windowsUI.DialogUI], false);
            
            SwithContextMenu(false);
            SwithInfoItem(false);
            
            inventoryController.SelectedItemGrid = null;
            
            if(uiWindows[(int)windowsUI.ChestUI].activeSelf)
            {
                CollUIPanel(uiWindows[(int)windowsUI.ChestUI], !uiWindows[(int)windowsUI.ChestUI].activeSelf);
                inventoryController.TheChest = null;
            }

            //при открытии инвентаря создаем предметы внутри инвентаря
            if(uiWindows[(int)windowsUI.InventoryUI].activeSelf)inventoryController.GetPlayerChest().OpenPlayerInventory();
        }  

        if(Input.GetKeyDown(KeyCode.C))
        {
            uiWindows[(int)windowsUI.StatsUI].GetComponent<WindowTranser>().SetTextsTransfers(pLayerController.GetStats());
            CollUIPanel(uiWindows[(int)windowsUI.StatsUI], !uiWindows[(int)windowsUI.StatsUI].activeSelf);
        }  

        if(Input.GetKeyDown(KeyCode.F1))
        {
            CollUIPanel(uiWindows[(int)windowsUI.HelpUI], !uiWindows[(int)windowsUI.HelpUI].activeSelf);
        }  

        if(Input.GetKeyDown(KeyCode.Mouse0))  
        {
            HighlightObject();
        }

        if(Input.GetKeyUp(KeyCode.Escape))  
        {
            CloseAllUiPanels();
        }
    }

    public UIDialogueTransfer GetDialogueTransfer()
    {
        return uIDialogueTransfer;
    }

    public void CloseAllUiPanels()
    {
        inventoryController.IsTreid = false;
        foreach (var window in uiWindows)
        {
            CollUIPanel(window);
        }
    }

    public void OpenChest()
    {
        CollUIPanel(uiWindows[(int)windowsUI.ChestUI], true);
        CollUIPanel(uiWindows[(int)windowsUI.InventoryUI], true);
    }

    public GameObject SwithContextMenu(bool value)
    {
        CollUIPanel(uiWindows[(int)windowsUI.ContextMenuUI], value, false);
        return uiWindows[(int)windowsUI.ContextMenuUI];
    }

    public void SwithInfoItem(bool value)
    {
        CollUIPanel(uiWindows[(int)windowsUI.InfoItemUI], value, false);
    }

    public GameObject GetContextMenu()
    {
        return uiWindows[(int)windowsUI.ContextMenuUI];
    }
    public GameObject GetInfoItem()
    {
        return uiWindows[(int)windowsUI.InfoItemUI];
    }

    public void CloseUIPanel(GameObject value)
    {
        CollUIPanel(value);
    }

    //метод вклюает/выключает переданый UI
    public void CollUIPanel( GameObject obj, bool value = false, bool tagetoff = true)
    {
        obj.SetActive(value);
        SetPLayerController(value);
        if(tagetoff)TargetOff();

        if(!value && TargetForWaight !=null)
        {
            TargetForWaight.SetOffwaitAndNextStage();
            TargetForWaight = null;
        }
    }

    //метод отключает контроль у камеры и включает мышку 
    private void SetPLayerController(bool value)
    {
        foreach (var windw in uiWindows)
        {
            if(windw.activeSelf && value == false)
            {
                return;
            }
        }
        
        SwithCameraEnabled(!value);
        SetIsControlingPlayer(!value);
        Cursor.visible = value;
        Cursor.lockState = !value ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SwithCameraEnabled(bool value)
    {
        cameraControll.enabled = value;
    }

    public void SetIsControlingPlayer(bool value)
    {
        isControlingPlayer = value;
    }

    public void CommandReviveTarget()
    {
        if(!CheckTarget())return;

        targetHumanForHelp.Revive();
    }
    public void CommandAttackTarget()
    {
        if(!CheckTarget())return;

        Unit tar = (Unit)targetHumanForHelp;
        tar.CommandAttackTheTarget();
    }

    public void CommandHealingTarget()
    {
        if(!CheckTarget())return;

        targetHumanForHelp.Healing(1000000);
    }

    public void CommandKillTarget()
    {
        if(!CheckTarget())return;

        targetHumanForHelp.TakeDamage(targetHumanForHelp.GetCurHP());
    }

    public void RestartScene(string value)
    {
        SceneManager.LoadScene(value);
    }

    public void CommandTakeTarget()
    {
        targetItemForHelp.GetComponent<ItemOnstreet>().TakeItem(inventoryController);
        targetItemForHelp = null;
        Debug.Log("takeTarget");
    }

    private void TargetOff()
    {
        if(targetHumanForHelp)
        {
            targetHumanForHelp.transform.GetComponent<Outline>().enabled = false;
            targetHumanForHelp.SowHealthBar (false);
            targetHumanForHelp = null;
        }
    }

    private void HighlightObject()
    {
        if (EventSystem.current != null) 
        {
            if (EventSystem.current.IsPointerOverGameObject()) 
            {
                return;
            }
        }

        if(uiWindows[(int)windowsUI.HelpUI].activeSelf)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            TargetOff();

            if(Physics.Raycast(ray, out hit, 100, triggerMaskForHelp))
            {
                Debug.Log(hit.transform.name + " " );
                if(hit.transform.root.GetComponent<AbstractBehavior>())
                {
                    
                    targetHumanForHelp = hit.transform.root.GetComponent<AbstractBehavior>();
                    targetHumanForHelp.SowHealthBar (true);
                    hit.transform.root.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    targetItemForHelp = hit.transform;
                    hit.transform.root.GetComponent<Outline>().enabled = true;
                }
            }
        }
    }

    private bool CheckTarget()
    {
        if(!targetHumanForHelp)
        {
            Debug.Log("GameManager! Нет таргета");
            return false;
        }
        return true;
    }
}
