using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseDor : MonoBehaviour, ICanUse
{
    [SerializeField] private Vector3 toOpen; 
    [SerializeField] private Vector3 toClose; 

    [SerializeField] private bool isClose = true;
    [SerializeField] private int speed = 40;
    private Outline outline;
    private Coroutine coroutine;

    
    private void Start() 
    {
        outline = this.GetComponent<Outline>();
    }
    public void ShowOutline(bool value)
    {
         outline.enabled = value;
    }

    public void Use(AbstractBehavior applicant = null)
    {
        if(coroutine != null) StopCoroutine(coroutine);
        
        SwithDoor(isClose = !isClose);
    }

    private void SwithDoor(bool value)
    {
        coroutine = StartCoroutine(OpenCloseDoor(value));
    }

    private IEnumerator OpenCloseDoor(bool openOreClose)
    {        
        Vector3 result =  openOreClose == false ? toOpen : toClose;

        while (transform.localRotation != Quaternion.Euler(result))
        {
            yield return new WaitForSeconds(0.1f *Time.deltaTime);

            Quaternion a = Quaternion.Euler(result);
            
            transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, a, speed * Time.deltaTime);
        }

        yield break;
    }
}
