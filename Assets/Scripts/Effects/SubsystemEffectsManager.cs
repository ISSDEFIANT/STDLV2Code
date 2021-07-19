using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubsystemEffectsManager : MonoBehaviour
{
    public EffectManagerStructure[] effects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (EffectManagerStructure effect in effects)
        {
            if (effect.effectsEnable && effect.controllingSubsystem != null)
            {
                if (effect.controllingSubsystem.SubSystemCurHealth <= effect.controllingSubsystem.SubSystemMaxHealth*0.2f)
                {
                    bool isEnabled = false;
                    foreach (EMSDamageEffectsSybStructure dam in effect.DamageEffects)
                    {
                        if (dam.borgEnable)
                        {
                            if (effect.controllingSubsystem.Owner.Assimilated)
                            {
                                if (dam.BorgDamageEffect.activeSelf) isEnabled = true;
                            }
                            else
                            {
                                if (dam.NormalDamageEffect.activeSelf) isEnabled = true;
                            }
                        }
                        else
                        {
                            if (dam.NormalDamageEffect.activeSelf) isEnabled = true;
                        }
                    }

                    if (!isEnabled)
                    {
                        foreach (EMSDamageEffectsSybStructure dam in effect.DamageEffects)
                        {
                            float active = Random.Range(0, 100);
                            if (active > 25)
                            {
                                if (dam.borgEnable)
                                {
                                    if (effect.controllingSubsystem.Owner.Assimilated)
                                    {
                                        dam.BorgDamageEffect.SetActive(true);
                                        if (dam.randomeLocation)
                                            dam.BorgDamageEffect.transform.position = dam.locationPoints[Random.Range(0, dam.locationPoints.Length)].transform.position;
                                        if (dam.randomeRotation)
                                            dam.BorgDamageEffect.transform.rotation = Random.rotation;
                                    }
                                    else
                                    {
                                        dam.NormalDamageEffect.SetActive(true);
                                        if (dam.randomeLocation)
                                            dam.NormalDamageEffect.transform.position = dam.locationPoints[Random.Range(0, dam.locationPoints.Length)].transform.position;
                                        if (dam.randomeRotation)
                                            dam.NormalDamageEffect.transform.rotation = Random.rotation;
                                    }
                                }
                                else
                                {
                                    dam.NormalDamageEffect.SetActive(true);
                                    if (dam.randomeLocation)
                                        dam.NormalDamageEffect.transform.position = dam.locationPoints[Random.Range(0, dam.locationPoints.Length)].transform.position;
                                    if (dam.randomeRotation)
                                        dam.NormalDamageEffect.transform.rotation = Random.rotation;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (EMSDamageEffectsSybStructure dam in effect.DamageEffects)
                    {
                        if (dam.borgEnable)
                        {
                            dam.BorgDamageEffect.SetActive(false);
                            dam.NormalDamageEffect.SetActive(false);
                        }
                        else
                        {
                            dam.NormalDamageEffect.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
[System.Serializable]
public class EffectManagerStructure
{
    public string subSystemName;
    public bool effectsEnable;
    public SubSystem controllingSubsystem;
    public Transform[] AimingPoints;
    public EMSDamageEffectsSybStructure[] DamageEffects;
}

[System.Serializable]
public class EMSDamageEffectsSybStructure
{
    public bool borgEnable;
    public GameObject NormalDamageEffect;
    public GameObject BorgDamageEffect;
    
    public bool randomeLocation;
    public GameObject[] locationPoints;
    public bool randomeRotation;
}