using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    [SerializeField] private Collider myCollider;

    private void OnCollisionEnter(Collision other) 
    {
        
        if(other.gameObject.layer == 7 || other.gameObject.layer == 3) return;

            Debug.Log(other.transform.name);
            Debug.Log(other.gameObject.layer);
            Physics.IgnoreCollision(other.collider, myCollider);

    }
}
