using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedMiningStation : StationType1
{
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Mining station";
        
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationMiningStation/Icon"] as Sprite;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationMiningStation"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 60;
        SensorRange = 400;
        
        WeaponRange = 300;
        MaxAttackTargetCount = 4;
        
        _hs.InitHullAndCrew(1000, 2500);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 3;
        
        DockingHub _dh1 = new DockingHub();
        _dh1.ResourceUnloading = true;
        _dh1.EnterPoint = new Vector3(70, 4, 0);
        _dh1.StayPoint = new Vector3(0, 4, 0);
        _dh1.ExitPoint = new Vector3(-70, 4, 0);
        
        DockingHub _dh2 = new DockingHub();
        _dh2.ResourceUnloading = true;
        _dh2.EnterPoint = new Vector3(0, -6, -70);
        _dh2.StayPoint = new Vector3(0, -6, 0);
        _dh2.ExitPoint = new Vector3(0, -6, 70);

        DockingHub[] _dh = new DockingHub[2] {_dh1, _dh2};
        
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(400, this);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(400,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(450,this);
        SubSystem _pw = subModulesObj.AddComponent<PrimaryWeaponSS>().InitSystemHealth(400, this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(360,this);
        StationDockingHubSS _sdh = (StationDockingHubSS)subModulesObj.AddComponent<StationDockingHubSS>().SetHubPurpose(_dh, new Vector3(-80, 0, -80), Vector3.left, this);


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
        
        _hs.SubSystems = new SubSystem[6]{_ie, _wc,_ls,_ss,_pw,_sdh};
        _wc.WarpCoreExplosion = (GameObject) DataLoader.Instance.ResourcesCache["FederationCoreDestruction"];
        
        initShilds(1,ShildsObj,_hs,500,180,100);

        FindInmodelElements();
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 1000000000;
        
        DilithiumCost = 700;
        TitaniumCost = 1000;
        CrewCost = 2500;

        canBeDeassembled = true;
        DeassembledAnim = DataLoader.Instance.ResourcesCache["FederationMiningStation/Animation"] as GameObject;

        DeassebleTime = 60;
        
        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.MiningStation;
    }
}
