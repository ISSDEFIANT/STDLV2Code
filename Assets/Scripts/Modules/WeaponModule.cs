using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponModule : Module
{
    public float WeaponRange;

    public STMethods.Alerts Alerts;
    public STMethods.AttackType Aiming;
    
    public List<SelectableObject> Targets;
    public SelectableObject MainTarget;

    public List<BeamWeapon> BeamWeapons;
    public List<TorpedoLauncher> Launchers;
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
    // Start is called before the first frame update
    void Awake()
    {
        Targets = new List<SelectableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
