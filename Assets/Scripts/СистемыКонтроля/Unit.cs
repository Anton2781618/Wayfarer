using System;
using System.Collections;
using System.Collections.Generic;
using DS;
using DS.Enumerations;
using DS.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static ICanTakeDamage;
using static Initializer;


public class Unit : AbstractBehavior
{
    public List<SolutionInfo> solutions;
    private delegate void Operation();
    [SerializeField] protected Canvas unitCanvas;
    [SerializeField] protected AI aI = new AI();
    [SerializeField] protected DSAction currentAction;
    private ModelDate CurrentModelData;

    //двигается ли персонаж сейчас
    private bool needMoving = false;
    private bool needCreateMap = true;
    private Vector3 PintToWalke;

    //участок земли 
    public Terrain currentTerrain;//!ждет доработки
    public List<MapSquare> map = new List<MapSquare>();
    


    public override void Init()
    {
        base.Init();
        
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
    public void SetAction(DSAction action, ModelDate modelDate)
    {
        Debug.Log("устанавливаю задачу " + action);
        
        CurrentModelData = modelDate;
        
        currentAction = action;
    }

    //выполнить текущую задачу
    public void ExecuteCurrentCommand()
    {
        if(unitCanvas.gameObject.activeSelf)RotateCanvas();

        Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, currentAction.ToString());

        operation.Invoke();
    }

    public override void Use(AbstractBehavior applicant) => CommandStartDialogue();

    #region [rgba(30,106,143, 0.05)] Разные вспомогательные методы -----------------------------------------------------------------------------------------------------//

    //двигаться к точке
    private void MoveToPoint(Vector3 point)
    {
        // FaceToPoint(point);

        agent.SetDestination(point);    

        SetAnimationRun(agent.remainingDistance > agent.stoppingDistance);
    }


