using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sovereign : ShipType1
{
    /// <summary> Добавление подсистем, щитов и других параметров судна. </summary>
    public override void Awake()
    {
        base.Awake();
        
        _hs.InitHullAndCrew(500, 855);

        _hs.ExplosionEffect = (GameObject)Resources.Load("Effects/DamageAndDestructions/Explosions/BigShipExplosion");
        
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(300);
        SubSystem _sw = subModulesObj.AddComponent<SecondaryWeaponSS>().InitSystemHealth(450);
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(400);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(350);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(400);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(450);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(360);
        SubSystem _tb = subModulesObj.AddComponent<TractorBeamSS>().InitSystemHealth(400);
        
        _hs.SubSystems = new SubSystem[8]{_pw,_sw,_ie,_we,_wc,_ls,_ss,_tb};
        _wc.WarpCoreExplosion = (GameObject)Resources.Load("Effects/DamageAndDestructions/WarpCoreDestroyingEffect/FedCoreDestroyed");
        
        initShilds(1,ShildsObj,_hs,500,180,100);
        
        Modules = new Module[2]{gameObject.AddComponent<WeaponModule>(), gameObject.AddComponent<SensorModule>()};

        Vector3 plus = transform.rotation.eulerAngles + new Vector3(0,90,0);
        Quaternion init = this.transform.rotation;
        init.eulerAngles = plus;
        
        GameObject model = (GameObject)Instantiate(Resources.Load("Models/Federation/Ships/STDL_Sovereign/SovereignPre"), transform.position, init);

        model.transform.parent = transform;

        moveComponent.Model = model.transform;

        WeaponModule _wm = (WeaponModule)Modules[0];
        _wm.WeaponRange = 100;
        _wm.BeamWeapons = gameObject.GetComponentsInChildren<BeamWeapon>().ToList();
        foreach (BeamWeapon _bw in _wm.BeamWeapons)
        {
            _bw.WeaponSystem = _wm;
            _bw.NecessarySystem = _hs.SubSystems[0];
        }
        _wm.Launchers = gameObject.GetComponentsInChildren<TorpedoLauncher>().ToList();
        foreach (TorpedoLauncher _l in _wm.Launchers)
        {
            _l.WeaponSystem = _wm;
            _l.NecessarySystem = _hs.SubSystems[0];
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
