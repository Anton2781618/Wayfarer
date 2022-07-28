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
}

public abstract class AbstractBehavior : MonoBehaviour, ICanTakeDamage, ICanUse
{
    public UnitStats unitStats;
    [SerializeField] protected ICanUse target;
    // [SerializeField] protected ItemType itemType;
     
    [SerializeField] protected Slider HpSlider;   
    [SerializeField] protected Slider MpSlider;   
    [SerializeField] protected States state = States.Патруль;
    
    protected Animator anim;
    [SerializeField] protected Canvas unitCanvas;
    protected NavMeshAgent agent;
    [SerializeField] private Sword sword;
    protected Chest chest;

    private void Start() 
    {
        target = FindObjectOfType<PLayerController>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        chest = GetComponent<Chest>();
        unitStats.curHP = unitStats.maxHP;
        HpSlider.maxValue = unitStats.maxHP;
        HpSlider.value = unitStats.maxHP;
    }

    public void TakeDamage(int value)
    {
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

    [ContextMenu("UseMana")]
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

    public void Die()
    {
        if(agent)agent.enabled = false;

        anim.SetBool("die", true);
        SowHealthBar (false);
        state = States.Мертв;
        this.enabled = false;
        Debug.Log(transform.name + " умер");
    }
    public void SowHealthBar(bool value)
    {
        if(unitCanvas == null)
        {
            Debug.Log("у " + transform.name + " не установлен unitCanvas");
            return;
        }
        unitCanvas.gameObject.SetActive(value);
    }

    public UnitStats GetStats()
    {
        return unitStats;
    }
    public States GetCurrentUnitState()
    {
        return state;
    }
    
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

    public ICanUse GetTarget()
    {
        return target;
    }

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

    public int GetCurHP()
    {
        return unitStats.curHP;
    }

    public States getStateNPC()
    {
        return state;
    }

    public Animator GetAnim()
    {
        return anim;
    }

    //метод переопределяется в классе Unit
    public virtual void Use()
    {
    }

    public void ShowOutline(bool value)
    {
        if(state == States.Мертв)
        {
            SowHealthBar(false);    
            return;
        }
        SowHealthBar(value);
    }
}
