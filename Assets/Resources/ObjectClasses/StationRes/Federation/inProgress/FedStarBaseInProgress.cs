using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedStarBaseInProgress : ObjectUnderConstruction
{
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Starbase";
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationStarbase/Icon"] as Sprite;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationStarbase/InProgress"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 120;
        SensorRange = 0;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(100000, 0);
        _hs.curHull = 50;

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 5;
        
        FindInmodelElements();
        
        modelEffects.Deactivate = true;
        
        rigitBody.mass = 1;
        rigitBody.isKinematic = true;

        _ucm = GetComponentInChildren<UnderConstructionManager>();
    }
}