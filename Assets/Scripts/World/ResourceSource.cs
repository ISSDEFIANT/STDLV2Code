using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : SelectableObject
{
    /// <summary> Текущее количество ресурсов. </summary>    
    public float curResources;
    /// <summary> Бесконечны ли. </summary>
    public bool isInfinite;
    /// <summary> Тип. </summary>
    public STMethods.ResourcesType type;

    public override void Awake()
    {
        base.Awake();
        stationSelectionType = true;

        healthSystem = true;

        _hs = gameObject.AddComponent<HealthSystem>();

        _hs.Owner = this;
    }
}
