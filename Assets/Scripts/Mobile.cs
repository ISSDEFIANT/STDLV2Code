using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mobile : SelectableObject
{    
    public Rigidbody rigitBody;
    
    public EngineModule moveComponent;
    
    public Module[] Modules;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        rigitBody = gameObject.AddComponent<Rigidbody>();
        rigitBody.useGravity = false;
        
        moveComponent = gameObject.AddComponent<EngineModule>();
    }

    // Update is called once per frame
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