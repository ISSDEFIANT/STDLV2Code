using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static : SelectableObject
{
    public Module[] Modules;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var _module in Modules)
        {
            _module.Active();
        }
    }
}
