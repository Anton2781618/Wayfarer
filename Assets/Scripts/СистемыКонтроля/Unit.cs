using System;
using System.Collections;
using System.Collections.Generic;
using DS;
using DS.Enumerations;
using DS.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static ICanTakeDamage;
using static Initializer;


public class Unit : AbstractBehavior
{
    private delegate int Operation();
    [SerializeField] private DSDialogue dSDialogue;
    [SerializeField] protected Canvas unitCanvas;
    [SerializeField] protected AI aI = new AI();
    [SerializeField] protected DSAction currentAction;
    private ModelDate CurrentModelData;

    public override void Init()
    {
        chest.InitChest(Initializer.singleton.InitObject(InitializerNames.Инвентарь_Моб).GetComponent<ItemGrid>());
        aI.Init(this, anim);
    }    

    private void Update() => aI.Analyzer();

    public override void ShowOutline(bool value)
    {
        if(state == States.Мертв)
        {
            SowHealthBar(false);    
            return;
        }
        SowHealthBar(value);
    }    

    public override void SowHealthBar(bool value)
    {
        if(unitCanvas == null)
        {
            Debug.Log("у " + transform.name + " не установлен unitCanvas");
            return;
        }
        unitCanvas.gameObject.SetActive(value);
    }

    public States GetUnityState() => state;

