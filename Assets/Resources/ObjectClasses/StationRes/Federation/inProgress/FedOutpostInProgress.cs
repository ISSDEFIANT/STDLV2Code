using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedOutpostInProgress : ObjectUnderConstruction
{
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Outpost";
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationOutpost/Icon"] as Sprite;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationOutpost/InProgress"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 60;
        SensorRange = 0;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(25000, 0);
        _hs.curHull = 50;

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 4;
        
        FindInmodelElements();
        
        modelEffects.Deactivate = true;
        
        rigitBody.mass = 1;
        rigitBody.isKinematic = true;

        _ucm = GetComponentInChildren<UnderConstructionManager>();
    }
}