using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumDilithiumRock : ResourceSource
{
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        Quaternion init = this.transform.rotation;
        
        model = (GameObject)Instantiate(Resources.Load("Models/World/DilithiumRock/Medium/MediumDilithiumRockPre"), transform.position, init);

        model.transform.parent = transform;
        
        FindInmodelElements();

        modelEffects.Deactivate = true;

        ObjectRadius = 3;
        SensorRange = 0;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(50, 0);

        _hs.ExplosionEffect = (GameObject)Resources.Load("Effects/DamageAndDestructions/Explosions/NormallShipExplosion");
        _hs.ExplosionEffectScale = 1;
        
        rigitBody.mass = 4000;

        curResources = 1000;
        type = STMethods.ResourcesType.Dilithium;
    }
}
