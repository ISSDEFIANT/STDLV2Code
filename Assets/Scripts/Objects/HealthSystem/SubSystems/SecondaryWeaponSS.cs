using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Controllers;

public class SecondaryWeaponSS : SubSystem
{
    /// <summary> Торпедные установки. </summary>
    public List<TorpedoLauncher> Launchers;
    
    /// <summary> Канонир. </summary>
    public GunnerController Gunner;
    // Start is called before the first frame update
    public override void isCreated()
    {
        Launchers = Owner.GetComponentsInChildren<TorpedoLauncher>().ToList();
        
        foreach (TorpedoLauncher _l in Launchers)
        {
            if (!_l.ImpulsPhaser)
            {
                _l.SecNecessarySystem = this;
            }
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
        Owner.effectManager.secondaryWeapon = this;
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
    }
    /// <summary> Атаковать не основную цель. </summary>
    public void AttackNotMainTarget(SelectableObject target, STMethods.AttackType Aiming)
    {
        if (target == null) return;
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
    }
    /// <summary> Прекратить атаку. </summary>
    public void StopFiring()
    {
        foreach (TorpedoLauncher _tl in Launchers)
        {
            _tl.Target = null;
        }
    }

    /// <summary> Удалить из листа атакуемых целей. </summary>
    public void DeleteFromTargetList(SelectableObject tar)
    {
        if (Gunner.TargetsUnderAttack.Any(x => x == tar))
        {
            Gunner.TargetsUnderAttack.Remove(tar);
        }
    }
}
