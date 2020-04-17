using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedMiningStation : StationType1
{
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        Quaternion init = this.transform.rotation;
        
        model = (GameObject)Instantiate(Resources.Load("Models/Federation/Stations/STDL_Minenstation/FederationMiningStationPre"), transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 60;
        SensorRange = 150;
        
        WeaponRange = 100;
        MaxAttackTargetCount = 4;
        
        _hs.InitHullAndCrew(500, 300);

        _hs.ExplosionEffect = (GameObject)Resources.Load("Effects/DamageAndDestructions/Explosions/BigShipExplosion");
        _hs.ExplosionEffectScale = 3;
        
        DockingHub _dh1 = new DockingHub();
        _dh1.ResourceUnloading = true;
        _dh1.EnterPoint = new Vector3(70, 2, 0);
        _dh1.StayPoint = new Vector3(0, 2, 0);
        _dh1.ExitPoint = new Vector3(-70, 2, 0);
        
        DockingHub _dh2 = new DockingHub();
        _dh2.ResourceUnloading = true;
        _dh2.EnterPoint = new Vector3(0, -8, -70);
        _dh2.StayPoint = new Vector3(0, -8, 0);
        _dh2.ExitPoint = new Vector3(0, -8, 70);

        DockingHub[] _dh = new DockingHub[2] {_dh1, _dh2};
        
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(400,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(450,this);
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(360,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(360,this);
        StationDockingHubSS _sdh = (StationDockingHubSS)subModulesObj.AddComponent<StationDockingHubSS>().SetHubPurpose(_dh, new Vector3(-70, 0, -70), this);
        
        _hs.SubSystems = new SubSystem[5]{_wc,_ls,_ss,_pw,_sdh};
        _wc.WarpCoreExplosion = (GameObject)Resources.Load("Effects/DamageAndDestructions/WarpCoreDestroyingEffect/FedCoreDestroyed");
        
        initShilds(1,ShildsObj,_hs,500,180,100);

        FindInmodelElements();
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 150000000;
    }
}
