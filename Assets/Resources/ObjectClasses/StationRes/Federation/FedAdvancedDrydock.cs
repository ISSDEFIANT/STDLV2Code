using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedAdvancedDrydock : StationType1
{
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Shipyard";
        
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationAdvancedDrydock/Icon"] as Sprite;

        Quaternion init = this.transform.rotation;

        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationAdvancedDrydock"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 30;
        SensorRange = 200;

        WeaponRange = 0;
        MaxAttackTargetCount = 0;

        _hs.InitHullAndCrew(650, 450);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 2;

        DockingHub _dh1 = new DockingHub();
        _dh1.ShipFixing = true;
        _dh1.ShipBuilding = true;
        _dh1.ShipDeassembling = true;
        _dh1.EnterPoint = new Vector3(-60, 0, 0);
        _dh1.StayPoint = new Vector3(0, 0, 0);
        _dh1.ExitPoint = new Vector3(60, 0, 0);
        _dh1.AbleToConstruct = new List<ConstructionContract>();

        ConstructionContract _akira = new ConstructionContract();
        _akira.Icon = DataLoader.Instance.ResourcesCache["Akira/Icon"] as Sprite;
        _akira.ObjectName = "Akira class";
        _akira.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Akira/Animation"];
        _akira.Object = "AkiraClass";
        _akira.TitaniumCost = 3055;
        _akira.DilithiumCost = 500;
        _akira.CrewCost = 500;
        _akira.ConstructionTime = 120;
        _akira.MaxConstructionTime = 120;
        _akira.NameIndex = -1;
        _akira.IndexList = GameManager.instance.NamesIndexes.AkiraIndexes;
        _akira.MaxIndexCount = NameCounter.Akira.Names.Count;
        
        ConstructionContract _prometheus = new ConstructionContract();
        _prometheus.Icon = DataLoader.Instance.ResourcesCache["Prometheus/Icon"] as Sprite;
        _prometheus.ObjectName = "Prometheus class";
        _prometheus.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Prometheus/Animation"];
        _prometheus.Object = "PrometheusClass";
        _prometheus.TitaniumCost = 2100;
        _prometheus.DilithiumCost = 100;
        _prometheus.CrewCost = 175;
        _prometheus.ConstructionTime = 130;
        _prometheus.MaxConstructionTime = 130;
        _prometheus.NameIndex = -1;
        _prometheus.IndexList = GameManager.instance.NamesIndexes.PrometheusIndexes;
        _prometheus.MaxIndexCount = NameCounter.Prometheuse.Names.Count;
        
        ConstructionContract _nebula = new ConstructionContract();
        _nebula.Icon = DataLoader.Instance.ResourcesCache["Nebula/Icon"] as Sprite;
        _nebula.ObjectName = "Nebula class";
        _nebula.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Nebula/Animation"];
        _nebula.Object = "NebulaClass";
        _nebula.TitaniumCost = 3309;
        _nebula.DilithiumCost = 900;
        _nebula.CrewCost = 750;
        _nebula.ConstructionTime = 150;
        _nebula.MaxConstructionTime = 150;
        _nebula.NameIndex = -1;
        _nebula.IndexList = GameManager.instance.NamesIndexes.NebulaIndexes;
        _nebula.MaxIndexCount = NameCounter.Nebula.Names.Count;
        
        ConstructionContract _galaxy = new ConstructionContract();
        _galaxy.Icon = DataLoader.Instance.ResourcesCache["Galaxy/Icon"] as Sprite;
        _galaxy.ObjectName = "Galaxy class";
        _galaxy.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Galaxy/Animation"];
        _galaxy.Object = "GalaxyClass";
        _galaxy.TitaniumCost = 5000;
        _galaxy.DilithiumCost = 1000;
        _galaxy.CrewCost = 1000;
        _galaxy.ConstructionTime = 160;
        _galaxy.MaxConstructionTime = 160;
        _galaxy.NameIndex = -1;
        _galaxy.IndexList = GameManager.instance.NamesIndexes.GalaxyIndexes;
        _galaxy.MaxIndexCount = NameCounter.Galaxy.Names.Count;
        
        ConstructionContract _sovereign = new ConstructionContract();
        _sovereign.Icon = DataLoader.Instance.ResourcesCache["Sovereign/Icon"] as Sprite;
        _sovereign.ObjectName = "Sovereign class";
        _sovereign.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Sovereign/Animation"];
        _sovereign.Object = "SovereignClass";
        _sovereign.TitaniumCost = 3205;
        _sovereign.DilithiumCost = 500;
        _sovereign.CrewCost = 855;
        _sovereign.ConstructionTime = 180;
        _sovereign.MaxConstructionTime = 180;
        _sovereign.NameIndex = -1;
        _sovereign.IndexList = GameManager.instance.NamesIndexes.SovereignIndexes;
        _sovereign.MaxIndexCount = NameCounter.Sovereign.Names.Count;
        
        ConstructionContract _excalibur = new ConstructionContract();
        _excalibur.Icon = DataLoader.Instance.ResourcesCache["Excalibur/Icon"] as Sprite;
        _excalibur.ObjectName = "Excalibur class";
        _excalibur.Animation = (GameObject) DataLoader.Instance.ResourcesCache["Excalibur/Animation"];
        _excalibur.Object = "ExcaliburClass";
        _excalibur.TitaniumCost = 10000;
        _excalibur.DilithiumCost = 1000;
        _excalibur.CrewCost = 2000;
        _excalibur.ConstructionTime = 200;
        _excalibur.MaxConstructionTime = 200;
        _excalibur.NameIndex = -1;
        _excalibur.IndexList = GameManager.instance.NamesIndexes.ExcaliburIndexes;
        _excalibur.MaxIndexCount = NameCounter.Excalibur.Names.Count;

        _dh1.AbleToConstruct.Add(_akira);
        _dh1.AbleToConstruct.Add(_prometheus);
        _dh1.AbleToConstruct.Add(_nebula);
        _dh1.AbleToConstruct.Add(_galaxy);
        _dh1.AbleToConstruct.Add(_sovereign);
        _dh1.AbleToConstruct.Add(_excalibur);

        DockingHub[] _dh = new DockingHub[1] {_dh1};

        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(200, this);
        WarpCoreSS _wc = (WarpCoreSS) subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(400, this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(450, this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(360, this);
        StationDockingHubSS _sdh = (StationDockingHubSS) subModulesObj.AddComponent<StationDockingHubSS>()
            .SetHubPurpose(_dh, new Vector3(-60, 0, 0), Vector3.left, this);
        
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

        initShilds(1, ShildsObj, _hs, 1000, 180, 100);

        FindInmodelElements();

        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 4000000;
        
        DilithiumCost = 400;
        TitaniumCost = 500;
        CrewCost = 450;

        canBeDeassembled = true;
        DeassembledAnim = DataLoader.Instance.ResourcesCache["FederationAdvancedDrydock/Animation"] as GameObject;

        DeassebleTime = 180;

        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.ConstructionStation;
    }
}