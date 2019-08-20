using System.Collections;
using System.Collections.Generic;
using Modules;
using UnityEngine;

public class Static : SelectableObject
{
    /// <summary> Модули, присутствующие на объекте. </summary>
    public Module[] Modules;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /// <summary> Включение модулей в работу. </summary>
    public override void Update()
    {
        base.Update();
        foreach (var _module in Modules)
        {
            _module.Active();
        }
    }
}
