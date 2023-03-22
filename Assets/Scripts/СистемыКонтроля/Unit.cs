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
using Unity.VisualScripting;


public class Unit : AbstractBehavior
{
    public List<SolutionInfo> solutions;
    public Terrain currentTerrain;//!ждет доработки
    public List<MapSquare> map = new List<MapSquare>();
    private DSAction currentAction = DSAction.CommandStandStill;
    //двигается ли персонаж сейчас
    private bool needMoving = false;
    private bool needCreateMap = true;
    private Vector3 PintToWalke;
    private delegate void Operation();
    [SerializeField] private Brain brain = new Brain();
    [SerializeField]private ModelDate _modelData;


    //участок земли 

    public override void Init()
    {
        _agent.updateRotation = false;
        
        Chest.InitGrid(Initializer.singleton.InitObject(InitializerNames.Инвентарь_Моб).GetComponent<ItemGrid>());

        brain.Init(this, _animator);
    }    

    private void Update() => brain.Analyzer();

    public override void Use(AbstractBehavior applicant) => CustomEvent.Trigger(this.gameObject, "StartDialogue");

    //метод принять решение, передает в мозг решение
    public void SetSolution(DSDialogueContainerSO solution)
    {
        if(solution.UngroupedDialogues[0].DialogueType == DSDialogueType.Action)
        {
            brain.StartAction(solution);
        }
        else
        {
            CommandStartDialogue(solution);
        }
    }

    //методы для работы слуха
    private void OnTriggerEnter(Collider other) 
    {
        if(!brain.GetHearing().hearObjectsList.Contains(other.gameObject)) 
        {
            brain.GetHearing().hearObjectsList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(brain.GetHearing().hearObjectsList.Contains(other.gameObject)) 
        {
            brain.GetHearing().hearObjectsList.Remove(other.gameObject);
        }
    }

    //установить действие
    //! надо перенести в мозг
    public void SetAction(DSAction action, ModelDate modelDate)
    {
        Debug.Log("устанавливаю задачу " + action);
        
        if(_modelData.go == null)
        {
            _modelData = modelDate;
        }
        else
        {
            modelDate.go = _modelData.go;

            _modelData = modelDate;
        }
        
        currentAction = action;
    }

    //выполнить текущую задачу работает постоянно постоянно
    public void ExecuteCurrentCommand()
    {
        if(_hpView.gameObject.activeSelf)RotateHpBar();

        Operation operation = (Operation)Delegate.CreateDelegate(typeof(Operation), this, currentAction.ToString());

        operation.Invoke();
    }


    //поварачивает канвас юнита лицом к игроку
    private void RotateHpBar()
    {
        if(_hpView.transform.rotation != Camera.main.transform.rotation)
        {
            _hpView.transform.rotation = Camera.main.transform.rotation;
        }
    }

    #region [rgba(30,106,143, 0.05)] Разные вспомогательные методы -----------------------------------------------------------------------------------------------------//

    //двигаться к точке, пересчитывая путь
    private void MoveToPoint(Vector3 point)
    {
        _agent.SetDestination(point);

        SetAnimationRun(_agent.velocity.magnitude);

        if(_agent.path.corners.Length > 1)
        {
            FaceToPoint(_agent.path.corners[1]);
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

    

    public void SetCompleteCommand(int dialogueIndex = 0)
    {
        needCreateMap = true;

        this.currentAction = DSAction.CommandStandStill;

        _target = null;

        Debug.Log("задача выполнена! Перехожу к следующей задаче.");
        
        brain.NextStage(dialogueIndex);
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

    //метод находит таргеты
    public bool FindTarget(LayerMask layerMask)
    {
        Eyes eyes = brain.GetEyes();

        Mamry mamry = brain.GetMamry();

        eyes.SetTargetMaskForEyes(layerMask);         

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
            if(mamry.mamryTargets[0] == null || (layerMask.value & (1 << mamry.mamryTargets[0].gameObject.layer)) == 0)
            {
                mamry.mamryTargets.RemoveAt(0);

                return false;
            }
            
            _target = mamry.mamryTargets[0].GetComponent<ICanUse>();

            Debug.Log("найден таргет " + _target);
            
            return true;
        }

        return false;
    }

    public void SetAnimationWalk(bool value) => _animator.SetBool("Walk", value);
    public void SetAnimationRun(bool value) => _animator.SetBool("Run", value);
    public void SetAnimationRun(float value) => _animator.SetFloat("vertical", value);
    public void SetAnimationGetToWork(bool value) => _animator.SetBool("Work", value);
    
        
    #endregion Разные вспомогательные методы КОНЕЦ ---------------------------------------------------------------------------------------------------------------------//

    #region [rgba(30,106,143, 0.05)] Комманды для управления NPC -------------------------------------------------------------------------------------------------------//
    
    private void CommandFindTheTarget()
    {
        if(needCreateMap)
        {
            CreateMap();

            needCreateMap = false;
        }

        if(!_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance && !needMoving)
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
        
        if(FindTarget(_modelData.targetMask)) SetCompleteCommand();
    }
    
    private void CommandHoldPositionFindTheTarget()
    {
        if(FindTarget(_modelData.targetMask)) SetCompleteCommand();
    }

    //стоять на месте
    private void CommandStandStill()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);
    }

    //метод команда атакавать таргет (привязана к системе диалогов)
    public void CommandAttackTheTarget()
    {
        if(_target == null)
        {
            Debug.Log("У " + transform.name + " не установлен таргет! Или таргет не возможно атакавать");
            
            SetCompleteCommand(1);

            return;
        }
        
        AbstractBehavior buferTarget = _target as AbstractBehavior;

        if(buferTarget.IsDead)
        {
            SetCompleteCommand();
            
            return;
        }

        if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))MoveToPoint(_target.transform.position);

