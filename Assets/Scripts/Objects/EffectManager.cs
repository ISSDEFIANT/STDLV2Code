using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public SelectableObject owner;
    
    public List<ObjectEffect> effects = new List<ObjectEffect>();

    public PrimaryWeaponSS primaryWeapon;
    public SecondaryWeaponSS secondaryWeapon;
    public ImpulsEngineSS impulsEngine;
    public WarpEngineSS warpEngine;
    public WarpCoreSS warpCore;
    public LifeSupportSS lifeSupport;
    public SensorSS sensor;
    public TractorBeamSS tractorBeam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (effects.Count > 0)
        {
            foreach (ObjectEffect ef in effects)
            {
                if (ef.LifeTime > 0)
                {
                    ef.LifeTime -= Time.deltaTime;
                }
                else
                {
                    effects.Remove(ef);
                    Destroy(ef);
                    return;
                }
            }
        }
    }

    public void AddEffect(ZoneEffect.Effects Effect, Vector3 sourcePosition)
    {
        switch (Effect)
        {
            case ZoneEffect.Effects.StarRadiation:
                if (!gameObject.GetComponent<StarEffect>())
                {
                    ObjectEffect newEffect = gameObject.AddComponent<StarEffect>();
                    newEffect.SoucePosition = sourcePosition;
                    effects.Add(newEffect);
                }
                break;
        }
    }

    public float SensorRange()
    {
        float curRange = sensor.radius;
        switch (owner.Alerts)
        {
            case STMethods.Alerts.GreenAlert:
                curRange = sensor.radius;
                break;
            case STMethods.Alerts.YellowAlert:
                curRange *= 0.75f;
                break;
            case STMethods.Alerts.RedAlert:
                curRange *= 0.5f;
                break;
        }
        if (sensor.efficiency < 0.5f && sensor.efficiency > 0.2)
        {
            curRange *= 0.5f;
        }
        else if (sensor.efficiency < 0.2)
        {
            curRange = owner.ObjectRadius+1;
            return curRange;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].SensoresAffect)
                {
                    if (effects[i].SensoresDisable)
                    {
                        curRange = owner.ObjectRadius+1;
                        return curRange;
                    }
                    else
                    {
                        curRange *= effects[i].SensoresMultiplyer;
                    }
                }
            }
        }
        return curRange;
    }
    
    public float PrimaryWeaponDamage(float damage)
    {
        float curDamage = damage;
        switch (owner.Alerts)
        {
            case STMethods.Alerts.GreenAlert:
                curDamage = 0;
                return curDamage;
            case STMethods.Alerts.YellowAlert:
                curDamage = 0;
                return curDamage;
            case STMethods.Alerts.RedAlert:
                curDamage = damage;
                break;
        }
        if (primaryWeapon.efficiency < 0.5f && primaryWeapon.efficiency > 0.2)
        {
            curDamage *= 0.5f;
        }
        else if (primaryWeapon.efficiency < 0.2)
        {
            curDamage = 0;
            return curDamage;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].PrimaryWeaponAffect)
                {
                    if (effects[i].PrimaryWeaponDisable)
                    {
                        curDamage = 0;
                        return curDamage;
                    }
                    else
                    {
                        curDamage *= effects[i].PrimaryWeaponDamageMultiplyer;
                    }
                }
            }
        }
        return curDamage;
    }
    
    public float PrimaryWeaponFireReload(float reload)
    {
        float curReload = reload;
        switch (owner.Alerts)
        {
            case STMethods.Alerts.GreenAlert:
                curReload = 0;
                return curReload;
            case STMethods.Alerts.YellowAlert:
                curReload = 0;
                return curReload;
            case STMethods.Alerts.RedAlert:
                curReload = reload;
                break;
        }
        if (primaryWeapon.efficiency < 0.5f && primaryWeapon.efficiency > 0.2)
        {
            curReload *= 2;
        }
        else if (primaryWeapon.efficiency < 0.2)
        {
            curReload = 0;
            return curReload;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].PrimaryWeaponAffect)
                {
                    if (effects[i].PrimaryWeaponDisable)
                    {
                        curReload = 0;
                        return curReload;
                    }
                    else
                    {
                        curReload *= effects[i].PrimaryWeaponFireReloadMultiplyer;
                    }
                }
            }
        }
        return curReload;
    }
    
    public float SecondaryWeaponFireReload(float reload)
    {
        float curReload = reload;
        switch (owner.Alerts)
        {
            case STMethods.Alerts.GreenAlert:
                curReload = 0;
                return curReload;
            case STMethods.Alerts.YellowAlert:
                curReload = 0;
                return curReload;
            case STMethods.Alerts.RedAlert:
                curReload = reload;
                break;
        }
        if (secondaryWeapon.efficiency < 0.5f && secondaryWeapon.efficiency > 0.2)
        {
            curReload *= 2;
        }
        else if (secondaryWeapon.efficiency < 0.2)
        {
            curReload = 0;
            return curReload;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].SecondaryWeaponAffect)
                {
                    if (effects[i].SecondaryWeaponDisable)
                    {
                        curReload = 0;
                        return curReload;
                    }
                    else
                    {
                        curReload *= effects[i].SecondaryWeaponFireReloadMultiplyer;
                    }
                }
            }
        }
        return curReload;
    }
    
    public float ImpulseEngineSpeed()
    {
        float curSpeed = owner.captain.Pilot.engines.MaxSpeed;
        if (owner.destroyed)
        {
            curSpeed = 0;
            return curSpeed;
        }
        switch (owner.Alerts)
        {
            case STMethods.Alerts.GreenAlert:
                curSpeed = owner.captain.Pilot.engines.MaxSpeed;
                break;
            case STMethods.Alerts.YellowAlert:
                curSpeed *= 0.9f;
                break;
            case STMethods.Alerts.RedAlert:
                curSpeed *= 0.75f;
                break;
        }
        if (impulsEngine.efficiency < 0.5f && impulsEngine.efficiency > 0.2)
        {
            curSpeed *= 0.5f;
        }
        else if (impulsEngine.efficiency < 0.2)
        {
            curSpeed = 0;
            return curSpeed;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].ImpulseEngineAffect)
                {
                    if (effects[i].ImpulseEngineDisable)
                    {
                        curSpeed = 0;
                        return curSpeed;
                    }
                    else
                    {
                        curSpeed *= effects[i].ImpulseEngineSpeedMultiplyer;
                    }
                }
            }
        }
        return curSpeed;
    }
    
    public float WarpEngineSpeed()
    {
        float curSpeed = owner.captain.Pilot.engines.MaxSpeed;
        if (owner.destroyed)
        {
            curSpeed = 0;
            return curSpeed;
        }
        switch (owner.Alerts)
        {
            case STMethods.Alerts.GreenAlert:
                curSpeed = owner.captain.Pilot.engines.MaxSpeed;
                break;
            case STMethods.Alerts.YellowAlert:
                curSpeed *= 0.9f;
                break;
            case STMethods.Alerts.RedAlert:
                curSpeed *= 0.75f;
                break;
        }
        if (warpEngine.efficiency < 0.5f && warpEngine.efficiency > 0.2)
        {
            curSpeed *= 0.5f;
        }
        else if (warpEngine.efficiency < 0.2)
        {
            curSpeed = 0;
            return curSpeed;
        }
        if (warpCore.efficiency < 0.5f && warpCore.efficiency > 0.2)
        {
            curSpeed *= 0.8f;
        }
        else if (warpCore.efficiency < 0.5f && warpCore.efficiency > 0.2)
        {
            curSpeed *= 0.75f;
        }
        else if (warpCore.efficiency < 0.2)
        {
            curSpeed = 0;
            return curSpeed;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].WarpEngineAffect)
                {
                    if (effects[i].WarpEngineDisable)
                    {
                        curSpeed = 0;
                        return curSpeed;
                    }
                    else
                    {
                        curSpeed *= effects[i].WarpEngineSpeedMultiplyer;
                    }
                }
                if (effects[i].WarpCoreAffect)
                {
                    if (effects[i].WarpCoreDisable)
                    {
                        curSpeed = 0;
                        return curSpeed;
                    }
                }
            }
        }
        return curSpeed;
    }
    
    public float UpdateCrew(float crew, float maxCrew, SubSystem ow)
    {
        float deltaCrew = crew;
        if (ow.efficiency < 0.2)
        {
            if (maxCrew / 90 > crew)
            {
                deltaCrew -= crew / 100 / owner.ObjectRadius;
            }
            else
            {
                deltaCrew -= crew / 25 / owner.ObjectRadius;
            }
            return deltaCrew;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].LifeSupportAffect)
                {
                    if (effects[i].LifeSupportDisable)
                    {
                        if (maxCrew / 90 > crew)
                        {
                            deltaCrew -= crew / 100 / owner.ObjectRadius;
                        }
                        else
                        {
                            deltaCrew -= crew / 25 / owner.ObjectRadius;
                        }
                        return deltaCrew;
                    }
                    else
                    {
                        deltaCrew *= effects[i].LifeSupportCrewMultiplyer;
                    }
                }
            }
        }
        return deltaCrew;
    }

    public bool PrimaryWeaponEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].PrimaryWeaponAffect)
                {
                    if (effects[i].PrimaryWeaponDisable) active = false;
                }
            }
        }

        return active;
    }
    
    public bool SecondaryWeaponEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].SecondaryWeaponAffect)
                {
                    if (effects[i].SecondaryWeaponDisable) active = false;
                }
            }
        }

        return active;
    }
    
    public bool ImpulseEngineEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].ImpulseEngineAffect)
                {
                    if (effects[i].ImpulseEngineDisable) active = false;
                }
            }
        }

        return active;
    }
    
    public bool WarpEngineEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].WarpEngineAffect)
                {
                    if (effects[i].WarpEngineDisable) active = false;
                }
            }
        }

        return active;
    }
    
    public bool WarpCoreEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].WarpCoreAffect)
                {
                    if (effects[i].WarpCoreDisable) active = false;
                }
            }
        }

        return active;
    }
    
    public bool LifeSupportEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].LifeSupportAffect)
                {
                    if (effects[i].LifeSupportDisable) active = false;
                }
            }
        }

        return active;
    }
    
    public bool SensoresEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].SensoresAffect)
                {
                    if (effects[i].SensoresDisable) active = false;
                }
            }
        }

        return active;
    }
    
    public bool TractorBeamEffectActivity()
    {
        bool active = true;
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].TractorBeamAffect)
                {
                    if (effects[i].TractorBeamDisable) active = false;
                }
            }
        }

        return active;
    }
    
    
    public float getEnergyRegeneration()
    {
        float curEnergyRegen = owner._hs.maxEnergy/owner.ObjectRadius/4;
        switch (owner.Alerts)
        {
            case STMethods.Alerts.GreenAlert:
                curEnergyRegen = owner._hs.maxEnergy/owner.ObjectRadius;
                break;
            case STMethods.Alerts.YellowAlert:
                curEnergyRegen *= 0.75f;
                break;
            case STMethods.Alerts.RedAlert:
                curEnergyRegen *= 0.5f;
                break;
        }
        if (warpCore.efficiency < 0.5f && warpCore.efficiency > 0.2)
        {
            curEnergyRegen *= 0.75f;
        }
        else if (warpCore.efficiency < 0.5f && warpCore.efficiency > 0.2)
        {
            curEnergyRegen *= 0.5f;
        }
        else if (warpCore.efficiency < 0.2)
        {
            curEnergyRegen = 0;
            return curEnergyRegen;
        }
        if (effects.Count > 0)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].EnergyRegenerationAffect)
                {
                    curEnergyRegen *= effects[i].EnergyRegenerationMultiplyer;
                }
            }
        }
        return curEnergyRegen;
    }
}