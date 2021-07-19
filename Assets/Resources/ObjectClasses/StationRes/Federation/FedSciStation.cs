using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedSciStation : StationType1
{
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Scientific station";
        
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationSciStation/Icon"] as Sprite;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationSciStation"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 30;
        SensorRange = 300;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(900, 450);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 2;
        
        SubSystem _ie = subModulesObj.AddComponent<ImpulsEngineSS>().InitSystemHealth(400, this);
        WarpCoreSS _wc = (WarpCoreSS)subModulesObj.AddComponent<WarpCoreSS>().InitSystemHealth(400,this);
        SubSystem _ls = subModulesObj.AddComponent<LifeSupportSS>().InitSystemHealth(450,this);
        SubSystem _ss = subModulesObj.AddComponent<SensorSS>().InitSystemHealth(360,this);
        SciLabsSS _sl = (SciLabsSS)subModulesObj.AddComponent<SciLabsSS>().SetStationRace(STMethods.Races.Federation,this);
        
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
        
        _hs.SubSystems = new SubSystem[5]{_ie, _wc,_ls,_ss,_sl};
        _wc.WarpCoreExplosion = (GameObject) DataLoader.Instance.ResourcesCache["FederationCoreDestruction"];
        
        initShilds(1,ShildsObj,_hs,1000,180,100);

        FindInmodelElements();
        
        Captain = gameObject.AddComponent<Captain>();
        Captain.Owner = this;
        Captain.Sensors = _ss as SensorSS;

        rigitBody.mass = 50000000;
        
        DilithiumCost = 500;
        TitaniumCost = 900;
        CrewCost = 450;

        canBeDeassembled = true;
        DeassembledAnim = DataLoader.Instance.ResourcesCache["FederationSciStation/Animation"] as GameObject;

        DeassebleTime = 200;
        
        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.SciStation;
    }
}
