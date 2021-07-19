using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excalibur : ShipType1
{
    /// <summary> Добавление подсистем, щитов и других параметров судна. </summary>
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Excalibur class";
        ObjectIcon = DataLoader.Instance.ResourcesCache["Excalibur/Icon"] as Sprite;

        AttackProbability.AlphaProbability = 0.1f;
        AttackProbability.BetaProbability = 0.3f;
        AttackProbability.GammaProbability = 1;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["Excalibur"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 25;
        SensorRange = 450;
        
        WeaponRange = 430;
        MaxAttackTargetCount = 4;
        
        _hs.InitHullAndCrew(700, 2000);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 2;
        
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(5000,this);
        SubSystem _sw = subModulesObj.AddComponent<SecondaryWeaponSS>().InitSystemHealth(4000,this);
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(4000,this);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(4300,this);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(7000,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(6000,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(8000,this);
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
        
        initShilds(1,ShildsObj,_hs,1500,180,100);

        FindInmodelElements();

        Threshold = 3f;        
        
        moveComponent.Model = model.transform;
        moveComponent.MaxSpeed = 13;
        moveComponent.Acceleration = 5;
        moveComponent.WarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["FederationWarpBlink"];
        moveComponent.BorgWarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["BorgWarpBlink"];
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 10000000;

        DilithiumCost = 1000;
        TitaniumCost = 10000;
        CrewCost = 2000;

        canBeDeassembled = true;
        DeassembledAnim = (GameObject) DataLoader.Instance.ResourcesCache["Excalibur/Animation"];

        DeassebleTime = 200;
        
        TractorBeam tractor = gameObject.AddComponent<TractorBeam>();
        tractor.owner = this;
        tractor.Radius = 60;
        
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
            nameIndexList = GameManager.instance.NamesIndexes.ExcaliburIndexes;
            InitNames(NameCounter.Excalibur);
        }
        else
        {
            if (nameIndex == -1)
            {
                nameIndexList = GameManager.instance.NamesIndexes.ExcaliburIndexes;
                GetRandomIndex(GameManager.instance.NamesIndexes.ExcaliburIndexes, NameCounter.Excalibur.Names.Count);
                if(nameIndex != -2) InitNames(NameCounter.Excalibur);
            }
        }
    }
}