        if(Vector3.Distance(transform.position, _target.transform.position) <= _agent.stoppingDistance ) 
        {

            FaceToPoint(_target.transform.position);
            
            Attack();
        }
    }

    private float cooldown = 2f;
    private float nextHit = 0f;
    private void Attack()
    {
        if(Time.time >= nextHit)
        {
            if(Vector3.Distance(transform.position, _target.transform.position) < _agent.stoppingDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, _target.transform.position.z - 1.5f), 1 * Time.deltaTime);
            }

            nextHit = Time.time + cooldown;

            _animator.SetTrigger("Attack");
        }
    }

    private void CommandSleep()
    {
        unitStats.sleep = 1000;
        SetCompleteCommand();
    }

    private void CommandTrading()
    {
        Chest.StartTrading(this);

        SetCompleteCommand();
    }

    private void CommandStartDialogue()
    {
        brain.StartDialogue(_modelData.dialogue);

        SetCompleteCommand();
    }
    public void CommandStartDialogue(DSDialogueContainerSO dialog)
    {
        brain.StartDialogue(dialog);

        SetCompleteCommand();
    }

    private void CommandPlayerGiveMoney()
    {
        Chest.ReceiveMoney(GameManager.singleton.pLayerController.Chest, (int)_modelData.number);
        
        SetCompleteCommand();
    }

    private void CommandMoveToTarget()
    {
        MoveToPoint(_target.transform.position);

        // CheckDistanceAndSwitchStage(_target.transform.position);
        if(_agent.isStopped)SetCompleteCommand();
    }
    
    [ContextMenu("CommandMoveToCoordinates")]
    private void CommandMoveToCoordinates()
    {
        MoveToPoint(_modelData.pos);
        
        CheckDistanceAndSwitchStage(_modelData.pos);
    }

    //метод запускает рабочий/производственный цикл
    private void CommandGetToWork()
    {
        IWorkplace newTarget = brain.GetMamry().workplace;

        FaceToPoint(newTarget.transform.position);

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

    private void CommandMoveToWork()
    {
        _agent.stoppingDistance = 0.3f;

        MoveToPoint(brain.GetMamry().workplace.workPoint.position);

        CheckDistanceAndSwitchStage(brain.GetMamry().workplace.workPoint.position);
    }

    private void CheckDistanceAndSwitchStage(Vector3 point)
    {
        if(Vector3.Distance(transform.position, point) <= _agent.stoppingDistance && _agent.velocity.magnitude == 0)
        {
            _agent.stoppingDistance = 1.5f;

            SetCompleteCommand();
        }
    }

    private void CommandCheckTargetInventoryForItem()
    {
        if(_target == null) Debug.Log("нет таргета");

        bool res = _target.transform.GetComponent<Chest>().CheckInventoryForItems(_modelData.itemData);

        SetCompleteCommand(res ? 0 : 1);
    }
    
    private void CommandCheckSelfInventoryForItem()
    {
        bool res = Chest.CheckInventoryForItems(_modelData.itemData);

        SetCompleteCommand(res ? 0 : 1);
    }

    private void CommandCheckSelfInventoryForItemType()
    {
        SetCompleteCommand(Chest.CheckInventoryForItemsType(_modelData.itemType));
    }

    private void CommandUseSelfInventoryItem()
    {
        InventoryItemInfo item = Chest.GetInventoryForItemType(_modelData.itemType);
        
        item.Use(this);

        Chest.RemoveAtChestGrid(item);

        SetCompleteCommand();
    }

    private void CommandTakeItemFromTarget()
    {
        if(_target == null) Debug.Log("нет таргета");

        Chest targetChest = _target.transform.GetComponent<Chest>();

        InventoryItemInfo item = targetChest.GetInventoryItem(_modelData.itemData);

        targetChest.RemoveAtChestGrid(item);
        
        Chest.AddItemToChest(item);

        SetCompleteCommand();
    }

    private void CommandAddItemToTargetInventory()
    {
        if(_target == null) Debug.Log("нет таргета");

        Chest targetChest = _target.transform.GetComponent<Chest>();

        targetChest.AddItemToChest(_modelData.itemData);

        SetCompleteCommand();
    }

    private void CommandPickUpItem()
    {
        if(_target == null) Debug.Log("нет таргета");

        _target.Use(this);

        SetCompleteCommand();
    }

    private void CommandTaskToGroup()
    {
        foreach (var person in brain.GetMamry().groupMembers)
        {
            person._target = _target;

            person.solutions.Add(new SolutionInfo(101, _modelData.dialogue));
        }

        SetCompleteCommand();
    }

    private void CommandTakeDecision()
    {
        _target = null;

        brain.GetMamry().mamryTargets.Clear();

        solutions.Add(new SolutionInfo(101, _modelData.dialogue));

        SetCompleteCommand();
    }

    private string cunAnimPlay = "";
    bool isPlayin = false; 
    private void CommandPlayAnimation()
    {
        
        if(_modelData.currentAnimation == CurrentAnimation.Украсть)
        {
            if(!_animator.GetCurrentAnimatorStateInfo(0).IsName(cunAnimPlay) && isPlayin == false)
            {
                cunAnimPlay = "Steal";

                _animator.Play("Steal");
            }
        }

        if(_animator.GetCurrentAnimatorStateInfo(0).IsName(cunAnimPlay))isPlayin = true;
            
        if(!_animator.GetCurrentAnimatorStateInfo(0).IsName(cunAnimPlay) && isPlayin == true)
        {
            cunAnimPlay = "";

            isPlayin = false;

            SetCompleteCommand();
        }
    }

    public AnimationClip FindAnimation (string name) 
    {
        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
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
        if(_modelData.operation == ObjectOperation.Выключить)_modelData.go.SetActive(false);
        else
        if(_modelData.operation == ObjectOperation.Включить)_modelData.go.SetActive(true);
        else
        if(_modelData.operation == ObjectOperation.Уничножить) Destroy(_modelData.go.gameObject);
        else
        if(_modelData.operation == ObjectOperation.Использовать) _modelData.go.GetComponent<ICanUse>().Use();

        SetCompleteCommand();
    }

    public void CommandPerformOperationWithAttribute()
    {
        UnitAtribut info = _modelData.unitAtribut;

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
                UnitOperation.Прибавить => atribut + (int)_modelData.number,
            
                UnitOperation.Вычисть => atribut - (int)_modelData.number,
            
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
    public GameObject go;
    public Vector3 pos;
    public ItemData itemData;
    public DSDialogueContainerSO dialogue;
    public LayerMask targetMask;
    public ItemData.ItemType itemType;
    public UnitAtribut unitAtribut;
    public ObjectOperation operation;
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