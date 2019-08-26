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
    
    /// <summary> Тревога. </summary>
    public STMethods.Alerts Alerts;    
    /// <summary> Система жизней. </summary>
    [HideInInspector] public HealthSystem _hs;
    
    /// <summary> Список кораблей, которые защищают объект. </summary>
    public List<Mobile> ProtectionFleet;

    // Start is called before the first frame update

    public virtual void Awake()
    {
        ProtectionFleet = new List<Mobile>();
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }
}