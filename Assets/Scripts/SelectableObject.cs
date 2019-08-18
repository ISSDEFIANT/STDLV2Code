using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public bool frameSelection;

    public bool healthSystem;

    [HideInInspector] public HealthSystem _hs;

    // Start is called before the first frame update

    public virtual void Awake()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }
}