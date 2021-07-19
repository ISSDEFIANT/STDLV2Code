using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defiant : ShipType1
{
    /// <summary> Добавление подсистем, щитов и других параметров судна. </summary>
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Defiant class";
        ObjectIcon = DataLoader.Instance.ResourcesCache["Defiant/Icon"] as Sprite;

        AttackProbability.AlphaProbability = 0;
        AttackProbability.BetaProbability = 1;
        AttackProbability.GammaProbability = 1;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["Defiant"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 4;
        SensorRange = 200;
        
        WeaponRange = 170;
        MaxAttackTargetCount = 1;
        
        _hs.InitHullAndCrew(200, 50);
        _hs.maxEnergy = 100;
        _hs.curEnergy = 100;

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["SmallShipExplosion"];
        
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(150,this);
        SubSystem _sw = subModulesObj.AddComponent<SecondaryWeaponSS>().InitSystemHealth(150,this);
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(130,this);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(100,this);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(150,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(180,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(130,this);
        SubSystem _tb = subModulesObj.AddComponent<TractorBeamSS>().InitSystemHealth(100,this);
        
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
        
        initShilds(1,ShildsObj,_hs,350,120,100);

        FindInmodelElements();

        Threshold = 3f;        
        
        moveComponent.Model = model.transform;
        moveComponent.MaxSpeed = 45;
        moveComponent.Acceleration = 10;
        moveComponent.WarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["FederationWarpBlink"];
        moveComponent.BorgWarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["BorgWarpBlink"];
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 355000;

        DilithiumCost = 50;
        TitaniumCost = 355;
        CrewCost = 50;

        canBeDeassembled = true;
        DeassembledAnim = (GameObject) DataLoader.Instance.ResourcesCache["Defiant/Animation"];

        DeassebleTime = 35;
        
        ObjectBluePrint[0] = DataLoader.Instance.ResourcesCache["Defiant/PrimaryWeapons"] as Sprite;
        ObjectBluePrint[1] = DataLoader.Instance.ResourcesCache["Defiant/SecondaryWeapons"] as Sprite;
        ObjectBluePrint[2] = DataLoader.Instance.ResourcesCache["Defiant/ImpulseEngines"] as Sprite;
        ObjectBluePrint[3] = DataLoader.Instance.ResourcesCache["Defiant/WarpEngines"] as Sprite;
        ObjectBluePrint[4] = DataLoader.Instance.ResourcesCache["Defiant/WarpCore"] as Sprite;
        ObjectBluePrint[5] = DataLoader.Instance.ResourcesCache["Defiant/LifeSupport"] as Sprite;
        ObjectBluePrint[6] = DataLoader.Instance.ResourcesCache["Defiant/Sensors"] as Sprite;
        ObjectBluePrint[7] = DataLoader.Instance.ResourcesCache["Defiant/Tracktor"] as Sprite;
        ObjectBluePrint[8] = DataLoader.Instance.ResourcesCache["Defiant/Hull"] as Sprite;
        
        TractorBeam tractor = gameObject.AddComponent<TractorBeam>();
        tractor.owner = this;
        tractor.Radius = 20;

        AntimatterMines defiantMines = gameObject.AddComponent<AntimatterMines>();
        defiantMines.owner = this;
        
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
            nameIndexList = GameManager.instance.NamesIndexes.DefianIndexes;
            InitNames(NameCounter.Defiant);
        }
        else
        {
            if (nameIndex == -1)
            {
                nameIndexList = GameManager.instance.NamesIndexes.DefianIndexes;
                GetRandomIndex(GameManager.instance.NamesIndexes.DefianIndexes, NameCounter.Defiant.Names.Count);
                if(nameIndex != -2) InitNames(NameCounter.Defiant);
            }
        }
    }
}
