using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Modules;
using UnityEngine;

public class Mobile : SelectableObject
{    
    /// <summary> Физический компонент. </summary>
    public Rigidbody rigitBody;
    /// <summary> Модуль двигателей. </summary>
    public EngineModule moveComponent;
    /// <summary> Временный флот. </summary>
    public List<Mobile> TimelyFleet;
    
    /// <summary> Инициализация двигателей. </summary>
    public override void Awake()
    {
        base.Awake();
        
        rigitBody = gameObject.AddComponent<Rigidbody>();
        rigitBody.useGravity = false;
        
        moveComponent = gameObject.AddComponent<EngineModule>();
        
        TimelyFleet = new List<Mobile>();
    }

    /// <summary> Включение модулей. </summary>
    public override void Update()
    {
        base.Update();
    }
}