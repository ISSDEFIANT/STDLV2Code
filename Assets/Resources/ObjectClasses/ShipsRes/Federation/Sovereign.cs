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

        AttackProbability.AlphaProbability = 0.5f;
        AttackProbability.BetaProbability = 0.75f;
        AttackProbability.GammaProbability = 1;
        
        Quaternion init = this.transform.rotation;
        
        model = (GameObject)Instantiate(Resources.Load("Models/Federation/Ships/STDL_Sovereign/SovereignPre"), transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 15;
        SensorRange = 100;
        
        WeaponRange = 100;
        MaxAttackTargetCount = 2;
        
        _hs.InitHullAndCrew(500, 855);

        _hs.ExplosionEffect = (GameObject)Resources.Load("Effects/DamageAndDestructions/Explosions/BigShipExplosion");
        
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(300,this);
        SubSystem _sw = subModulesObj.AddComponent<SecondaryWeaponSS>().InitSystemHealth(450,this);
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(400,this);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(350,this);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(400,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(450,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(360,this);
        SubSystem _tb = subModulesObj.AddComponent<TractorBeamSS>().InitSystemHealth(400,this);
        
        _hs.SubSystems = new SubSystem[8]{_pw,_sw,_ie,_we,_wc,_ls,_ss,_tb};
        _wc.WarpCoreExplosion = (GameObject)Resources.Load("Effects/DamageAndDestructions/WarpCoreDestroyingEffect/FedCoreDestroyed");
        
        initShilds(1,ShildsObj,_hs,500,180,100);

        FindInmodelElements();

        Threshold = 3f;        
        
        moveComponent.Model = model.transform;
        moveComponent.MaxSpeed = 17;
        moveComponent.Acceleration = 5;
        moveComponent.WarpBlink = (GameObject)Resources.Load("Effects/Warp/FedWarpEffect");
        moveComponent.BorgWarpBlink = (GameObject)Resources.Load("Effects/Warp/BorgWarpEffect");
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 3205000;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (ConstructedOnDock)
        {
            UndockingAfterConstruction();
            ConstructedOnDock = false;
        }
    }
}
