using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float timeDestroySelf = 3;
    void Start()
    {
        Destroy(this.gameObject, timeDestroySelf);
    }    
}
