using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Modules;
using UnityEngine;

public class Mobile : SelectableObject
{    
    /// <summary> Модуль двигателей. </summary>
    public Engines moveComponent;
    /// <summary> Временный флот. </summary>
    public List<Mobile> TimelyFleet;

    /// <summary> Вероятность принятия паттернов атаки. </summary>
    public AttackPatternProbability AttackProbability;
    
    /// <summary> Допустимая ошибка (для двигателей). </summary>
    public float Threshold;
    
    /// <summary> Инициализация двигателей. </summary>
    public override void Awake()
    {
        base.Awake();
        
        moveComponent = gameObject.AddComponent<Engines>();

        moveComponent.Owner = this;
        
        TimelyFleet = new List<Mobile>();
    }

    /// <summary> Включение модулей. </summary>
    public override void Update()
    {
        base.Update();
    }

    public void ResetTimelyFleet(List<Mobile> newList)
    {
        if (TimelyFleet != null && TimelyFleet.Count > 0)
        {
            foreach (Mobile all in TimelyFleet)
            {
                if (all != this)
                {
                    if (all.TimelyFleet.Any(x => x == this))
                        all.TimelyFleet.Remove(this);
                }
            }
        }

        if (newList == null)
        {
            TimelyFleet.Clear();   
        }
        else
        {
            TimelyFleet = newList.ToList();
        }
    }
}