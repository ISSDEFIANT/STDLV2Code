using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    /// <summary> Можно ли выделить рамкой. </summary>
    public bool frameSelection;
    /// <summary> Имеет ли систему жизней. </summary>
    public bool healthSystem;
    /// <summary> Система жизней. </summary>
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