using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Controllers;

public class PrimaryWeaponSS : SubSystem
{
    /// <summary> Лучевые орудия. </summary>
    public List<BeamWeapon> BeamWeapons;
    
    /// <summary> Импульсные установки. </summary>
    public List<TorpedoLauncher> Launchers;

    /// <summary> Канонир. </summary>
    public GunnerController Gunner;
    // Start is called before the first frame update
    public override void isCreated()
    {
        BeamWeapons = Owner.GetComponentsInChildren<BeamWeapon>().ToList();
        
        foreach (BeamWeapon _bw in BeamWeapons)
        {
            _bw.NecessarySystem = this;
            
            List<BeamWeapon> test = new List<BeamWeapon>(BeamWeapons.ToList());
            test.Remove(_bw);

            _bw.otherWeapon = test;
        }
        
        Launchers = Owner.GetComponentsInChildren<TorpedoLauncher>().ToList();
        
        foreach (TorpedoLauncher _l in Launchers)
        {
            if (_l.ImpulsPhaser)
            {
                _l.PriNecessarySystem = this;
            }
        }

        if (!Owner.GetComponent<GunnerController>())
        {
            Gunner = Owner.gameObject.AddComponent<GunnerController>();

            Gunner.PriWea = this;
            Gunner.Owner = Owner;
        }
        else
        {
            Gunner = Owner.GetComponent<GunnerController>();
            Gunner.PriWea = this;
        }
        Owner.effectManager.primaryWeapon = this;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    /// <summary> Атаковать. </summary>
    public void Attack(SelectableObject target, STMethods.AttackType Aiming)
    {
        if (BeamWeapons.Count > 0)
        {
            foreach (BeamWeapon _pw in BeamWeapons)
            {
                List<BeamWeapon> test = new List<BeamWeapon>(BeamWeapons.ToList());
                test.Remove(_pw);
                _pw.Active(target, Aiming);
            }
        }
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
        if (BeamWeapons.Count > 0)
        {
            foreach (BeamWeapon _pw in BeamWeapons)
            {
                if (_pw.Target == null)
                {
                    List<BeamWeapon> test = new List<BeamWeapon>(BeamWeapons.ToList());
                    test.Remove(_pw);

                    _pw.Active(target, Aiming);
                }
            }
        }
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
        foreach (BeamWeapon _pw in BeamWeapons)
        {
            _pw.Target = null;
        }
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