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
    /// <summary> Другие модули. </summary>
    public Module[] Modules;
    /// <summary> Временный флот. </summary>
    public List<Mobile> TimelyFleet;
    
    /// <summary> Инициализация двигателей. </summary>
    public override void Awake()
    {
        base.Awake();
        
        rigitBody = gameObject.AddComponent<Rigidbody>();
        rigitBody.useGravity = false;
        
        moveComponent = gameObject.AddComponent<EngineModule>();
    }

    /// <summary> Включение модулей. </summary>
    public override void Update()
    {
        base.Update();
        
        if (Modules.Length != 0)
        {
            foreach (var _module in Modules)
            {
                _module.Active();
            }
        }
    }
}