    [ContextMenu("CreatePOints")]
    public Vector3 CreatePoints(Vector3 point)
    {
        NavMeshPath path = new NavMeshPath();

        NavMeshHit hit;

        Vector3 result = transform.position;

        if (NavMesh.SamplePosition(point, out hit, 5f, NavMesh.AllAreas)) 
        {
            result = hit.position;
        }

        UnityEngine.AI.NavMesh.CalculatePath(transform.position, result, UnityEngine.AI.NavMesh.AllAreas, path);

        for (int i = 0; i < path.corners.Length-1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i+1], Color.red, 10f);	
        }

        return result;
    }

    //создать карту для перемещения
    public void CreateMap()
    {
        float offset = 6.25f;

        map.ForEach(t => Destroy(t.cubePref));
        map.Clear();

        for (int x = 0; x < 8; x++)
        {
            for (int z = 0; z < 8; z++)
            {
                Vector2 size = new Vector2(12.5f, 12.5f);

                Vector2 pos = new Vector2 (currentTerrain.transform.position.x + size.x * x + offset, currentTerrain.transform.position.z + size.y * z + offset);

                // GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // primitive.transform.localScale = new Vector3(size.x, 0.5f, size.y);

                // map.Add(new MapSquare(pos, size, primitive));
                map.Add(new MapSquare(pos, size));
            }
        }
    }

    //метод вычисляет в каком квадрате мы находимся и удаляет этот квадрат из карты
    public void GetMyPosOnMap()
    {
        foreach (var pref in map)
        {
            if(transform.position.x > pref.pos.x - 6.25f && transform.position.x < pref.pos.x + pref.size.x - 6.25f &&
            transform.position.z > pref.pos.y - 6.25f && transform.position.z < pref.pos.y + pref.size.y - 6.25f)
            {
                Destroy(pref.cubePref);

                map.Remove(pref);

                return;
            }
        }
    }
    public Transform posGame;

    // найти случайную точку на квадрате в списку квадратов (на карте)
    private Vector3 GetNewRandomPointOnMap()
    {
        MapSquare square = map[UnityEngine.Random.Range(0, map.Count - 1)];

        int x = (int)square.pos.x;
        
        int z = (int)square.pos.y;

        float y = currentTerrain.SampleHeight(new Vector3(x, 0, z));

        posGame.position = new Vector3(x, y, z);

        return new Vector3(x, y, z);
    }

    //найти случайную точку на определенном террейне
    private Vector3 GetRandomPointInSquare(Terrain square)
    {
        int x = Mathf.CeilToInt(UnityEngine.Random.Range(0, currentTerrain.terrainData.bounds.size.x));
        
        int z = Mathf.CeilToInt(UnityEngine.Random.Range(0, currentTerrain.terrainData.bounds.size.z));
        
        float y = currentTerrain.terrainData.GetHeight(x,z);

        return new Vector3(x, y + 1, z);
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
        needCreateMap = true;

        this.currentAction = DSAction.CommandStandStill;

        Debug.Log("задача выполнена! Перехожу к следующей задаче.");
        
        aI.StartNextStage(dialogueIndex);
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

    private bool FindTarget()
    {
        Eyes eyes = aI.GetEyes();

        eyes.SetTargetMaskForEyes(CurrentModelData.targetMask);         

        if(eyes.visileTargets.Count > 0)
        {
            foreach (var item in eyes.visileTargets)
            {
                if(!eyes.SetTargetToMamry(item))
                {
                    eyes.mamryTargets.Add(item);
                }
            }
        }

        if(eyes.mamryTargets.Count > 0)
        {
            if(eyes.mamryTargets[0] == null)
            {
                eyes.mamryTargets.RemoveAt(0);

                return false;
            } 
            
            target = eyes.mamryTargets[0].GetComponent<ICanUse>();

            Debug.Log("найден таргет " + target);
            
            return true;
        }

        return false;
    }

    public void SetTarget(ICanUse newTarget) => target = newTarget;

    public void SetAnimationRun(bool value) => anim.SetBool("walk", value);
    
        
    #endregion Разные вспомогательные методы КОНЕЦ ---------------------------------------------------------------------------------------------------------------------//

    #region [rgba(30,106,143, 0.05)] Комманды для управления NPC -------------------------------------------------------------------------------------------------------//
    
    private void CommandFindTheTarget()
    {
        if(needCreateMap)
        {
            CreateMap();

            needCreateMap = false;
        }

        if(!agent.pathPending && agent.remainingDistance < agent.stoppingDistance && !needMoving)
        {
            PintToWalke = CreatePoints(GetNewRandomPointOnMap());

            needMoving = true;
        }
        else
        {
            MoveToPoint(PintToWalke);

            GetMyPosOnMap();

            needMoving = false;
        }
        
        if(FindTarget()) SetCompleteCommand();
    }

    private void TestTime()
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
     
        stopWatch.Start();

        // FindTarget();
        System.Threading.Thread.Sleep(100);

        stopWatch.Stop();

        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds,ts.Milliseconds / 10);

        Debug.Log("RunTime " + elapsedTime);
    }

    private void CommandHoldPositionFindTheTarget()
    {
        if(FindTarget()) SetCompleteCommand();
    }

    //стоять на месте
    private void CommandStandStill()
    {
        anim.SetBool("walk", false);

        agent.SetDestination(this.transform.position);
    }

    //метод команда атакавать таргет (привязана к системе диалогов)
    public void CommandAttackTheTarget()
    {
        if(target == null)
        {
            Debug.Log("У " + transform.name + " не установлен таргет! Или таргет не возможно атакавать");
            
            SetCompleteCommand();

            return;
        }
        
        AbstractBehavior buferTarget = (AbstractBehavior)target;

        if(buferTarget.GetCurrentUnitState() == States.Мертв)
        {
            state = States.Патруль;
            
            SetCompleteCommand();
            
            return;
        }
        
        MoveToPoint(target.transform.position);
        
        anim.SetBool("Hit", agent.remainingDistance <= agent.stoppingDistance && target != null);
    }

    public void CommandRetreat()
    {
        Debug.Log("Отступаю");
    }

    private void CommandTrading()
    {
        chest.StartTrading(this);

        SetCompleteCommand();
    }

    private void CommandStartDialogue()
    {
        aI.StartDialogue(CurrentModelData.dialogue);

        SetCompleteCommand();
    }

    private void CommandPlayerGiveMoney()
    {
        chest.ReceiveMoney(GameManager.singleton.pLayerController.chest, (int)CurrentModelData.number);
        
        SetCompleteCommand();
    }

    private void CommandMoveToTarget()
    {
        if(target == null)
        {
            Debug.Log("у NPC не установлен таргет");
        }

        MoveToPoint(target.transform.position);

        CheckAndSwitchStage();
    }
    
    private void CommandMoveToCoordinates()
    {
        MoveToPoint(CurrentModelData.pos);
        
        CheckAndSwitchStage();
    }
    
    private void CheckAndSwitchStage()
    {
        if(!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            SetCompleteCommand();
        }
    }

    private void CommandCheckTargetInventoryForItem()
    {
        if(target == null) Debug.Log("нет таргета");

        SetCompleteCommand(target.transform.GetComponent<Chest>().CheckInventoryForItems(CurrentModelData.itemData));
    }
    
    private void CommandCheckSelfInventoryForItem()
    {
        SetCompleteCommand(chest.CheckInventoryForItems(CurrentModelData.itemData));
    }

    private void CommandUseSelfInventoryItem()
    {
        InventoryItemInfo item = chest.GetInventoryForItemType(CurrentModelData.itemData.itemType);

        item.Use(this);

        chest.RemoveAtChestGrid(item);

        SetCompleteCommand();
    }

    private void CommandTakeItemFromTarget()
    {
        if(target == null) Debug.Log("нет таргета");

        Chest targetChest = target.transform.GetComponent<Chest>();

        InventoryItemInfo item = targetChest.GetInventoryItem(CurrentModelData.itemData);

        targetChest.RemoveAtChestGrid(item);
        
        chest.AddItemToChest(item);

        SetCompleteCommand();
    }

    private void CommandPickUpItem()
    {
        if(target == null) Debug.Log("нет таргета");

        target.Use(this);

        SetCompleteCommand();
    }

    [ContextMenu("пуск")]
    public void Action2() => aI.StartSolution();
    [ContextMenu("пускaaaa")]
    public void Action3() => aI.AnalyzeImportanceSolutions();
    
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
    public MapSquare(Vector2 pos, Vector2 size)
    {
        this.pos = pos;
        this.size = size;
    }
}

[Serializable]
public class SolutionInfo
{
    //важность решения
    public float importance = 0;
    
    public DSDialogueContainerSO solution;
}