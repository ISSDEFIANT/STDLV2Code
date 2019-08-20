using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Module : MonoBehaviour
{
    /// <summary> Объект, которому модуль принадлежит. </summary>
    public SelectableObject Owner;
    
    
    /// <summary> Когда модуль активируется. </summary>
    public virtual void Active(){}
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
