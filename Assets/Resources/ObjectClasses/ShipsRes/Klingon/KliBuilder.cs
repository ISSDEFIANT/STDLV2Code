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
        
        AttackProbability.AlphaProbability = 0;
        AttackProbability.BetaProbability = 0.5f;
        AttackProbability.GammaProbability = 1;
        
        _hs.InitHullAndCrew(150, 100);

        WeaponRange = 50;
        
        _hs.ExplosionEffect = (GameObject)Resources.Load("Effects/DamageAndDestructions/Explosions/NormallShipExplosion");
        
        Quaternion init = this.transform.rotation;
        
        model = (GameObject)Instantiate(Resources.Load("Models/Klingon/Ships/Builder/BuilderPre"), transform.position, init);

        model.transform.parent = transform;
        
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(100,this);
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(100,this);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(100,this);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(150,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(120,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(100,this);
        SubSystem _tb = subModulesObj.AddComponent<TractorBeamSS>().InitSystemHealth(80,this);
        SubSystem _mb = subModulesObj.AddComponent<MiningBeamSS>().SetMaxResources(150, true, true, false, this);
        
        _hs.SubSystems = new SubSystem[8]{_pw,_ie,_we,_wc,_ls,_ss,_tb,_mb};
        _wc.WarpCoreExplosion = (GameObject)Resources.Load("Effects/DamageAndDestructions/WarpCoreDestroyingEffect/KliCoreDestroyed");
        
        initShilds(1,ShildsObj,_hs,200,120,100);
        
        ObjectRadius = 10;
        SensorRange = 80;
        
        WeaponRange = 50;
        MaxAttackTargetCount = 1;
        
        Threshold = 1f;        
        
        moveComponent.Model = model.transform;
        moveComponent.MaxSpeed = 15;
        moveComponent.Acceleration = 5;
        moveComponent.WarpBlink = (GameObject)Resources.Load("Effects/Warp/KliWarpEffect");
        moveComponent.BorgWarpBlink = (GameObject)Resources.Load("Effects/Warp/BorgWarpEffect");
        
        FindInmodelElements();
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 200000;
    }
}
