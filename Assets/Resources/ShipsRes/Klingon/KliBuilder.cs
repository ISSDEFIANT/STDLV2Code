using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KliBuilder : ShipType1
{
    /// <summary> Добавление подсистем, щитов и других параметров судна. </summary>
    public override void Awake()
    {
        base.Awake();
        
        _hs.InitHullAndCrew(150, 100);

        _hs.ExplosionEffect = (GameObject)Resources.Load("Effects/DamageAndDestructions/Explosions/NormallShipExplosion");
        
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(100);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(100);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(150);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(120);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(100);
        SubSystem _tb = subModulesObj.AddComponent<TractorBeamSS>().InitSystemHealth(80);
        
        _hs.SubSystems = new SubSystem[6]{_ie,_we,_wc,_ls,_ss,_tb};
        _wc.WarpCoreExplosion = (GameObject)Resources.Load("Effects/DamageAndDestructions/WarpCoreDestroyingEffect/KliCoreDestroyed");
        
        initShilds(1,ShildsObj,_hs,200,120,100);
        
        Modules = new Module[1]{gameObject.AddComponent<SensorModule>()};
        
        Quaternion init = this.transform.rotation;
        
        GameObject model = (GameObject)Instantiate(Resources.Load("Models/Klingon/Ships/Builder/BuilderPre"), transform.position, init);

        model.transform.parent = transform;
        
        moveComponent.Model = model.transform;
        moveComponent.MaxSpeed = 15;
        moveComponent.Acceleration = 5;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
