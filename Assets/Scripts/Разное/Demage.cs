using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demage : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer == 6) return;
        Debug.Log(other.transform);

        other.transform.GetComponent<Unit>().TakeDamage(1000);
    }
}
