using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigitbodyVelocityAdder : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 angularVelocity;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().velocity = velocity;
        gameObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}