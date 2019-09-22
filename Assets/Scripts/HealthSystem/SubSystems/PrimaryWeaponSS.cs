using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Controllers;

public class PrimaryWeaponSS : SubSystem
{
    /// <summary> Лучевые орудия. </summary>
    public List<BeamWeapon> BeamWeapons;

    /// <summary> Канонир. </summary>
    private GunnerController Gunner;
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
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        ReturnAttackingTargets();
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
        
        ReturnAttackingTargets();
    }
    /// <summary> Атаковать не основную цель. </summary>
    public void AttackNotMainTarget(SelectableObject target, STMethods.AttackType Aiming)
    {
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
        
        ReturnAttackingTargets();
    }
    /// <summary> Прекратить атаку. </summary>
    public void StopFiring()
    {
        foreach (BeamWeapon _pw in BeamWeapons)
        {
            _pw.Target = null;
        }
    }
    /// <summary> Возвращение списка целей, находящихся под атакой орудий. </summary>
    public void ReturnAttackingTargets()
    {
        List<SelectableObject> targets = new List<SelectableObject>(); 
            
        foreach (BeamWeapon _pw in BeamWeapons)
        {
            if (_pw.Target != null)
            {
                targets.Add(_pw.Target);
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
    /// <summary> Удалить из листа атакуемых целей. </summary>
    public void DeleteFromTargetList(SelectableObject tar)
    {
        if (STMethods.FindInList(tar, Gunner.TargetsUnderAttack))
        {
            Gunner.TargetsUnderAttack.Remove(tar);
        }
    }
}