    //установить действие
    public int SetAction(DSAction action, ModelDate modelDate)
    {
        Debug.Log("устанавливаю задачу " + action);
        
        CurrentModelData = modelDate;
        
        currentAction = action;
        
        return 0;
    }
    //выполнить текущую задачу
    public void ExecuteCurrentCommand()
    {
        if(unitCanvas.gameObject.activeSelf)RotateCanvas();

        Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, currentAction.ToString());
        operation.Invoke();
    }

    public override void Use() => CommandStartDialogue();

    #region [rgba(908,300,207,0.02)] Разные вспомогательные методы -----------------------------------------------------------------------------------------------------//

    private void MoveToPoint(Vector3 point)
    {
        FaceToPoint(point);

        agent.SetDestination(point);     

        SetAnimationRun(agent.remainingDistance > agent.stoppingDistance);
    }

    //поварачивает канвас юнита лицом к игроку
    private void RotateCanvas()
    {
        if(unitCanvas.transform.rotation != Camera.main.transform.rotation)
        {
            unitCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void SetCompleteCommand(int dialogueIndex = 0)
    {
        this.currentAction = DSAction.CommandStandStill;

        Debug.Log("задача выполнена! Перехожу к следующей задаче.");
        
        aI.NextStage(dialogueIndex);
    }

    public override void Die()
    {
        base.Die();
        SowHealthBar (false);
    }

    private void FaceToPoint(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void SetTarget(ICanUse newTarget) => target = newTarget;

    public void SetAnimationRun(bool value) => anim.SetBool("walk", value);
    
        
    #endregion Разные вспомогательные методы КОНЕЦ ---------------------------------------------------------------------------------------------------------------------//

    #region [rgba(108,300,207,0.02)] Комманды для управления NPC -------------------------------------------------------------------------------------------------------//
    public Terrain currentTerrain;//участок земли
    private bool isWalken = false;
    private Vector3 newPintToWalke;

    public GameObject TestPrefab;
    public List<MapSquare> map = new List<MapSquare>();
    [ContextMenu("CreateGrid")]
    public void CreateMapGrid()
    {
        float offset = 12.5f;
        map.Clear();

        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                Vector2 pos = new Vector2( currentTerrain.transform.position.x + TestPrefab.transform.localScale.x  * x + offset, currentTerrain.transform.position.z + TestPrefab.transform.localScale.z * z + offset);
                Vector2 size = new Vector2(25, 25);

                map.Add(new MapSquare(pos, size, Instantiate(TestPrefab)));
            }
        }
    }

    public void CalculPos()
    {
        foreach (var pref in map)
        {
            if(transform.position.x > pref.pos.x - 12.5f&& 
            transform.position.x < pref.pos.x + pref.size.x - 12.5f &&
            transform.position.z > pref.pos.y - 12.5f && 
            transform.position.z < pref.pos.y + pref.size.y - 12.5f)
            {
                Destroy(pref.cubePref);
                map.Remove(pref);
                return;
            }
        }
    }

    private int CommandFindTheTarget()
    {
        if(!agent.pathPending && agent.remainingDistance < agent.stoppingDistance && !isWalken)
        {
            isWalken = true;
            // newPint = SetRandomPointInSquare(currentTerrain);
            int x = (int)map[UnityEngine.Random.Range(0, map.Count - 1)].pos.x;
        
            int z = (int)map[UnityEngine.Random.Range(0, map.Count - 1)].pos.y;
            
            float y = currentTerrain.terrainData.GetHeight(x,z);

            newPintToWalke = new Vector3(x, y, z);
        }
        else
        {
            isWalken = false;
            MoveToPoint(newPintToWalke);
            CalculPos();
        }

        if(aI.GetEyes().visileTargets.Count > 0)
        {
            target = aI.GetEyes().visileTargets[0].GetComponent<ICanUse>();
            Debug.Log("найден таргет " + target);
            SetCompleteCommand();
        }

        return 0;
    }    

    private Vector3 SetRandomPointInSquare(Terrain square)
    {
        int x = Mathf.CeilToInt(UnityEngine.Random.Range(0, currentTerrain.terrainData.bounds.size.x));
        
        int z = Mathf.CeilToInt(UnityEngine.Random.Range(0, currentTerrain.terrainData.bounds.size.z));
        
        float y = currentTerrain.terrainData.GetHeight(x,z);

        return new Vector3(x, y + 1, z);
    }

    //стоять на месте
    private int CommandStandStill()
    {
        anim.SetBool("walk", false);

        agent.SetDestination(this.transform.position);
        
        return 0;
    }

    //метод команда атакавать таргет (привязана к системе диалогов)
    public int CommandAttackTheTarget()
    {
        if(target == null)
        {
            Debug.Log("У " + transform.name + " не установлен таргет");

            return 0;
        }

        AbstractBehavior buferTarget = target as AbstractBehavior;

        if(buferTarget.GetCurrentUnitState() == States.Мертв)
        {
            state = States.Патруль;
            
            SetCompleteCommand();
            
            return 0;
        }
        
        MoveToPoint(target.transform.position);
        
        anim.SetBool("Hit", agent.remainingDistance <= agent.stoppingDistance && target != null);
        
        return 0;
    }

    public int CommandRetreat()
    {
        Debug.Log("Отступаю");
        return 0;
    }

    private int CommandTrading()
    {
        chest.StartTrading();

        SetCompleteCommand();
        
        return 0;
    }

    private int CommandStartDialogue()
    {
        dSDialogue.SetDialog(CurrentModelData.dialogue);

        dSDialogue.StartDialogue(this);

        SetCompleteCommand();

        return 0;
    }

    private int CommandPlayerGiveMoney()
    {
        chest.ReceiveMoney(GameManager.singleton.pLayerController.chest, (int)CurrentModelData.number);
        
        SetCompleteCommand();
        
        return 0;
    }

    private int CommandMoveToTarget()
    {
        if(target == null)
        {
            Debug.Log("у NPC не установлен таргет");
        }

        MoveToPoint(target.transform.position);

        CheckAndSwitchStage();
        
        return 0;
    }
    
    private int CommandMoveToCoordinates()
    {
        MoveToPoint(CurrentModelData.pos);
        
        CheckAndSwitchStage();
        
        return 0;
    }
    
    private void CheckAndSwitchStage()
    {
        if(!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            SetCompleteCommand();
        }
    }

    private int CommandCheckTargetInventoryForItem()
    {
        if(target == null) Debug.Log("нет таргета");

        SetCompleteCommand(target.transform.GetComponent<Chest>().CheckInventoryForItems(CurrentModelData.itemData));

        return 0;
    }

    [ContextMenu("пуск")]
    public void Action2() => aI.StartSolution();
    
    #endregion Комманды для управления NPC КОНЕЦ --------------------------------------------------------------------------------------------------------------------//
}

[Serializable]
public class ModelDate
{
    public float number;
    public Vector3 pos;
    public ItemData itemData;
    public DSDialogueContainerSO dialogue;
    public LayerMask targetMask;
}

//класс описывает квадрат распологаемый на карте
[Serializable]
public class MapSquare
{
    public Vector2 pos;
    public Vector2 size;
    public GameObject cubePref;

    public MapSquare(Vector2 pos, Vector2 size, GameObject cubePref)
    {
        this.pos = pos;
        this.size = size;

        cubePref.transform.position = new Vector3(pos.x, -10, pos.y);
        cubePref.transform.localScale = new Vector3(size.x, 1, size.y);
        this.cubePref = cubePref;
    }
}