using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Atlantia : ShipType1
{
    /// <summary> Добавление подсистем, щитов и других параметров судна. </summary>
    public override void Awake()
    {
        base.Awake();

        ObjectClass = "Atlantia class";
        ObjectIcon = DataLoader.Instance.ResourcesCache["Atlantia/Icon"] as Sprite;

        AttackProbability.AlphaProbability = 0.25f;
        AttackProbability.BetaProbability = 0.75f;
        AttackProbability.GammaProbability = 1;

        _hs.InitHullAndCrew(150, 100);

        ObjectRadius = 14;
        SensorRange = 200;

        WeaponRange = 150;
        MaxAttackTargetCount = 1;

        _hs.ExplosionEffect = (GameObject) DataLoader.Instance.ResourcesCache["MediumShipExplosion"];

        Quaternion init = this.transform.rotation;

        model = Instantiate(DataLoader.Instance.ResourcesCache["Atlantia"] as GameObject, transform.position, init);

        model.transform.parent = transform;
        
        
        ConstructionContract _starbase = new ConstructionContract();
        _starbase.Icon = Resources.Load <Sprite>("Textures/Icons/Federation/Stations/StarbaseIcon");
        _starbase.Ghost = (GameObject)Resources.Load("Models/Federation/Stations/STDL_Starbase/FedStarbaseGhost");
        _starbase.Object = "FedStarBase";
        _starbase.ObjectUnderConstruction = "FedStarBaseInProgress";
        _starbase.TitaniumCost = 10000;
        _starbase.DilithiumCost = 7000;
        _starbase.CrewCost = 2000;
        _starbase.ObjectRadius = 120;
        _starbase.ObjectCategory = 1;
        _starbase.BuilderEfficiency = 50;
        
        ConstructionContract _drydock = new ConstructionContract();
        _drydock.Icon = Resources.Load <Sprite>("Textures/Icons/Federation/Stations/DrydockIcon");
        _drydock.Ghost = (GameObject)Resources.Load("Models/Federation/Stations/STDL_Drydock/FedDrydockGhost");
        _drydock.Object = "FedDrydock";
        _drydock.ObjectUnderConstruction = "FedDrydockInProgress";
        _drydock.TitaniumCost = 300;
        _drydock.DilithiumCost = 200;
        _drydock.CrewCost = 300;
        _drydock.ObjectRadius = 15;
        _drydock.ObjectCategory = 1;
        _drydock.BuilderEfficiency = 10;
        
        ConstructionContract _shipyard = new ConstructionContract();
        _shipyard.Icon = Resources.Load <Sprite>("Textures/Icons/Federation/Stations/ShipyardIcon");
        _shipyard.Ghost = (GameObject)Resources.Load("Models/Federation/Stations/STDL_AdvancedDrydock/FedAdvancedDrydockGhost");
        _shipyard.Object = "FedShipyard";
        _shipyard.ObjectUnderConstruction = "FedShipyardInProgress";
        _shipyard.TitaniumCost = 500;
        _shipyard.DilithiumCost = 400;
        _shipyard.CrewCost = 450;
        _shipyard.ObjectRadius = 30;
        _shipyard.ObjectCategory = 1;
        _shipyard.BuilderEfficiency = 10;
        
        ConstructionContract _scistation = new ConstructionContract();
        _scistation.Icon = Resources.Load <Sprite>("Textures/Icons/Federation/Stations/SciStationIcon");
        _scistation.Ghost = (GameObject)Resources.Load("Models/Federation/Stations/STDL_SciStation/FedSciStationGhost");
        _scistation.Object = "FedSciStation";
        _scistation.ObjectUnderConstruction = "FedSciStationInProgress";
        _scistation.TitaniumCost = 900;
        _scistation.DilithiumCost = 500;
        _scistation.CrewCost = 450;
        _scistation.ObjectRadius = 30;
        _scistation.ObjectCategory = 1;
        _scistation.BuilderEfficiency = 5;
        
        ConstructionContract _miningstation = new ConstructionContract();
        _miningstation.Icon = Resources.Load <Sprite>("Textures/Icons/Federation/Stations/MiningStationIcon");
        _miningstation.Ghost = (GameObject)Resources.Load("Models/Federation/Stations/STDL_MineStation/FederationMiningStationGhost");
        _miningstation.Object = "FedMiningStation";
        _miningstation.ObjectUnderConstruction = "FedMiningStationInProgress";
        _miningstation.TitaniumCost = 1000;
        _miningstation.DilithiumCost = 700;
        _miningstation.CrewCost = 2500;
        _miningstation.ObjectRadius = 60;
        _miningstation.ObjectCategory = 2;
        _miningstation.BuilderEfficiency = 10;
        
        ConstructionContract _outpost = new ConstructionContract();
        _outpost.Icon = Resources.Load <Sprite>("Textures/Icons/Federation/Stations/OutpostIcon");
        _outpost.Ghost = (GameObject)Resources.Load("Models/Federation/Stations/STDL_Outpost/FedOutpostGhost");
        _outpost.Object = "FedOutpost";
        _outpost.ObjectUnderConstruction = "FedOutpostInProgress";
        _outpost.TitaniumCost = 25000;
        _outpost.DilithiumCost = 7000;
        _outpost.CrewCost = 2000;
        _outpost.ObjectRadius = 60;
        _outpost.ObjectCategory = 3;
        _outpost.BuilderEfficiency = 50;
        
        ConstructionContract[] _ac = new ConstructionContract[6]{_starbase, _drydock, _shipyard, _scistation, _miningstation, _outpost};

        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(100, this);
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(100, this);
        SubSystem _we = subModulesObj.AddComponent<WarpEngineSS>().InitSystemHealth(100, this);
        WarpCoreSS _wc = (WarpCoreSS) subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(150, this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(120, this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(100, this);
        SubSystem _tb = subModulesObj.AddComponent<TractorBeamSS>().InitSystemHealth(80, this);
        SubSystem _mb = subModulesObj.AddComponent<MiningBeamSS>().SetMaxResources(150, true, true, false, this);
        SubSystem _bcc = subModulesObj.AddComponent<BuilderConstructionSS>().SetBuilderConstructionList(_ac.ToList(), this);
        
        SubsystemEffectsManager inModelPoints = model.GetComponentInChildren<SubsystemEffectsManager>();
        inModelPoints.effects[0].controllingSubsystem = _pw;
        _pw.effects = inModelPoints.effects[0];
        inModelPoints.effects[1].controllingSubsystem = _ie;
        _ie.effects = inModelPoints.effects[1];
        inModelPoints.effects[2].controllingSubsystem = _we;
        _we.effects = inModelPoints.effects[2];
        inModelPoints.effects[3].controllingSubsystem = _wc;
        _wc.effects = inModelPoints.effects[3];
        inModelPoints.effects[4].controllingSubsystem = _ls;
        _ls.effects = inModelPoints.effects[4];
        inModelPoints.effects[5].controllingSubsystem = _ss;
        _ss.effects = inModelPoints.effects[5];
        inModelPoints.effects[6].controllingSubsystem = _tb;
        _tb.effects = inModelPoints.effects[6];
        
        _hs.HullPoints = inModelPoints.effects[7].AimingPoints;

        _hs.SubSystems = new SubSystem[9] {_pw, _ie, _we, _wc, _ls, _ss, _tb, _mb, _bcc};
        _wc.WarpCoreExplosion = (GameObject) DataLoader.Instance.ResourcesCache["FederationCoreDestruction"];

        initShilds(1, ShildsObj, _hs, 200, 60, 100);

        Threshold = 1f;

        moveComponent.Model = model.transform;
        moveComponent.MaxSpeed = 15;
        moveComponent.Acceleration = 5;
        moveComponent.WarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["FederationWarpBlink"];
        moveComponent.BorgWarpBlink = (GameObject) DataLoader.Instance.ResourcesCache["BorgWarpBlink"];

        FindInmodelElements();

        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 280000;

        DilithiumCost = 80;
        TitaniumCost = 280;
        CrewCost = 100;

        canBeDeassembled = true;
        DeassembledAnim = (GameObject)DataLoader.Instance.ResourcesCache["Atlantia/Animation"];

        DeassebleTime = 20;

        ObjectBluePrint[0] = DataLoader.Instance.ResourcesCache["Atlantia/PrimaryWeapons"] as Sprite;
        ObjectBluePrint[2] = DataLoader.Instance.ResourcesCache["Atlantia/ImpulseEngines"] as Sprite;
        ObjectBluePrint[3] = DataLoader.Instance.ResourcesCache["Atlantia/WarpEngines"] as Sprite;
        ObjectBluePrint[4] = DataLoader.Instance.ResourcesCache["Atlantia/WarpCore"] as Sprite;
        ObjectBluePrint[5] = DataLoader.Instance.ResourcesCache["Atlantia/LifeSupport"] as Sprite;
        ObjectBluePrint[6] = DataLoader.Instance.ResourcesCache["Atlantia/Sensors"] as Sprite;
        ObjectBluePrint[7] = DataLoader.Instance.ResourcesCache["Atlantia/Tracktor"] as Sprite;
        ObjectBluePrint[8] = DataLoader.Instance.ResourcesCache["Atlantia/Hull"] as Sprite;
        
        TractorBeam tractor = gameObject.AddComponent<TractorBeam>();
        tractor.owner = this;
        tractor.Radius = 40;
        
        SoundController _sc = gameObject.AddComponent<SoundController>();
        _sc.Owner = this;
        _sc.NormalSound = DataLoader.Instance.ResourcesCache["FederationShipSound"] as AudioClip;
        _sc.BorgSound = DataLoader.Instance.ResourcesCache["BorgShipSound"] as AudioClip;
    }
    
    public override void Update()
    {
        base.Update();
        if (ConstructedOnDock)
        {
            UndockingAfterConstruction();
            ConstructedOnDock = false;
            nameIndexList = GameManager.instance.NamesIndexes.AtlantiaIndexes;
            InitNames(NameCounter.Atlantia);
        }
        else
        {
            if (nameIndex == -1)
            {
                nameIndexList = GameManager.instance.NamesIndexes.AtlantiaIndexes;
                GetRandomIndex(GameManager.instance.NamesIndexes.AtlantiaIndexes, NameCounter.Atlantia.Names.Count);
                if(nameIndex != -2) InitNames(NameCounter.Atlantia);
            }
        }
    }
}
