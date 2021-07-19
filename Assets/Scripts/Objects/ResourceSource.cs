using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : SelectableObject
{
    /// <summary> Максимальное количество дилития. </summary>    
    public float maxDilithium;
    /// <summary> Текущее количество дилития. </summary>    
    public float curDilithium;
    /// <summary> Максимальное количество титана. </summary>    
    public float maxTitanium;
    /// <summary> Текущее количество титана. </summary>    
    public float curTitanium;
    /// <summary> Бесконечны ли. </summary>
    public bool isInfinite;

    public RingSpawnZone spawner;

    public override void Awake()
    {
        base.Awake();
        stationSelectionType = true;

        healthSystem = true;

        _hs = gameObject.AddComponent<HealthSystem>();

        _hs.Owner = this;

        ControllingFraction = STMethods.Races.None;
        
        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.Asteroid;
    }
}