using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEffect : MonoBehaviour
{
    public SelectableObject target;
    public Vector3 SoucePosition;
    
    public string effectName;
    
    public bool SensoresAffect = false;
    public bool SensoresDisable = false;
    public float SensoresMultiplyer = 1;
    
    public bool PrimaryWeaponAffect = false;
    public bool PrimaryWeaponDisable = false;
    public float PrimaryWeaponDamageMultiplyer = 1;
    public float PrimaryWeaponFireReloadMultiplyer = 1;
    
    public bool SecondaryWeaponAffect = false;
    public bool SecondaryWeaponDisable = false;
    public float SecondaryWeaponFireReloadMultiplyer = 1;
    
    public bool ImpulseEngineAffect = false;
    public bool ImpulseEngineDisable = false;
    public float ImpulseEngineSpeedMultiplyer = 1;
    
    public bool WarpEngineAffect = false;
    public bool WarpEngineDisable = false;
    public float WarpEngineSpeedMultiplyer = 1;
    
    public bool WarpCoreAffect = false;
    public bool WarpCoreDisable = false;
    
    public bool LifeSupportAffect = false;
    public bool LifeSupportDisable = false;
    public float LifeSupportCrewMultiplyer = 1;
    
    public bool TractorBeamAffect = false;
    public bool TractorBeamDisable = false;
    
    public bool EnergyRegenerationAffect = false;
    public float EnergyRegenerationMultiplyer = 1;

    public float LifeTime = 0;

    public void Awake()
    {
        target = gameObject.GetComponent<SelectableObject>();
    }

    public virtual void Update()
    {
        
    }
}