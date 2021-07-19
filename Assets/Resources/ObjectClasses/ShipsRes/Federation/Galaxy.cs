using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : ShipType1
{
    /// <summary> Добавление подсистем, щитов и других параметров судна. </summary>
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Galaxy class";
        ObjectIcon = DataLoader.Instance.ResourcesCache["Galaxy/Icon"] as Sprite;

        AttackProbability.AlphaProbability = 0.4f;
        AttackProbability.BetaProbability = 0.5f;
        AttackProbability.GammaProbability = 1;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["Galaxy"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 13;
        SensorRange = 400;
        
        WeaponRange = 350;
        MaxAttackTargetCount = 2;
        
        _hs.InitHullAndCrew(600, 1000);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(3000,this);
        SubSystem _sw = subModulesObj.AddComponent<SecondaryWeaponSS>().InitSystemHealth(3500,this);
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(2000,this);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(3400,this);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(2800,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(3100,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(2000,this);
        SubSystem _tb = subModulesObj.AddComponent<TractorBeamSS>().InitSystemHealth(3000,this);
        
        SubsystemEffectsManager inModelPoints = model.GetComponentInChildren<SubsystemEffectsManager>();
        inModelPoints.effects[0].controllingSubsystem = _pw;
        _pw.effects = inModelPoints.effects[0];
        inModelPoints.effects[1].controllingSubsystem = _sw;
        _sw.effects = inModelPoints.effects[1];
        inModelPoints.effects[2].controllingSubsystem = _ie;
        _ie.effects = inModelPoints.effects[2];
        inModelPoints.effects[3].controllingSubsystem = _we;
        _we.effects = inModelPoints.effects[3];
        inModelPoints.effects[4].controllingSubsystem = _wc;
        _wc.effects = inModelPoints.effects[4];
        inModelPoints.effects[5].controllingSubsystem = _ls;
        _ls.effects = inModelPoints.effects[5];
        inModelPoints.effects[6].controllingSubsystem = _ss;
        _ss.effects = inModelPoints.effects[6];
        inModelPoints.effects[7].controllingSubsystem = _tb;
        _tb.effects = inModelPoints.effects[7];
        
        _hs.HullPoints = inModelPoints.effects[8].AimingPoints;
        
        _hs.SubSystems = new SubSystem[8]{_pw,_sw,_ie,_we,_wc,_ls,_ss,_tb};
        _wc.WarpCoreExplosion = (GameObject) DataLoader.Instance.ResourcesCache["FederationCoreDestruction"];
        
        initShilds(1,ShildsObj,_hs,1200,120,100);

        FindInmodelElements();

        Threshold = 3f;
        
        moveComponent.Model = model.transform;
        moveComponent.MaxSpeed = 10;
        moveComponent.Acceleration = 5;
        moveComponent.WarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["FederationWarpBlink"];
        moveComponent.BorgWarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["BorgWarpBlink"];
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 5000000;

        DilithiumCost = 1000;
        TitaniumCost = 5000;
        CrewCost = 1000;

        canBeDeassembled = true;
        DeassembledAnim = (GameObject) DataLoader.Instance.ResourcesCache["Galaxy/Animation"];

        DeassebleTime = 160;
        
        TractorBeam tractor = gameObject.AddComponent<TractorBeam>();
        tractor.owner = this;
        tractor.Radius = 50;
        
        SoundController _sc = gameObject.AddComponent<SoundController>();
        _sc.Owner = this;
        _sc.NormalSound = DataLoader.Instance.ResourcesCache["FederationShipSound"] as AudioClip;
        _sc.BorgSound = DataLoader.Instance.ResourcesCache["BorgShipSound"] as AudioClip;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (ConstructedOnDock)
        {
            UndockingAfterConstruction();
            ConstructedOnDock = false;
            nameIndexList = GameManager.instance.NamesIndexes.GalaxyIndexes;
            InitNames(NameCounter.Galaxy);
        }
        else
        {
            if (nameIndex == -1)
            {
                nameIndexList = GameManager.instance.NamesIndexes.GalaxyIndexes;
                GetRandomIndex(GameManager.instance.NamesIndexes.GalaxyIndexes, NameCounter.Galaxy.Names.Count);
                if(nameIndex != -2) InitNames(NameCounter.Galaxy);
            }
        }
    }
}
