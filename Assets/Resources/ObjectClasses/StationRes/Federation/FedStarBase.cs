using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedStarBase : StationType1
{
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Starbase";
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationStarbase/Icon"] as Sprite;

        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationStarbase"] as GameObject, transform.position, init);

        model.transform.parent = transform;
        
        ObjectRadius = 120;
        SensorRange = 1000;

        WeaponRange = 800;
        MaxAttackTargetCount = 4;
        
        _hs.InitHullAndCrew(100000, 20000);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 5;
        _hs.reCrewMultiplyer = 100f;
        
        DockingHub _dh1 = new DockingHub();
        _dh1.ShipBuilding = true;
        _dh1.EnterPoint = new Vector3(150, 0, 0);
        _dh1.StayPoint = new Vector3(30, 0, 0);
        _dh1.ExitPoint = new Vector3(150, 0, 0);
        _dh1.AbleToConstruct = new List<ConstructionContract>();
        
        DockingHub _dh2 = new DockingHub();
        _dh2.ShipFixing = true;
        _dh2.EnterPoint = new Vector3(-150, 0, 0);
        _dh2.StayPoint = new Vector3(-30, 0, 0);
        _dh2.ExitPoint = new Vector3(-150, 0, 0);
        
        DockingHub[] _dh = new DockingHub[2] {_dh1,_dh2};
        
        UpdatingContract _uc1 = new UpdatingContract();
        _uc1.Name = "Tactical Opps center";
        _uc1.ButtonImage = DataLoader.Instance.ResourcesCache["FederationStarbase/UpgradeButton1"] as Sprite;
        _uc1.CrewCost = 1000;
        _uc1.TitaniumCost = 1000;
        _uc1.DilithiumCost = 700;
        _uc1.ConstructionTime = 60;
        _uc1.curConstructionTime = 60;
        
        UpdatingContract _uc2 = new UpdatingContract();
        _uc2.Name = "Starfleet Command";
        _uc2.ButtonImage = DataLoader.Instance.ResourcesCache["FederationStarbase/UpgradeButton2"] as Sprite;
        _uc2.CrewCost = 3000;
        _uc2.TitaniumCost = 3000;
        _uc2.DilithiumCost = 2100;
        _uc2.ConstructionTime = 120;
        _uc2.curConstructionTime = 120;
        
        ConstructionContract _atlantia = new ConstructionContract();
        _atlantia.Icon = DataLoader.Instance.ResourcesCache["Atlantia/Icon"] as Sprite;
        _atlantia.ObjectName = "Atlantia class";
        _atlantia.Animation = (GameObject)DataLoader.Instance.ResourcesCache["Atlantia/Animation"];
        _atlantia.Object = "AtlantiaClass";
        _atlantia.TitaniumCost = 280;
        _atlantia.DilithiumCost = 80;
        _atlantia.CrewCost = 100;
        _atlantia.ConstructionTime = 20;
        _atlantia.MaxConstructionTime = 20;
        _atlantia.NameIndex = -1;
        _atlantia.MaxIndexCount = NameCounter.Atlantia.Names.Count;
        
        _dh1.AbleToConstruct.Add(_atlantia);
        
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(3000, this);
        WarpCoreSS _wc = (WarpCoreSS) subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(5000, this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(4000, this);
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(3000,this);
        SubSystem _sw = subModulesObj.AddComponent<SecondaryWeaponSS>().InitSystemHealth(4000,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(6000, this);
        StationDockingHubSS _sdh = (StationDockingHubSS) subModulesObj.AddComponent<StationDockingHubSS>().SetHubPurpose(_dh, new Vector3(-150, 0, -150),Vector3.left, this);
        UpdatingCentralSS _uc = (UpdatingCentralSS) subModulesObj.AddComponent<UpdatingCentralSS>().SetLevels(new UpdatingContract[2]{_uc1,_uc2}, this);
        
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
        
        _hs.SubSystems = new SubSystem[8] {_ie, _wc, _ls, _pw, _sw, _ss, _sdh, _uc};
        
        _wc.WarpCoreExplosion = (GameObject) DataLoader.Instance.ResourcesCache["FederationCoreDestruction"];

            initShilds(1, ShildsObj, _hs, 2000, 180, 100);

        FindInmodelElements();

        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 10000000000;
        
        DilithiumCost = 7000;
        TitaniumCost = 10000;
        CrewCost = 20000;

        canBeDeassembled = true;
        DeassembledAnim = (GameObject) DataLoader.Instance.ResourcesCache["FederationStarbase/Animation"];

        DeassebleTime = 180;
        
        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.Starbase;
    }
    public override void Update()
    {
        base.Update();
        if (Constructed)
        {
            _hs.curCrew = 2000;
            Constructed = false;
        }
    }
}
