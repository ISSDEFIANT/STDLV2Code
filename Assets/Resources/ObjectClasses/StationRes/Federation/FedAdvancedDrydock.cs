using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedAdvancedDrydock : StationType1
{
    // Start is called before the first frame update
   public override void Awake()
    {
        base.Awake();

        Quaternion init = this.transform.rotation;
        
        model = (GameObject)Instantiate(Resources.Load("Models/Federation/Stations/STDL_AdvancedDrydock/FedAdvancedDrydockPre"), transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 30;
        SensorRange = 120;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(300, 150);

        _hs.ExplosionEffect = (GameObject)Resources.Load("Effects/DamageAndDestructions/Explosions/BigShipExplosion");
        _hs.ExplosionEffectScale = 2;
        
        DockingHub _dh1 = new DockingHub();
        _dh1.ShipFixing = true;
        _dh1.ShipBuilding = true;
        _dh1.EnterPoint = new Vector3(-45, 0, 0);
        _dh1.StayPoint = new Vector3(0, 0, 0);
        _dh1.ExitPoint = new Vector3(45, 0, 0);
        _dh1.AbleToConstruct = new List<ConstructionContract>();
        
        ConstructionContract _sovereign = new ConstructionContract();
        _sovereign.Animation = (GameObject)Resources.Load("Models/Federation/Ships/STDL_Sovereign/SovereignAnim");
        _sovereign.Object = "SovereignClass";
        _sovereign.TitaniumCost = 3205;
        _sovereign.DilithiumCost = 500;
        _sovereign.CrewCost = 855;
        _sovereign.ConstructionTime = 60;
        
        _dh1.AbleToConstruct.Add(_sovereign);

        DockingHub[] _dh = new DockingHub[1] {_dh1};
        
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(400,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(450,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(360,this);
        StationDockingHubSS _sdh = (StationDockingHubSS)subModulesObj.AddComponent<StationDockingHubSS>().SetHubPurpose(_dh, new Vector3(-45, 0, 0), this);
        
        _hs.SubSystems = new SubSystem[4]{_wc,_ls,_ss,_sdh};
        _wc.WarpCoreExplosion = (GameObject)Resources.Load("Effects/DamageAndDestructions/WarpCoreDestroyingEffect/FedCoreDestroyed");
        
        initShilds(1,ShildsObj,_hs,750,180,100);

        FindInmodelElements();
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 150000000;
    }
}