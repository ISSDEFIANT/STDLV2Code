﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedSciStationInProgress : ObjectUnderConstruction
{
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Scientific station";
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationSciStation/Icon"] as Sprite;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationSciStation/InProgress"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 30;
        SensorRange = 0;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(900, 0);
        _hs.curHull = 50;

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 2;
        
        FindInmodelElements();
        
        modelEffects.Deactivate = true;
        
        rigitBody.mass = 1;
        rigitBody.isKinematic = true;

        _ucm = GetComponentInChildren<UnderConstructionManager>();
    }
}
