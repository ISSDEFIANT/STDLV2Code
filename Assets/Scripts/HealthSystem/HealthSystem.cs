using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float MaxHull;
    public float curHull;

    public float MaxCrew;
    public float curCrew;

    public SubSystem[] Shilds;

    public SubSystem[] SubSystems;

    protected float Timer = 3;

    public GameObject ExplosionEffect;

    private GameObject ShieldEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        Timer = Random.Range(3, 7);
        if (Shilds.Length > 0)
        {
            foreach (SubSystem _sh in Shilds)
            {
                _sh.healthSystem = this;
            }

            ShieldEffect = gameObject.GetComponentInChildren<Forcefield>().gameObject;
        }

        if (SubSystems.Length > 0)
        {
            foreach (SubSystem _ss in SubSystems)
            {
                _ss.healthSystem = this;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (curHull <= 0)
        {
            if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                DestroyObject();
            }
        }
    }

    public void ApplyDamage(float damage, STMethods.AttackType aiming, Vector3 attackVector)
    {
        if (Shilds.Length > 0)
        {
            for (int i = Shilds.Length - 1; i >= 0;)
            {
                if (Shilds[i].SubSystemCurHealth > 0)
                {
                    ShieldEffect.GetComponent<Renderer>().enabled = true;
                    ShieldEffect.GetComponent<Forcefield>().Shot = true;
                    ShieldEffect.GetComponent<Forcefield>().AttackVector = attackVector;
                    
                    Shilds[i].SubSystemCurHealth -= Time.deltaTime*damage;
                    break;
                }
                else
                {
                    if (i > 0)
                    {
                        i--;
                    }
                    else
                    {
                        if (curHull-Time.deltaTime*damage > 0)
                        {
                            ApplyHullAndSubSystemDamage(damage,aiming);
                            break;
                        }
                        else
                        {
                            curHull = 0;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            if (curHull-Time.deltaTime*damage > 0)
            {
                curHull -= Time.deltaTime*damage;
            }
            else
            {
                curHull = 0;
            }
        }
    }

    public void DestroyObject()
    {
        Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void CrewDamage(float damage)
    {
        if (curCrew-damage > 0)
        {
            curCrew -= damage;
        }
        else
        {
            curCrew = 0;
        }
    }

    public void InitHullAndCrew(float Hull, int Crew)
    {
        MaxHull = Hull;
        curHull = Hull;
        MaxCrew = Crew;
        curCrew = Crew;
    }

    void ApplyHullAndSubSystemDamage(float damage, STMethods.AttackType aiming)
    {
        switch (aiming)
        {
            case STMethods.AttackType.NormalAttack:
                curHull -= Time.deltaTime*damage;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 5);
                }
                break;
            case STMethods.AttackType.PrimaryWeaponSystemAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is PrimaryWeaponSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
            case STMethods.AttackType.SecondaryWeaponSystemAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is SecondaryWeaponSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
            case STMethods.AttackType.ImpulseSystemAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is ImpulsEngineSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
            case STMethods.AttackType.WarpEngingSystemAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is WarpEngineSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
            case STMethods.AttackType.WarpCoreAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is WarpCoreSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
            case STMethods.AttackType.LifeSupportSystemAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is LifeSupportSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
            case STMethods.AttackType.SensorsSystemAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is SensorSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
            case STMethods.AttackType.TractorBeamSystemAttack:
                curHull -= Time.deltaTime*damage/5;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage / 10);
                    if (_ss is TractorBeamSS)
                    {
                        _ss.SubSystemCurHealth -= Time.deltaTime*damage/2;
                    }
                }
                break;
        }
    }
}