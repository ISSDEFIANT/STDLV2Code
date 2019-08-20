using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponModule : Module
{
    /// <summary> Радиус орудий. </summary>
    public float WeaponRange;
    /// <summary> Наведение орудий. </summary>
    public STMethods.AttackType Aiming;
    /// <summary> Все цели, доступные для стрельбы. </summary>
    public List<SelectableObject> Targets;
    /// <summary> Главная цель для стрельбы. </summary>
    public SelectableObject MainTarget;

    /// <summary> Лучевые орудия. </summary>
    public List<BeamWeapon> BeamWeapons;
    /// <summary> Торпедные и дисраптерные установки. </summary>
    public List<TorpedoLauncher> Launchers;
    
    /// <summary> Механика системы в рабочем состоянии. </summary>
    public override void Active()
    {
        if (MainTarget != null)
        {
            if (!MainTarget.healthSystem || Vector3.Distance(transform.position, MainTarget.transform.position) > WeaponRange)
            {
                MainTarget = null;
            }
        }

        if (Targets.Count > 0)
        {
            foreach (SelectableObject _targets in Targets)
            {
                if (!_targets.healthSystem || Vector3.Distance(transform.position, _targets.transform.position) > WeaponRange)
                {
                    Targets.Remove(_targets);
                }
            }
        }

        if (BeamWeapons.Count > 0)
        {
            foreach (BeamWeapon _pw in BeamWeapons)
            {
                List<BeamWeapon> test = new List<BeamWeapon>(BeamWeapons.ToList());
                test.Remove(_pw);
                _pw.Active(test);
            }
        }
        if (Launchers.Count > 0)
        {
            foreach (TorpedoLauncher _l in Launchers)
            {
                _l.Active();
            }
        }
    }
    /// <summary> Инициализация листа целей. </summary>
    void Awake()
    {
        Targets = new List<SelectableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
