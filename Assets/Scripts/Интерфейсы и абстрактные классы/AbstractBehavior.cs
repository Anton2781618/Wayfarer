using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using static ICanTakeDamage;
using DS;

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
    protected ICanUse target;
    [SerializeField] protected Slider HpSlider;   
    [SerializeField] protected Slider MpSlider;   
    [SerializeField] protected States state = States.Патруль;
    
    public Animator anim;
    
    protected NavMeshAgent agent;
    [SerializeField] private Sword sword;
    public Chest chest{get; private set;}

    private void Start() 
    {
        target = FindObjectOfType<PLayerController>();

        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();
        
        chest = GetComponent<Chest>();
        
        Init();

        unitStats.curHP = unitStats.maxHP;

        HpSlider.maxValue = unitStats.maxHP;

        HpSlider.value = unitStats.maxHP;

    }

    public virtual void Init()
    {
    }

    public virtual void TakeDamage(AbstractBehavior enemy, int value)
    {
        target = enemy;

        unitStats.curHP -= value;

        HpSlider.value -= value;
        
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
    public void UseMana(int value)
    {
        if(value > unitStats.curMana)
        {
            Debug.Log("не хватает маны");
            return;
        } 

        unitStats.curMana -= value;

        MpSlider.value -= value;

        if(unitStats.curMana < 0) unitStats.curMana = 0;
    }

    private IEnumerator DieCurutina()
    {
        yield return new WaitForEndOfFrame();

        Die();
    }

    public virtual void Die()
    {
        if(agent)agent.enabled = false;

        anim.SetBool("die", true);
        
        state = States.Мертв;

        this.enabled = false;

        Debug.Log(transform.name + " умер");
    }

    public UnitStats GetStats() => unitStats;
    public States GetCurrentUnitState() => state;

    public void SetHitBoolOFF()
    {
        if(sword == null)
        {
            Debug.Log("У " + transform.name + " нет оружия");
            return;
        }
        sword.SetHitBoolOFF();
    }

    public void SetHitBoolOn()
    {
        if(sword == null)
        {
            Debug.Log("У " + transform.name + " нет оружия");
            return;
        }
        sword.SetHitBoolOn();
    }

    public ICanUse GetTarget() => target;

    public void Revive()
    {
        if(state != States.Мертв)
        {
            Debug.Log(transform.name + " живой");
            return;
        }

        state = States.Патруль;
        StartCoroutine(WaihtRevive());
        
        unitStats.curHP = unitStats.maxHP;
        HpSlider.value = unitStats.maxHP;

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

            if(agent)agent.enabled = true;  

            end = true; 
        }
    }

    public void Healing(int value)
    {
        if((value + unitStats.curHP) > unitStats.maxHP)
        {
            unitStats.curHP = unitStats.maxHP;

            HpSlider.value = unitStats.curHP;

            return;
        }

        unitStats.curHP += value;

        HpSlider.value = unitStats.curHP;
    }

    public void RestoreMana(int value)
    {
        if((value + unitStats.curMana) > unitStats.maxMana)
        {
            unitStats.curMana = unitStats.maxMana;
            MpSlider.value = unitStats.curMana;
            return;
        }
        unitStats.curMana += value;
        MpSlider.value = unitStats.curMana;
    }

    public int GetCurHP() => unitStats.curHP;

    public States GetStateNPC() => state;

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

    public virtual void ShowTest()
    {
        Debug.Log("базовый");
    }
}
