using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using static Cinemachine.CinemachineBrain;

public class GameManager : MonoBehaviour
{
    public GameObject Prefab;
    public static GameManager Instance;
    [SerializeField] private GameObject[] uiWindows;
    public PLayerController pLayerController{get; private set;}
    private InventoryController inventoryController;
    private AbstractBehavior targetHumanForHelp;
    private Transform targetItemForHelp;
    [SerializeField] private LayerMask triggerMaskForHelp;
    public CinemachineBrain cameraControll {get; private set;}
    public CinemachineFreeLook cinemachine {get; private set;}
    public bool isControlingPlayer {get; private set;} = true;

    public enum windowsUI { HelpUI, InventoryUI, StatsUI, ChestUI, ContextMenuUI, InfoItemUI, DialogUI }

    private void Awake() 
    {
        Cursor.visible = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        inventoryController = GetComponent<InventoryController>();
        
        Instance = this;
        
        pLayerController = FindObjectOfType<PLayerController>();     

        cameraControll = FindObjectOfType<CinemachineBrain>();

        cinemachine = FindObjectOfType<CinemachineFreeLook>();

        CloseAllUiPanels();
    }

    private void Update() 
    {
        InputHandler();
    }

    ///<summary>
    ///обработчик нажатий, запускает метод в зависимости от кнопки 
    ///</summary>
    private void InputHandler()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(GetDialogWindow().isActiveAndEnabled) return;

            inventoryController.IsTreid = false;

            CollUIPanel(uiWindows[(int)windowsUI.InventoryUI], !uiWindows[(int)windowsUI.InventoryUI].activeSelf);
            CollUIPanel(uiWindows[(int)windowsUI.DialogUI], false);
            
            SwithContextMenu(false);
            SwithInfoItem(false);
            
            inventoryController.SelectedItemGrid = null;
            
            if(uiWindows[(int)windowsUI.ChestUI].activeSelf)
            {
                CollUIPanel(uiWindows[(int)windowsUI.ChestUI], !uiWindows[(int)windowsUI.ChestUI].activeSelf);
                inventoryController.selectedChest = null;
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

    ///<summary>
    /// закрыть все панели
    ///</summary>
    public void CloseAllUiPanels()
    {
        inventoryController.IsTreid = false;
        foreach (var window in uiWindows)
        {
            CollUIPanel(window);
        }
    }


    ///<summary>
    /// открыть сундук
    ///</summary>
    public void OpenChest()
    {
        CollUIPanel(uiWindows[(int)windowsUI.ChestUI], true);
        CollUIPanel(uiWindows[(int)windowsUI.InventoryUI], true);
    }

    ///<summary>
    /// открыть/закрыть контекстное меню
    ///</summary>
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

    ///<summary>
    /// метод вклюает/выключает переданый UI
    ///</summary>
    public void CollUIPanel( GameObject obj, bool value = false, bool tagetoff = true)
    {
        obj.SetActive(value);

        SetPLayerController(value);
        
        if(tagetoff)TargetOff();
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

        BlockPlayerControl(value, !value);
    }

    //метод отключает контроль у камеры и включает мышку
    public void BlockPlayerControl(bool value, bool cursorBlock)
    {
        SwithCameraEnabled(!value);
        
        SetIsControlingPlayer(!value);
        
        Cursor.visible = value;
        
        Cursor.lockState = cursorBlock ? CursorLockMode.Locked : CursorLockMode.None;
    }

    //метод парализует камеру
    public void SwithCameraEnabled(bool value)
    {
        if(!value)
        {
            cinemachine.m_XAxis.m_MaxSpeed = 0.0f;
            cinemachine.m_YAxis.m_MaxSpeed = 0.0f;
        }
        else
        {
            cinemachine.m_XAxis.m_MaxSpeed = 300.0f;
            cinemachine.m_YAxis.m_MaxSpeed = 2.0f;
        }
        
    }

    public void SetIsControlingPlayer(bool value)
    {
        isControlingPlayer = value;
    }

    //метод дает ссылку на диалоговое окно
    public DialogueWindowUI GetDialogWindow()
    {
        return uiWindows[(int)windowsUI.DialogUI].GetComponent<DialogueWindowUI>();
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

        targetHumanForHelp.TakeDamage(pLayerController, targetHumanForHelp.GetCurHP());
    }

    public void RestartScene(string value)
    {
        SceneManager.LoadScene(value);
    }

    public void CommandTakeTarget()
    {
        targetItemForHelp.GetComponent<ItemOnstreet>().TakeItem(pLayerController.Chest);
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

    public void CreateNewEnemy()
    {
        GameObject enemy = Instantiate(Prefab, Prefab.transform.position, Quaternion.identity);
        enemy.SetActive(true);
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
