using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedDrydock : StationType1
{
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Drydock";
        
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationDrydock/Icon"] as Sprite;

        Quaternion init = this.transform.rotation;

        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationDrydock"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 18;
        SensorRange = 150;

        WeaponRange = 0;
        MaxAttackTargetCount = 0;

        _hs.InitHullAndCrew(300, 450);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 1.5f;

        DockingHub _dh1 = new DockingHub();
        _dh1.ShipFixing = true;
        _dh1.ShipBuilding = true;
        _dh1.ShipDeassembling = true;
        _dh1.EnterPoint = new Vector3(-25, 0, 0);
        _dh1.StayPoint = new Vector3(0, 0, 0);
        _dh1.ExitPoint = new Vector3(25, 0, 0);
        _dh1.AbleToConstruct = new List<ConstructionContract>();

        ConstructionContract _defian = new ConstructionContract();
        _defian.Icon = DataLoader.Instance.ResourcesCache["Defiant/Icon"] as Sprite;
        _defian.ObjectName = "Defiant class";
        _defian.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Defiant/Animation"];
        _defian.Object = "DefiantClass";
        _defian.TitaniumCost = 355;
        _defian.DilithiumCost = 50;
        _defian.CrewCost = 50;
        _defian.ConstructionTime = 35;
        _defian.MaxConstructionTime = 35;
        _defian.NameIndex = -1;
        _defian.MaxIndexCount = NameCounter.Defiant.Names.Count;
        
        ConstructionContract _nova = new ConstructionContract();
        _nova.Icon = DataLoader.Instance.ResourcesCache["Nova/Icon"] as Sprite;
        _nova.ObjectName = "Nova class";
        _nova.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Nova/Animation"];
        _nova.Object = "NovaClass";
        _nova.TitaniumCost = 147;
        _nova.DilithiumCost = 40;
        _nova.CrewCost = 80;
        _nova.ConstructionTime = 45;
        _nova.MaxConstructionTime = 45;
        _nova.NameIndex = -1;
        _nova.IndexList = GameManager.instance.NamesIndexes.NovaIndexes;
        _nova.MaxIndexCount = NameCounter.Nova.Names.Count;
        
        ConstructionContract _saber = new ConstructionContract();
        _saber.Icon = DataLoader.Instance.ResourcesCache["Saber/Icon"] as Sprite;
        _saber.ObjectName = "Saber class";
        _saber.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Saber/Animation"];
        _saber.Object = "SaberClass";
        _saber.TitaniumCost = 310;
        _saber.DilithiumCost = 100;
        _saber.CrewCost = 40;
        _saber.ConstructionTime = 60;
        _saber.MaxConstructionTime = 60;
        _saber.NameIndex = -1;
        _saber.IndexList = GameManager.instance.NamesIndexes.SaberIndexes;
        _saber.MaxIndexCount = NameCounter.Saber.Names.Count;
        
        ConstructionContract _luna = new ConstructionContract();
        _luna.Icon = DataLoader.Instance.ResourcesCache["Luna/Icon"] as Sprite;
        _luna.ObjectName = "Luna class";
        _luna.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Luna/Animation"];
        _luna.Object = "LunaClass";
        _luna.TitaniumCost = 355;
        _luna.DilithiumCost = 100;
        _luna.CrewCost = 350;
        _luna.ConstructionTime = 80;
        _luna.MaxConstructionTime = 80;
        _luna.NameIndex = -1;
        _luna.IndexList = GameManager.instance.NamesIndexes.LunaIndexes;
        _luna.MaxIndexCount = NameCounter.Luna.Names.Count;
        
        ConstructionContract _steamrunner = new ConstructionContract();
        _steamrunner.Icon = DataLoader.Instance.ResourcesCache["Steamrunner/Icon"] as Sprite;
        _steamrunner.ObjectName = "Steamrunner class";
        _steamrunner.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Steamrunner/Animation"];
        _steamrunner.Object = "SteamrunnerClass";
        _steamrunner.TitaniumCost = 376;
        _steamrunner.DilithiumCost = 70;
        _steamrunner.CrewCost = 200;
        _steamrunner.ConstructionTime = 100;
        _steamrunner.MaxConstructionTime = 100;
        _steamrunner.NameIndex = -1;
        _steamrunner.IndexList = GameManager.instance.NamesIndexes.SteamrunnerIndexes;
        _steamrunner.MaxIndexCount = NameCounter.Steamrunner.Names.Count;

        _dh1.AbleToConstruct.Add(_defian);
        _dh1.AbleToConstruct.Add(_nova);
        _dh1.AbleToConstruct.Add(_saber);
        _dh1.AbleToConstruct.Add(_luna);
        _dh1.AbleToConstruct.Add(_steamrunner);

        DockingHub[] _dh = new DockingHub[1] {_dh1};

        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(200, this);
        WarpCoreSS _wc = (WarpCoreSS) subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(200, this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(200, this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(150, this);
        StationDockingHubSS _sdh = (StationDockingHubSS) subModulesObj.AddComponent<StationDockingHubSS>()
            .SetHubPurpose(_dh, new Vector3(-25, 0, 0), Vector3.left, this);
        
        SubsystemEffectsManager inModelPoints = model.GetComponentInChildren<SubsystemEffectsManager>();
        inModelPoints.effects[0].controllingSubsystem = _ie;
        _ie.effects = inModelPoints.effects[0];
        inModelPoints.effects[1].controllingSubsystem = _wc;
        _wc.effects = inModelPoints.effects[1];
        inModelPoints.effects[2].controllingSubsystem = _ls;
        _ls.effects = inModelPoints.effects[2];
        inModelPoints.effects[3].controllingSubsystem = _ss;
        _ss.effects = inModelPoints.effects[3];

        _hs.HullPoints = inModelPoints.effects[4].AimingPoints;

        _hs.SubSystems = new SubSystem[5] {_ie, _wc, _ls, _ss, _sdh};
        _wc.WarpCoreExplosion = (GameObject) DataLoader.Instance.ResourcesCache["FederationCoreDestruction"];

            initShilds(1, ShildsObj, _hs, 150, 180, 100);

        FindInmodelElements();

        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 3000000;
        
        DilithiumCost = 200;
        TitaniumCost = 300;
        CrewCost = 300;

        canBeDeassembled = true;
        DeassembledAnim = DataLoader.Instance.ResourcesCache["FederationDrydock/Animation"] as GameObject;

        DeassebleTime = 60;
        
        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.ConstructionStation;
    }
}
