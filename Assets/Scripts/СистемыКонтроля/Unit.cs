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
    public List<GameObject> objectsForOperations;
    private delegate void Operation();
    [SerializeField] protected Canvas unitCanvas;
    [SerializeField] public AI aI = new AI();
    public DSAction currentAction;
    public DSDialogueContainerSO dialogTEST;
    private ModelDate CurrentModelData;

    public TMPro.TextMeshPro textMesh; 

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
        
        agent.updateRotation = false;
        
        chest.InitGrid(Initializer.singleton.InitObject(InitializerNames.Инвентарь_Моб).GetComponent<ItemGrid>());

        aI.Init(this, anim);
    }    

    //методы для работы слуха
    private void OnTriggerEnter(Collider other) 
    {
        
        if(!aI.GetHearing().hearObjectsList.Contains(other.gameObject)) aI.GetHearing().hearObjectsList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other) 
    {
        
        if(aI.GetHearing().hearObjectsList.Contains(other.gameObject)) aI.GetHearing().hearObjectsList.Remove(other.gameObject);
    }

    public override void ShowOutline(bool value)
    {
        if(state == States.Мертв)
        {
            SowHealthBar(false);    

            return;
        }
        SowHealthBar(value);
    }

    public override void TakeDamage(AbstractBehavior enemy, int value)
    {
        base.TakeDamage(enemy, value);

        aI.SetAttackSolution();
    }
    public void TakeDamage(int value)
    {
        unitStats.curHP -= value;

        HpSlider.value -= value;
        
        Debug.Log(unitStats.curHP);
        if(unitStats.curHP <= 0)
        {
            StartCoroutine(DieCurutina());
        }
        else
        {
            anim.SetBool("Takehit", true);
        }
    }

    private IEnumerator DieCurutina()
    {
        yield return new WaitForEndOfFrame();

        Die();
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

    // public override void Use(AbstractBehavior applicant) => CommandStartDialogue(dialogTEST);
    public override void Use(AbstractBehavior applicant)
    {
        gameObject.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().SetVariableValue("DialogState", true);
    }

    #region [rgba(30,106,143, 0.05)] Разные вспомогательные методы -----------------------------------------------------------------------------------------------------//

    //двигаться к точке, пересчитывая путь
    private void MoveToPoint(Vector3 point)
    {
        agent.SetDestination(point);

        SetAnimationRun(Vector3.Distance(transform.position, point) > agent.stoppingDistance);

        if(agent.path.corners.Length > 1)
        {
            FaceToPoint(agent.path.corners[1]);
        }
    }

    private NavMeshPath CreatePath(Vector3 point)
    {
        NavMeshPath path = new NavMeshPath();

        NavMeshHit outPoint;

        NavMeshHit outTrasformPos;

        if (NavMesh.SamplePosition(point, out outPoint, 10f, NavMesh.AllAreas)) 
        {
            Debug.Log("1 прошел");
            
            if (NavMesh.SamplePosition(transform.localPosition, out outTrasformPos, 10f, NavMesh.AllAreas)) 
            {
                Debug.Log("2 прошел");
                if(NavMesh.CalculatePath(outTrasformPos.position, outPoint.position, UnityEngine.AI.NavMesh.AllAreas, path))
                // if(NavMesh.CalculatePath(transform.position, outPoint.position, UnityEngine.AI.NavMesh.AllAreas, path))
                {
                    Debug.Log("3 прошел");
                    return path;
                }
            }
        }
        
        return null;
    }

    //создать случайную точку в выбранном квадрате
    public Vector3 CreatePoints(Vector3 point)
    {
        NavMeshPath path = new NavMeshPath();

        NavMeshHit hit;

        Vector3 result = transform.position;

        if (NavMesh.SamplePosition(point, out hit, 5f, NavMesh.AllAreas)) 
        {
            result = hit.position;
        }

        NavMesh.CalculatePath(transform.position, result, UnityEngine.AI.NavMesh.AllAreas, path);

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
        
        aI.NextStage(dialogueIndex);
    }

    public override void Die()
    {
        base.Die();

        // transform.Find("граунд").GetComponent<ExampleClass>().gameObject.SetActive(false);
        
        SowHealthBar(false);
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

        Mamry mamry = aI.GetMamry();
        

        eyes.SetTargetMaskForEyes(CurrentModelData.targetMask);         

        if(eyes.visileTargets.Count > 0)
        {
            foreach (var item in eyes.visileTargets)
            {
                if(!eyes.SetTargetToMamry(item))
                {
                    mamry.mamryTargets.Add(item);
                }
            }
        }

        if(mamry.mamryTargets.Count > 0)
        {
            if(mamry.mamryTargets[0] == null || (CurrentModelData.targetMask.value & (1 << mamry.mamryTargets[0].gameObject.layer)) == 0)
            {
                mamry.mamryTargets.RemoveAt(0);

                return false;
            }
            
            target = mamry.mamryTargets[0].GetComponent<ICanUse>();

            Debug.Log("найден таргет " + target);
            
            return true;
        }

        return false;
    }

    public void SetTarget(ICanUse newTarget) => target = newTarget;
    public void SetAnimationWalk(bool value) => anim.SetBool("Walk", value);
    public void SetAnimationRun(bool value) => anim.SetBool("Run", value);
    public void SetAnimationGetToWork(bool value) => anim.SetBool("Work", value);
    
        
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
        // if(Vector3.Distance(transform.position, path.corners[path.corners.Length - 1]) <= 1.5f && !needMoving)
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

        Debug.Log("Ticks " + ts.Ticks);
    }

    private void CommandHoldPositionFindTheTarget()
    {
        if(FindTarget()) SetCompleteCommand();
    }

    //стоять на месте
    private void CommandStandStill()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Run", false);
    }

    //метод команда атакавать таргет (привязана к системе диалогов)
    public void CommandAttackTheTarget()
    {
        if(target == null)
        {
            Debug.Log("У " + transform.name + " не установлен таргет! Или таргет не возможно атакавать");
            
            SetCompleteCommand(1);

            return;
        }
        
        AbstractBehavior buferTarget = target as AbstractBehavior;

        if(buferTarget.GetCurrentUnitState() == States.Мертв)
        {
            state = States.Патруль;
            
            SetCompleteCommand();
            
            return;
        }

        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))MoveToPoint(target.transform.position);

        if(Vector3.Distance(transform.position, target.transform.position) <= agent.stoppingDistance ) 
        {

            FaceToPoint(target.transform.position);

            SetAnimationRun(false);            
            
            Attack();
        }
    }

    private float cooldown = 2f;
    private float nextHit = 0f;
    private void Attack()
    {
        if(Time.time >= nextHit)
        {
            
            if(Vector3.Distance(transform.position, target.transform.position) < agent.stoppingDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, target.transform.position.z - 1.5f), 1 * Time.deltaTime);
            }
            

            nextHit = Time.time + cooldown;

            anim.SetTrigger("Hit");
        }
    }

    //метод запускает рабочий/производственный цикл
    private void CommandGetToWork()
    {
        IWorkplace newTarget = aI.GetMamry().workplace;

        if(newTarget.WorkIsFinish)
        {
            SetAnimationGetToWork(true);

            newTarget.Use();
        }
        else
        {
            SetAnimationGetToWork(false);

            SetCompleteCommand();
        }
    }

    private void CommandSleep()
    {
        unitStats.sleep = 1000;
        SetCompleteCommand();
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
    public void CommandStartDialogue(DSDialogueContainerSO dialog)
    {
        aI.StartDialogue(dialog);

        SetCompleteCommand();
    }

    private void CommandPlayerGiveMoney()
    {
        chest.ReceiveMoney(GameManager.singleton.pLayerController.chest, (int)CurrentModelData.number);
        
        SetCompleteCommand();
    }

    private void CommandMoveToTarget()
    {
        MoveToPoint(target.transform.position);

        CheckDistanceAndSwitchStage(target.transform.position);
    }
    
    private void CommandMoveToCoordinates()
    {
        MoveToPoint(CurrentModelData.pos);

        CheckDistanceAndSwitchStage(CurrentModelData.pos);
    }

    private void CommandMoveToWork()
    {
        MoveToPoint(aI.GetMamry().workplace.workPoint.position);

        CheckDistanceAndSwitchStage(aI.GetMamry().workplace.workPoint.position);
    }
    
    private void CheckDistanceAndSwitchStage(Vector3 point)
    {
        if(Vector3.Distance(transform.position, point) <= agent.stoppingDistance)
        {
            SetCompleteCommand();
        }
    }

    private void CommandCheckTargetInventoryForItem()
    {
        if(target == null) Debug.Log("нет таргета");

        bool res = target.transform.GetComponent<Chest>().CheckInventoryForItems(CurrentModelData.itemData);
        SetCompleteCommand(res ? 0 : 1);
    }
    
    private void CommandCheckSelfInventoryForItem()
    {
        bool res = chest.CheckInventoryForItems(CurrentModelData.itemData);

        SetCompleteCommand(res ? 0 : 1);
    }

    private void CommandCheckSelfInventoryForItemType()
    {
        SetCompleteCommand(chest.CheckInventoryForItemsType(CurrentModelData.itemType));
    }

    private void CommandUseSelfInventoryItem()
    {
        InventoryItemInfo item = chest.GetInventoryForItemType(CurrentModelData.itemType);
        
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

    private void CommandAddItemToTargetInventory()
    {
        if(target == null) Debug.Log("нет таргета");

        Chest targetChest = target.transform.GetComponent<Chest>();

        targetChest.AddItemToChest(CurrentModelData.itemData);

        SetCompleteCommand();
    }

    private void CommandPickUpItem()
    {
        if(target == null) Debug.Log("нет таргета");

        target.Use(this);

        SetCompleteCommand();
    }

    private void CommandTaskToGroup()
    {
        foreach (var person in aI.GetMamry().groupMembers)
        {
            person.target = target;

            person.solutions.Add(new SolutionInfo(101, CurrentModelData.dialogue));
        }

        SetCompleteCommand();
    }

    private void CommandTakeDecision()
    {
        target = null;

        aI.GetMamry().mamryTargets.Clear();

        solutions.Add(new SolutionInfo(101, CurrentModelData.dialogue));

        SetCompleteCommand();
    }

    private string cunAnimPlay = "";
    bool isPlayin = false; 
    private void CommandPlayAnimation()
    {
        
        if(CurrentModelData.currentAnimation == CurrentAnimation.Украсть)
        {
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName(cunAnimPlay) && isPlayin == false)
            {
                cunAnimPlay = "Steal";

                anim.Play("Steal");
            }
        }

        if(anim.GetCurrentAnimatorStateInfo(0).IsName(cunAnimPlay))isPlayin = true;
            
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName(cunAnimPlay) && isPlayin == true)
        {
            cunAnimPlay = "";

            isPlayin = false;

            SetCompleteCommand();
        }
    }

    public AnimationClip FindAnimation (string name) 
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }

    private void CommandObjectOperation()
    {
        GameObject obj = objectsForOperations[CurrentModelData.index];

        if(CurrentModelData.objectOperation == ObjectOperation.Выключить)obj.SetActive(false);
        else
        if(CurrentModelData.objectOperation == ObjectOperation.Включить)obj.SetActive(true);
        else
        if(CurrentModelData.objectOperation == ObjectOperation.Уничножить) Destroy(obj.gameObject);
        else
        if(CurrentModelData.objectOperation == ObjectOperation.Использовать) obj.GetComponent<ICanUse>().Use();

        SetCompleteCommand();
    }

    public void CommandPerformOperationWithAttribute()
    {
        UnitAtribut info = CurrentModelData.unitAtribut;

        info = info switch
        {
            UnitAtribut.Здоровье => Calculate(ref unitStats.curHP, UnitOperation.Прибавить),

            UnitAtribut.Мана => Calculate(ref unitStats.curMana, UnitOperation.Прибавить),
            
            UnitAtribut => throw new ArgumentException("Передан недопустимый аргумент")
        };

        UnitAtribut Calculate(ref int atribut, UnitOperation operation)
        {
            atribut = operation switch
            {
                UnitOperation.Прибавить => atribut + (int)CurrentModelData.number,
            
                UnitOperation.Вычисть => atribut - (int)CurrentModelData.number,
            
                UnitOperation => throw new ArgumentException("Передан недопустимый аргумент")
            };

            return UnitAtribut.Здоровье;
        }

        SetCompleteCommand();
    }
    
    
    #endregion Комманды для управления NPC КОНЕЦ --------------------------------------------------------------------------------------------------------------------//
}

[Serializable]
public class ModelDate
{
    public float number;
    public int index;
    public string text;
    public Vector3 pos;
    public ItemData itemData;
    public DSDialogueContainerSO dialogue;
    public LayerMask targetMask;
    public ItemData.ItemType itemType;
    public Transform objectOnScen;
    public UnitAtribut unitAtribut;
    public ObjectOperation objectOperation;
    public CurrentAnimation currentAnimation;
    public UnitOperation unitOperation;
    public DSAction dSAction;
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

    public SolutionInfo(float importance, DSDialogueContainerSO solution)
    {
        this.importance = importance;
        this.solution = solution;
    }
}