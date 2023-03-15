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
    public UnitStats unitStats;
    public bool IsDead {get; private set;}
    protected Animator _animator;
    protected ICanUse _target;
    protected NavMeshAgent _agent;
    public Chest Chest{get; private set;}
    [SerializeField] protected HPWindowUI _hpView;   
    [SerializeField] private Sword _sword;

    private void Start() 
    {
        _agent = GetComponent<NavMeshAgent>();

        _animator = GetComponent<Animator>();
        
        Chest = GetComponent<Chest>();
        
        Init();

        _hpView.InitHp(unitStats.curHP, unitStats.maxHP);
    }

    public abstract void Init();
    
    //метод переопределяется в классе Unit
    public virtual void Use(AbstractBehavior applicant){}

    public UnitStats GetStats() => unitStats;

    public ICanUse GetTarget() => _target;

    public void SetTarget(ICanUse newTarget) => _target = newTarget;

    public int GetCurHP() => unitStats.curHP;

    public void SowHealthBar(bool value) => _hpView.gameObject.SetActive(value);

    public void SetHitBoolOFF() => _sword.SetHitBoolOFF();

    public void SetHitBoolOn() => _sword.SetHitBoolOn();

    public void ShowOutline(bool value)
    {
        if(IsDead)
        {
            SowHealthBar(false);    

            return;
        }
        
        SowHealthBar(value);
    }

    public virtual void TakeDamage(AbstractBehavior enemy, int value)
    {
        _target = enemy;

        unitStats.curHP -= value;

        _hpView.SetHpValue(unitStats.curHP);
        
        if(unitStats.curHP <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetBool("Takehit", true);
        }
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

        _hpView.SetManaValue(unitStats.curMana);

        if(unitStats.curMana < 0) unitStats.curMana = 0;
    }
    
    //умереть
    public virtual void Die()
    {
        if(_agent)_agent.enabled = false;

        _animator.SetBool("die", true);
        
        IsDead = true;

        this.enabled = false;

        Debug.Log(transform.name + " умер");
    }

    //воскресить
    public void Revive()
    {
        if(!IsDead)
        {
            Debug.Log(transform.name + " живой");

            return;
        }

        StartCoroutine(WaihtRevive());
        
        unitStats.curHP = unitStats.maxHP;

        _hpView.SetHpValue(unitStats.maxHP);

        _animator.SetTrigger("Revive");

        _animator.SetBool("die", false);
        
        transform.GetComponent<Collider>().enabled = true;

        this.enabled = true;
    }

    //ждать пока npc понимится.
    private IEnumerator WaihtRevive()
    {
        bool end = false;
        
        while (!end)
        {
            yield return new WaitForEndOfFrame();

            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Get Up") ||
             _animator.GetCurrentAnimatorStateInfo(0).IsName("Male Die"))
            {
                continue;  
            }   

            if(_agent)_agent.enabled = true;  

            end = true; 
        }
    }

    //лечить
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

    //пополнить ману
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
}
