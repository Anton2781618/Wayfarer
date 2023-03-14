using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UseDor : MonoBehaviour, ICanUse
{
    private bool isClose = false;
    private Coroutine coroutine;
    private float startTime;
    [SerializeField] private Vector3 toOpen; 
    [SerializeField] private Vector3 toClose; 
    [SerializeField] private int speed = 40;
    [SerializeField] private UnityEvent unityEvent;
    [SerializeField] private bool isLock = false;
    [SerializeField] Outline outline;

    
    private void Start() 
    {
        startTime = Time.time;
    }

    public void LockDore(bool value)
    {
        isLock = value;
    }
    
    public void ShowOutline(bool value)
    {
        outline.enabled = value;
    }

    public void Use(AbstractBehavior applicant = null)
    {
        unityEvent?.Invoke();
    }

    public void SwithDoor(bool value)
    {
        if(coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(OpenCloseDoor(value));
    }

    public void SwithDoor()
    {
        SwithDoor(isClose = !isClose);
    }

    //удаляет компонент через промежуток веремени
    public void DestroySelfImmediate(float durationTime)
    {
        Invoke("DestroySelfImmediate", durationTime);
    }

    //удаляет компонент сразу
    public void DestroySelfImmediate()
    {
        DestroyImmediate(this);
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
