using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedOutpost : StationType1
{
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Outpost";
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationOutpost/Icon"] as Sprite;

        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationOutpost"] as GameObject, transform.position, init);

        model.transform.parent = transform;
        
        ObjectRadius = 60;
        SensorRange = 600;

        WeaponRange = 500;
        MaxAttackTargetCount = 4;
        
        _hs.InitHullAndCrew(25000, 2000);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 4;
        
        DockingHub _dh1 = new DockingHub();
        _dh1.ShipBuilding = true;
        _dh1.EnterPoint = new Vector3(0, 3.43f, -13.16f);
        _dh1.StayPoint = new Vector3(0, 3.43f, -13.16f);
        _dh1.ExitPoint = new Vector3(0, 3.43f, -32);
        _dh1.AbleToConstruct = new List<ConstructionContract>();
        
        DockingHub _dh2 = new DockingHub();
        _dh2.ShipFixing = true;
        _dh2.EnterPoint = new Vector3(32, 5, -19);
        _dh2.StayPoint = new Vector3(32, 5, -19);
        _dh2.ExitPoint = new Vector3(32, 5, -19);
        
        DockingHub _dh3 = new DockingHub();
        _dh3.ShipFixing = true;
        _dh3.EnterPoint = new Vector3(-32, 5, -19);
        _dh3.StayPoint = new Vector3(-32, 5, -19);
        _dh3.ExitPoint = new Vector3(-32, 5, -19);
        
        DockingHub _dh4 = new DockingHub();
        _dh4.ShipFixing = true;
        _dh4.EnterPoint = new Vector3(0, 5, 37);
        _dh4.StayPoint = new Vector3(0, 5, 37);
        _dh4.ExitPoint = new Vector3(0, 5, 37);
        
        DockingHub _dh5 = new DockingHub();
        _dh5.ShipFixing = true;
        _dh5.EnterPoint = new Vector3(18, -15, 10);
        _dh5.StayPoint = new Vector3(18, -15, 10);
        _dh5.ExitPoint = new Vector3(18, -15, 10);
        
        DockingHub _dh6 = new DockingHub();
        _dh6.ShipFixing = true;
        _dh6.EnterPoint = new Vector3(-18, -15, 10);
        _dh6.StayPoint = new Vector3(-18, -15, 10);
        _dh6.ExitPoint = new Vector3(-18, -15, 10);
        
        DockingHub _dh7 = new DockingHub();
        _dh7.ShipFixing = true;
        _dh7.EnterPoint = new Vector3(0, -15, -21);
        _dh7.StayPoint = new Vector3(0, -15, -21);
        _dh7.ExitPoint = new Vector3(0, -15, -21);

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
        
        _dh1.AbleToConstruct.Add(_defian);
        
        DockingHub[] _dh = new DockingHub[7] {_dh1,_dh2,_dh3,_dh4,_dh5,_dh6,_dh7};
        
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(3000, this);
        WarpCoreSS _wc = (WarpCoreSS) subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(5000, this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(4000, this);
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(3000,this);
        SubSystem _sw = subModulesObj.AddComponent<SecondaryWeaponSS>().InitSystemHealth(4000,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(6000, this);
        StationDockingHubSS _sdh = (StationDockingHubSS) subModulesObj.AddComponent<StationDockingHubSS>().SetHubPurpose(_dh, new Vector3(60, 0, -46),Vector3.left, this);
        
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

        inModelPoints.effects[5].controllingSubsystem = _pw;
        _pw.effects = inModelPoints.effects[5];
        
        inModelPoints.effects[6].controllingSubsystem = _sw;
        _sw.effects = inModelPoints.effects[6];
        
        _hs.SubSystems = new SubSystem[7] {_ie, _wc, _ls, _pw, _sw, _ss, _sdh};
        
        _wc.WarpCoreExplosion = (GameObject) DataLoader.Instance.ResourcesCache["FederationCoreDestruction"];
        
        initShilds(1, ShildsObj, _hs, 2000, 180, 100);

        FindInmodelElements();

        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 250000000;
        
        DilithiumCost = 7000;
        TitaniumCost = 2500;
        CrewCost = 2000;

        canBeDeassembled = true;
        DeassembledAnim = (GameObject) DataLoader.Instance.ResourcesCache["FederationOutpost/Animation"];

        DeassebleTime = 200;
        
        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.DefenceStation;
    }
}
