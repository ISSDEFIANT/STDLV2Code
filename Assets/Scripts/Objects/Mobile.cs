using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mobile : SelectableObject
{    
    /// <summary> Модуль двигателей. </summary>
    public Engines moveComponent;
    /// <summary> Временный флот. </summary>
    public List<Mobile> TimelyFleet;

    /// <summary> Вероятность принятия паттернов атаки. </summary>
    public AttackPatternProbability AttackProbability = new AttackPatternProbability();
    
    /// <summary> Допустимая ошибка (для двигателей). </summary>
    public float Threshold;
    
    /// <summary> Построен в доке. </summary>
    public bool ConstructedOnDock;
    
    /// <summary> Команда выхода из дока. </summary>
    public UndockingCommand _uc;
    
    /// <summary> Инициализация двигателей. </summary>
    public override void Awake()
    {
        base.Awake();
        
        moveComponent = gameObject.AddComponent<Engines>();

        moveComponent.Owner = this;
        
        TimelyFleet = new List<Mobile>();

        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.Ship;
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

    public void UndockingAfterConstruction()
    {
        captain.ToExitPoint = true;
                                
        captain.dockingStation = _uc.DocingStation;
        captain.dockingHub = _uc.Hub;
            
        captain.curCommandInfo = _uc;
        captain.Command = Captain.PlayerCommand.Undocking;
    }
}