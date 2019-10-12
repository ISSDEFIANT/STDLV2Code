using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    /// <summary> Номер игрок. </summary>
    public int PlayerNum;
    
    /// <summary> Можно ли выделить рамкой. </summary>
    public bool frameSelection;
    /// <summary> Имеет ли систему жизней. </summary>
    public bool healthSystem;
    
    /// <summary> Тревога. </summary>
    public STMethods.Alerts Alerts;    
    /// <summary> Система жизней. </summary>
    [HideInInspector] public HealthSystem _hs;
    
    /// <summary> Радиус объекта. </summary>
    public float ObjectRadius;
    
    /// <summary> Зона сенсоров. </summary>
    public float SensorRange;
    
    /// <summary> Радиус орудий. </summary>
    public float WeaponRange;
    /// <summary> До скольки целей может атаковать одновременно. </summary>
    public int MaxAttackTargetCount;
    
    /// <summary> Список кораблей, которые защищают объект. </summary>
    public List<Mobile> ProtectionFleet;

    /// <summary> Уничтожен. </summary>
    [HideInInspector] public bool destroyed;
    
    // Start is called before the first frame update

    public virtual void Awake()
    {
        ProtectionFleet = new List<Mobile>();
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    private void OnDestroy()
    {
        GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>().UpdateList();
    }
}