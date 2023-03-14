using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using static ICanTakeDamage;

[Serializable]
public class UnitStats
{
    public int level = 1;
    public int maxExperience = 500;
    public int curExperience = 0;
    public int pointExperience = 0;
    public int maxHP = 100;
    public int curHP = 100;
    public int maxMana = 5;
    public int curMana = 5;

    public int experienceForKilling = 50; 

    public int hunger = 100;
    public int sleep = 100;
}

public abstract class AbstractBehavior : MonoBehaviour, ICanTakeDamage, ICanUse
{
    public Animator anim;
    public UnitStats unitStats;
    protected ICanUse _target;
    protected NavMeshAgent _agent;
    public Chest chest{get; private set;}
    [SerializeField] protected HPWindowUI _hpView;   
    [SerializeField] protected States _state = States.Патруль;
    [SerializeField] private Sword _sword;

    private void Start() 
    {
        _agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();
        
        chest = GetComponent<Chest>();
        
        Init();

        _hpView.InitHp(unitStats.curHP, unitStats.maxHP);
    }

    public virtual void Init()
    {
    }

    public virtual void TakeDamage(AbstractBehavior enemy, int value)
    {
        _target = enemy;

        unitStats.curHP -= value;

        _hpView.SetHpValue(value);
        
        if(unitStats.curHP <= 0)
        {
            StartCoroutine(DieCurutina());
        }
        else
        {
            anim.SetBool("Takehit", true);
        }
    }

    public void ManaMinus()
    {
        UseMana(5);
    }

    //использовать ману
    public void UseMana(int value)
    {
        if(value > unitStats.curMana)
        {
            Debug.Log("не хватает маны");
            return;
        } 

        unitStats.curMana -= value;

        _hpView.SetManaValue(value);

        if(unitStats.curMana < 0) unitStats.curMana = 0;
    }

    private IEnumerator DieCurutina()
    {
        yield return new WaitForEndOfFrame();

        Die();
    }
    
    //умереть
    public virtual void Die()
    {
        // if(agent)agent.enabled = false;

        anim.SetBool("die", true);
        
        _state = States.Мертв;

        this.enabled = false;

        Debug.Log(transform.name + " умер");
    }

    public UnitStats GetStats() => unitStats;
    public States GetCurrentUnitState() => _state;

    public void SetHitBoolOFF()
    {
        if(_sword == null)
        {
            Debug.Log("У " + transform.name + " нет оружия");
            return;
        }
        _sword.SetHitBoolOFF();
    }

    public void SetHitBoolOn()
    {
        if(_sword == null)
        {
            Debug.Log("У " + transform.name + " нет оружия");
            return;
        }

        _sword.SetHitBoolOn();
    }

    public ICanUse GetTarget() => _target;
    // public void SetTarget(ICanUse newTarget) => _target = newTarget;
    public void SetTarget(ICanUse newTarget)
    {

        _target = newTarget;

        Debug.Log(_target + " " + newTarget);
    } 

    public void Revive()
    {
        if(_state != States.Мертв)
        {
            Debug.Log(transform.name + " живой");
            return;
        }

        _state = States.Патруль;

        StartCoroutine(WaihtRevive());
        
        unitStats.curHP = unitStats.maxHP;

        _hpView.SetHpValue(unitStats.maxHP);

        anim.SetTrigger("Revive");

        anim.SetBool("die", false);
        
        transform.GetComponent<Collider>().enabled = true;

        this.enabled = true;
    }

    private IEnumerator WaihtRevive()
    {
        bool end = false;
        
        while (!end)
        {
            yield return new WaitForEndOfFrame();

            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up") || anim.GetCurrentAnimatorStateInfo(0).IsName("Male Die"))
            {
                continue;  
            }   

            if(_agent)_agent.enabled = true;  

            end = true; 
        }
    }

    public void Healing(int value)
    {
        if((value + unitStats.curHP) > unitStats.maxHP)
        {
            unitStats.curHP = unitStats.maxHP;

            _hpView.SetHpValue(unitStats.curHP);

            return;
        }

        unitStats.curHP += value;

        _hpView.SetHpValue(unitStats.curHP);
    }

    public void RestoreMana(int value)
    {
        if((value + unitStats.curMana) > unitStats.maxMana)
        {
            unitStats.curMana = unitStats.maxMana;
            
            _hpView.SetManaValue(unitStats.curMana);
            return;
        }
        
        unitStats.curMana += value;
        
        _hpView.SetManaValue(unitStats.curMana);
    }

    public int GetCurHP() => unitStats.curHP;

    public States GetStateNPC() => _state;

    public Animator GetAnim() => anim;

    public virtual void SowHealthBar(bool value)
    {
    }

    //метод переопределяется в классе Unit
    public virtual void Use(AbstractBehavior applicant)
    {
    }

    public virtual void ShowOutline(bool value)
    {
    }
}
