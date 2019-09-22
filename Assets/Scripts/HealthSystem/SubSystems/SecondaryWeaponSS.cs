using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Controllers;

public class SecondaryWeaponSS : SubSystem
{
    /// <summary> Торпедные и дисраптерные установки. </summary>
    public List<TorpedoLauncher> Launchers;
    
    /// <summary> Канонир. </summary>
    private GunnerController Gunner;
    // Start is called before the first frame update
    public override void isCreated()
    {
        Launchers = Owner.GetComponentsInChildren<TorpedoLauncher>().ToList();
        
        foreach (TorpedoLauncher _l in Launchers)
        {
            _l.NecessarySystem = this;
        }
        
        if (!Owner.GetComponent<GunnerController>())
        {
            Gunner = Owner.gameObject.AddComponent<GunnerController>();

            Gunner.SecWea = this;
            Gunner.Owner = Owner;
        }
        else
        {
            Gunner = Owner.GetComponent<GunnerController>();
            Gunner.SecWea = this;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    /// <summary> Атаковать. </summary>
    public void Attack(SelectableObject target, STMethods.AttackType Aiming)
    {
        if (Launchers.Count > 0)
        {
            foreach (TorpedoLauncher _l in Launchers)
            {
                _l.Active(target, Aiming);
            }
        }
        
        ReturnAttackingTargets();
    }
    /// <summary> Атаковать не основную цель. </summary>
    public void AttackNotMainTarget(SelectableObject target, STMethods.AttackType Aiming)
    {
        if (Launchers.Count > 0)
        {
            foreach (TorpedoLauncher _l in Launchers)
            {
                if (_l.Target == null)
                {
                    _l.Active(target, Aiming);
                }
            }
        }

        ReturnAttackingTargets();
    }
    /// <summary> Прекратить атаку. </summary>
    public void StopFiring()
    {
        foreach (TorpedoLauncher _tl in Launchers)
        {
            _tl.Target = null;
        }
    }
    /// <summary> Возвращение списка целей, находящихся под атакой орудий. </summary>
    public void ReturnAttackingTargets()
    {
        List<SelectableObject> targets = new List<SelectableObject>(); 
            
        foreach (TorpedoLauncher _tl in Launchers)
        {
            if (_tl.Target != null)
            {
                targets.Add(_tl.Target);
            }
        }

        if (targets.Count > 0)
        {
            foreach (SelectableObject _tar in targets)
            {
                if (!STMethods.FindInList(_tar, Gunner.TargetsUnderAttack))
                {
                    Gunner.TargetsUnderAttack.Add(_tar);
                }
            }
        }
    }
    /// <summary> Цдвлить из листа атакуемых целей. </summary>
    public void DeleteFromTargetList(SelectableObject tar)
    {
        if (STMethods.FindInList(tar, Gunner.TargetsUnderAttack))
        {
            Gunner.TargetsUnderAttack.Remove(tar);
        }
    }
}
