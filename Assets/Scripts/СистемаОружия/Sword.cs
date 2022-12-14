using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ICanTakeDamage;



public class Sword : MonoBehaviour
{
    [SerializeField] private AbstractBehavior holder; 
    [SerializeField] private int damage = 50; 
    [SerializeField] private LayerMask triggerMask;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject wood;

    private bool hit = false;
    private void OnTriggerEnter(Collider other)
    {
        if((triggerMask.value & (1 << other.gameObject.layer)) == 0 || !hit)return;

        if(other.transform.gameObject != transform.gameObject)
        {

            if(other.gameObject.layer == 9 || other.gameObject.layer == 13)
            {
                Instantiate(wood, other.ClosestPoint(transform.position), Quaternion.identity);
            } 
            else
            {
                AbstractBehavior bufer = other.transform.GetComponent<AbstractBehavior>();

                if(bufer.GetStateNPC() != States.Мертв)
                {   
                    Instantiate(blood, other.ClosestPoint(transform.position), Quaternion.identity);

                    bufer.TakeDamage(holder, damage);
                }
            }
            SetHitBoolOFF();
        }
    }

    public void SetHitBoolOFF()
    {
        hit = false;
    }

    public void SetHitBoolOn()
    {
        hit = true;
    }

    public int Jopa()
    {
        return 0;
    }
}





