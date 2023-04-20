using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using static Cinemachine.CinemachineBrain;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UIManager UIManager;
    private Dictionary<GameObject, ICanUse> _allUsableObjects = new Dictionary<GameObject, ICanUse>();
    [SerializeField] private GameObject[] uiWindows;
    public PLayerController pLayerController{get; private set;}
    private InventoryController inventoryController;
    public CinemachineBrain cameraControll {get; private set;}
    public CinemachineFreeLook cinemachine {get; private set;}
    public bool isControlingPlayer {get; private set;} = true;

    private void Awake() 
    {
        Instance = this;

        Cursor.visible = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        inventoryController = UIManager.GetPlayerInventoryWindowUI().GetInventoryController();
        
        pLayerController = FindObjectOfType<PLayerController>();     

        cameraControll = FindObjectOfType<CinemachineBrain>();

        cinemachine = FindObjectOfType<CinemachineFreeLook>();

        CloseAllWindows();
    }

    //зарегестрировать юнита в соварь
    public void RegistrateUnit(ICanUse unit) => _allUsableObjects.Add(unit.transform.gameObject, unit);

    //получить юнита из словаря
    public ICanUse GetUsableObject(GameObject objectUse) => _allUsableObjects[objectUse];

    public void RemoveUsableObject(GameObject objectUse) => _allUsableObjects.Remove(objectUse);

    private void Update() 
    {
        InputHandler();
    }

    [ContextMenu("Показать словарь юзабельных объектов")]
    public void tra()
    {
        foreach (var item in _allUsableObjects)
        {
            Debug.Log(item.Key);
        }
    }

    ///<summary>
    ///обработчик нажатий, запускает метод в зависимости от кнопки 
    ///</summary>
    private void InputHandler()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(UIManager.GetDialogueWindow().isActiveAndEnabled) return;

            inventoryController.IsTreid = false;

            OpenWindowUI(UIManager.GetPlayerInventoryWindowUI().gameObject, !UIManager.GetPlayerInventoryWindowUI().gameObject.activeSelf);
            
            OpenWindowUI(UIManager.GetDialogueWindow().gameObject, false);
            
            SwithContextMenu(false);
            
            OpenDescriptInfo(false);
            
            inventoryController.SelectedItemGrid = null;
            
            if(UIManager.GetNpcInventoryWindowUI().gameObject.activeSelf)
            {
                OpenWindowUI(UIManager.GetNpcInventoryWindowUI().gameObject, !UIManager.GetNpcInventoryWindowUI().gameObject.activeSelf);

                inventoryController.selectedChest = null;
            }

            //при открытии инвентаря создаем предметы внутри инвентаря
            if (UIManager.GetPlayerInventoryWindowUI().gameObject.activeSelf) inventoryController.GetPlayerChest().OpenPlayerInventory();
        }  

        if(Input.GetKeyDown(KeyCode.C))
        {
            UIManager.GetStatsWindowUI().SetStates(pLayerController.GetStats());
            
            OpenWindowUI(UIManager.GetStatsWindowUI().gameObject, !UIManager.GetStatsWindowUI().gameObject.activeSelf);
        }  

        if(Input.GetKeyDown(KeyCode.F1))
        {
            OpenWindowUI(UIManager.GetHelpWindow().gameObject, !UIManager.GetHelpWindow().gameObject.activeSelf);
        }  

        if(Input.GetKeyUp(KeyCode.Escape))  
        {
            if(!UIManager.GetDialogueWindow().gameObject.activeSelf)CloseAllWindows();
        }
    }

    ///<summary>
    /// закрыть все панели
    ///</summary>
    public void CloseAllWindows()
    {
        inventoryController.IsTreid = false;

        foreach (var window in uiWindows)
        {
            OpenWindowUI(window);
        }
    }

    ///<summary>
    /// открыть сундук
    ///</summary>
    public void OpenChest()
    {
        OpenWindowUI(UIManager.GetNpcInventoryWindowUI().gameObject, true);

        OpenWindowUI(UIManager.GetPlayerInventoryWindowUI().gameObject, true);
    }

    ///<summary>
    /// открыть/закрыть контекстное меню
    ///</summary>
    public GameObject SwithContextMenu(bool value)
    {
        OpenWindowUI(UIManager.GetContextMenu().gameObject, value);

        return UIManager.GetContextMenu().gameObject;
    }

    public void OpenDescriptInfo(bool value) => OpenWindowUI(UIManager.GetDescriptionWindowUI().gameObject, value);

    // закрыть окно вызывается кнопкой
    public void CloseUIPanel(GameObject value) => OpenWindowUI(value);

    ///<summary>
    /// метод вклюает/выключает переданый UI
    ///</summary>
    public void OpenWindowUI( GameObject panel, bool value = false)
    {
        panel.SetActive(value);

        TogglePLayerControl(value);
    }

    //метод отключает контроль у камеры и включает мышку 
    private void TogglePLayerControl(bool value)
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
        BlockCamera(value);
        
        isControlingPlayer = !value;
        
        Cursor.visible = value;
        
        Cursor.lockState = cursorBlock ? CursorLockMode.Locked : CursorLockMode.None;
    }

    //метод парализует камеру
    public void BlockCamera(bool value)
    {
        cinemachine.m_XAxis.m_MaxSpeed = value == false ? 300.0f : 0.0f;

        cinemachine.m_YAxis.m_MaxSpeed = value == false ? 2.0f : 0.0f;
    }
}
