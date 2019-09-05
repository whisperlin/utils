using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnCollisionEnter " + gameObject.name + " to " + other.gameObject.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter " + gameObject.name);
    }
}
