using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KliBuilder : ShipType1
{
    // Start is called before the first frame update
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
        
        Vector3 plus = transform.rotation.eulerAngles + new Vector3(0,180,0);
        Quaternion init = this.transform.rotation;
        init.eulerAngles = plus;
        
        GameObject model = (GameObject)Instantiate(Resources.Load("Models/Klingon/Ships/Builder/BuilderPre"), transform.position, init);

        model.transform.parent = transform;
        
        moveComponent.Model = model.transform;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
