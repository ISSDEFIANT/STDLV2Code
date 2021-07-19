using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FedDrydockInProgress : ObjectUnderConstruction
{
    public override void Awake()
    {
        base.Awake();
        
        ObjectClass = "Drydock";
        ObjectIcon = DataLoader.Instance.ResourcesCache["FederationDrydock/Icon"] as Sprite;
        
        Quaternion init = this.transform.rotation;
        
        model = Instantiate(DataLoader.Instance.ResourcesCache["FederationDrydock/InProgress"] as GameObject, transform.position, init);

        model.transform.parent = transform;

        ObjectRadius = 15;
        SensorRange = 0;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(300, 0);
        _hs.curHull = 50;

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["BigShipExplosion"];
        _hs.ExplosionEffectScale = 1.5f;
        
        FindInmodelElements();
        
        modelEffects.Deactivate = true;
        
        rigitBody.mass = 1;
        rigitBody.isKinematic = true;

        _ucm = GetComponentInChildren<UnderConstructionManager>();
    }
}
