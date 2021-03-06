using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    /// <summary> ��������. </summary>
    public SelectableObject Owner;

    /// <summary> ������������ ����������� �������. </summary>
    public float MaxHull;

    /// <summary> ������� ����������� �������. </summary>
    public float curHull;

    /// <summary> ������������ ������. </summary>
    public float MaxCrew;

    /// <summary> ������� ������. </summary>
    public float curCrew;

    /// <summary> ����. </summary>
    public SubSystem[] Shilds;

    /// <summary> ����������. </summary>
    public SubSystem[] SubSystems;

    /// <summary> ����� �������� �� �������. </summary>
    public Transform[] HullPoints = null;
    
    /// <summary> ������������ �������������� �������. </summary>
    public float maxEnergy;

    /// <summary> ������� �������������� �������. </summary>
    public float curEnergy;

    /// <summary> ������ �� ���������� ������� �� ������. </summary>
    public float Timer = 3;

    /// <summary> �����. </summary>
    public GameObject ExplosionEffect;

    /// <summary> ������ ������. </summary>
    public float ExplosionEffectScale = 1f;

    /// <summary> ������� �����. </summary>
    private GameObject ShieldEffect;
    
    [HideInInspector]
    public float SelfDestructTimer = 5;
    [HideInInspector]
    public bool SelfDestructActive;

    private List<Light> lights;
    
    /// <summary> Атакован. </summary>
    public bool isAttacked;
    /// <summary> Время через которое открывается возможность пополнить экипах после атаки. </summary>
    private float recrewCooldown = 5f;
    /// <summary> Производится ли пополнение. </summary>
    public bool isRecrewing;
    /// <summary> Множитель пополнения. </summary>
    public float reCrewMultiplyer = 10f;

    /// <summary> ��������� ��������� � �����. </summary>
    void Start()
    {
        lights = GetComponentsInChildren<Light>().ToList();
        Timer = Random.Range(5, 7);
        if (Owner is ResourceSource) Timer = 0.1f; 
        if (Shilds != null && Shilds.Length > 0)
        {
            foreach (SubSystem _sh in Shilds)
            {
                _sh.healthSystem = this;
            }

            ShieldEffect = gameObject.GetComponentInChildren<Forcefield>().gameObject;
        }

        if (SubSystems != null && SubSystems.Length > 0)
        {
            foreach (SubSystem _ss in SubSystems)
            {
                _ss.healthSystem = this;
            }
        }
    }

    /// <summary> ������ ������� � ������ ���������� �����. </summary>
    void LateUpdate()
    {
        if (Owner.destroyed)
        {
            if (SubSystems != null && SubSystems.Length > 0)
            {
                foreach (SubSystem _s in SubSystems)
                {
                    if (!(_s is WarpCoreSS))
                    {
                        _s.SubSystemCurHealth = 0;
                        _s.efficiency = 0;
                    }
                }
            }

            if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                DestroyObject();
            }
        }

        SelfDestructionEvent();
        if (maxEnergy > 0)
        {
            if (curEnergy < maxEnergy)
            {
                curEnergy += Time.deltaTime * Owner.effectManager.getEnergyRegeneration();
            }
            else
            {
                curEnergy = maxEnergy;
            }
        }

        if (MaxCrew > 0 && (int) curCrew <= 0)
        {
            if (lights == null || lights.Count <= 0) return;
            foreach (Light _l in lights)
            {
                if(_l.gameObject.activeSelf) _l.gameObject.SetActive(false);
            }
        }
        else if (MaxCrew > 0 && (int) curCrew > 0)
        {
            if (lights == null || lights.Count <= 0) return;
            foreach (Light _l in lights)
            {
                if(!_l.gameObject.activeSelf) _l.gameObject.SetActive(true);
            }
        }
        
        if (isAttacked)
        {
            recrewCooldown -= Time.deltaTime;
            if (recrewCooldown <= 0)
            {
                isAttacked = false;
                recrewCooldown = 5f;
            }
        }
        else
        {
            if (isRecrewing)
            {
                Recrew();
            }
        }
    }

    public void Recrew()
    {
        if (MaxCrew <= 0) return;
        if (curCrew < MaxCrew && GameManager.instance.Players[Owner.PlayerNum - 1].Crew > 0)
        {
            curCrew += Time.deltaTime * reCrewMultiplyer;
            GameManager.instance.Players[Owner.PlayerNum - 1].Crew -= Time.deltaTime * reCrewMultiplyer;
        }
        else
        {
            curCrew = MaxCrew;
            isRecrewing = false;
        }
    }
    
    /// <summary> ���������� ����� � �������. </summary>
    public void ApplyDamage(float damage, STMethods.AttackType aiming, Vector3 attackVector, bool ignoreShields = false, float shieldsPower = 1f, bool ingnoreDeltaTime = false)
    {
        isAttacked = true;
        recrewCooldown = 5f;
        if (ignoreShields)
        {
            ApplyHullAndSubSystemDamage(damage, aiming, ingnoreDeltaTime);
            if (MaxCrew > 0) CrewDamage(Random.Range(damage/50, damage/25));
        }
        if (Shilds != null && Shilds.Length > 0)
        {
            for (int i = Shilds.Length - 1; i >= 0; i--)
            {
                if (Shilds[i].SubSystemCurHealth > 0)
                {
                    ShieldEffect.GetComponent<Renderer>().enabled = true;
                    ShieldEffect.GetComponent<Forcefield>().OnHit(attackVector, 0, shieldsPower);

                    if (ingnoreDeltaTime)
                    {
                        Shilds[i].SubSystemCurHealth -= damage;
                    }
                    else
                    {
                        Shilds[i].SubSystemCurHealth -= Time.deltaTime * damage;   
                    }
                    if (MaxCrew > 0) CrewDamage(Random.Range(damage/10000, damage/9000));
                    break;
                }
            }

            if (Shilds[0].SubSystemCurHealth <= 0)
            {
                if (curHull - Time.deltaTime * damage > 0)
                {
                    ApplyHullAndSubSystemDamage(damage, aiming, ingnoreDeltaTime);
                    if (MaxCrew > 0) CrewDamage(Random.Range(damage/50, damage/25));
                }
                else
                {
                    curHull = 0;
                    Owner.destroyed = true;
                }
            }
        }
        else
        {
            if (curHull - Time.deltaTime * damage > 0)
            {
                ApplyHullAndSubSystemDamage(damage, aiming, ingnoreDeltaTime);
                if (MaxCrew > 0) CrewDamage(Random.Range(damage/50, damage/25));
            }
            else
            {
                curHull = 0;
                Owner.destroyed = true;
            }
        }

        Owner.modelEffects.UpdateDamageMat();
    }

    /// <summary> ����������� �������. </summary>
    public void DestroyObject()
    {
        if(ExplosionEffect != null)
        {
            GameObject _ex = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            _ex.transform.localScale = _ex.transform.localScale * ExplosionEffectScale;
        }
        Destroy(gameObject);
    }

    /// <summary> ���� �� �������. </summary>
    public void CrewDamage(float damage)
    {
        if (curCrew - damage > 0)
        {
            curCrew -= damage;
        }
        else
        {
            curCrew = 0;
        }
    }

    /// <summary> ������������� ������� � �������. </summary>
    public void InitHullAndCrew(float Hull, int Crew)
    {
        MaxHull = Hull;
        curHull = Hull;
        MaxCrew = Crew;
        curCrew = Crew;
    }

    /// <summary> ���������� ����� � �����������. </summary>
    void ApplyHullAndSubSystemDamage(float damage, STMethods.AttackType aiming, bool ingnoreDeltaTime = false)
    {
        switch (aiming)
        {
            case STMethods.AttackType.NormalAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.6f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.6f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.4f);
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.4f);
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
            case STMethods.AttackType.PrimaryWeaponSystemAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.2f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.2f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.1f);
                                if (_ss is PrimaryWeaponSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.7f;
                                }
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                                if (_ss is PrimaryWeaponSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.7f;
                                }
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }
                break;
            case STMethods.AttackType.SecondaryWeaponSystemAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.4f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.4f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.1f);
                                if (_ss is SecondaryWeaponSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.5f;
                                }
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                                if (_ss is SecondaryWeaponSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.5f;
                                }
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
            case STMethods.AttackType.ImpulseSystemAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.1f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.1f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.1f);
                                if (_ss is ImpulsEngineSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.8f;
                                }
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                                if (_ss is ImpulsEngineSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.8f;
                                }
                            }
                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
            case STMethods.AttackType.WarpEngingSystemAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.1f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.1f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.1f);
                                if (_ss is WarpEngineSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.7f;
                                }

                                if (_ss is WarpCoreSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.1f;
                                }
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                                if (_ss is WarpEngineSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.7f;
                                }

                                if (_ss is WarpCoreSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.1f;
                                }
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
            case STMethods.AttackType.WarpCoreAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.3f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.3f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.1f);
                                if (_ss is WarpEngineSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.1f;
                                }

                                if (_ss is WarpCoreSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.5f;
                                }
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                                if (_ss is WarpEngineSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.1f;
                                }

                                if (_ss is WarpCoreSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.5f;
                                }   
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
            case STMethods.AttackType.LifeSupportSystemAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.3f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.3f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.1f);
                                if (_ss is LifeSupportSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.6f;
                                }
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                                if (_ss is LifeSupportSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.6f;
                                }   
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
            case STMethods.AttackType.SensorsSystemAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.2f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.2f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                _ss.SubSystemCurHealth -= Random.Range(0, damage * 0.1f);
                                if (_ss is SensorSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.7f;
                                }
                            }
                            else
                            {
                                _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                                if (_ss is SensorSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.7f;
                                }   
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
            case STMethods.AttackType.TractorBeamSystemAttack:
                if (ingnoreDeltaTime)
                {
                    curHull -= damage * 0.01f;
                }
                else
                {
                    curHull -= Time.deltaTime * damage * 0.01f;   
                }
                if (SubSystems != null && SubSystems.Length > 0)
                {
                    foreach (SubSystem _ss in SubSystems)
                    {
                        if (!_ss.Immortal)
                        {
                            if (ingnoreDeltaTime)
                            {
                                if (_ss is TractorBeamSS)
                                {
                                    _ss.SubSystemCurHealth -= damage * 0.99f;
                                }
                            }
                            else
                            {
                                if (_ss is TractorBeamSS)
                                {
                                    _ss.SubSystemCurHealth -= Time.deltaTime * damage * 0.99f;
                                }   
                            }

                            _ss.ChangeEfficiency();
                        }
                    }
                }

                break;
        }
    }

    public bool NeedFix()
    {
        if (SubSystems != null && SubSystems.Length > 0)
        {
            for (int i = 0; i < SubSystems.Length; i++)
            {
                if ((!SubSystems[i].Immortal && SubSystems[i].SubSystemCurHealth < SubSystems[i].SubSystemMaxHealth) ||
                    curCrew < MaxCrew || curHull < MaxHull)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Fixing()
    {
        if (curCrew < MaxCrew)
        {
            if (Owner.PlayerNum > 0)
            {
                if (GameManager.instance.Players[Owner.PlayerNum - 1].Crew > Time.deltaTime * reCrewMultiplyer)
                {
                    curCrew += Time.deltaTime * reCrewMultiplyer;
                    GameManager.instance.Players[Owner.PlayerNum - 1].Crew -= Time.deltaTime * reCrewMultiplyer;
                }
            }
            else
            {
                curCrew += Time.deltaTime * reCrewMultiplyer;
            }
        }
        else
        {
            curCrew = MaxCrew;
        }

        if (curHull < MaxHull)
        {
            curHull += Time.deltaTime * 10;
        }
        else
        {
            curHull = MaxHull;
        }

        for (int i = 0; i < SubSystems.Length; i++)
        {
            if (!SubSystems[i].Immortal && SubSystems[i].SubSystemCurHealth < SubSystems[i].SubSystemMaxHealth)
            {
                SubSystems[i].SubSystemCurHealth += Time.deltaTime * 10;
                SubSystems[i].ChangeEfficiency();
            }
            else
            {
                SubSystems[i].SubSystemCurHealth = SubSystems[i].SubSystemMaxHealth;
            }
        }
        Owner.modelEffects.UpdateDamageMat();
    }

    public float ActiveShieldCurHealth()
    {
        if (Shilds != null && Shilds.Length > 0)
        {
            for (int i = Shilds.Length - 1; i >= 0; i--)
            {
                if (Shilds[i].SubSystemCurHealth > 0)
                {
                    return Shilds[i].SubSystemCurHealth;
                }
            }

            if (Shilds[0].SubSystemCurHealth <= 0)
            {
                return 0;
            }
            else
            {
                return Shilds[0].SubSystemCurHealth;
            }
        }
        else
        {
            return 0;
        }
    }

    public float ActiveShieldMaxHealth()
    {
        if (Shilds != null && Shilds.Length > 0)
        {
            for (int i = Shilds.Length - 1; i >= 0; i--)
            {
                if (Shilds[i].SubSystemCurHealth > 0)
                {
                    return Shilds[i].SubSystemMaxHealth;
                }
            }

            return Shilds[0].SubSystemMaxHealth;
        }
        else
        {
            return 0;
        }
    }
    
    private void SelfDestructionEvent()
    {
        if (SelfDestructActive)
        {
            if (SelfDestructTimer > 0)
            {
                SelfDestructTimer -= Time.deltaTime;
            }
            else
            {
                Owner.destroyed = true;
                curHull = 0;
                Timer = -1;
            }
        }
        else
        {
            if (SelfDestructTimer < 5)
            {
                SelfDestructTimer = 5;
            }
        }
    }

    public bool ShieldsEnable()
    {
        if (Shilds != null && Shilds.Length > 0)
        {
            for (int i = Shilds.Length - 1; i >= 0; i--)
            {
                if (Shilds[i].SubSystemCurHealth > 0)
                {
                    return true;
                }
            }

            if (Shilds[0].SubSystemCurHealth <= 0)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return false;
    }

    public Transform getRandomFirePoint(STMethods.AttackType aiming, Transform attackingVessel)
    {
        Transform returnTransform = null;
        if (aiming == STMethods.AttackType.NormalAttack)
        {
            if (HullPoints != null && HullPoints.Length > 0)
            {
                if (HullPoints.Length >= 3)
                {
                    returnTransform =
                        STMethods.NearestTransformSortList(HullPoints, attackingVessel)[Random.Range(0, 3)];
                }
                else
                {
                    returnTransform =
                        STMethods.NearestTransformSortList(HullPoints, attackingVessel)[
                            Random.Range(0, HullPoints.Length)];
                }
            }
        }
        if (SubSystems != null && SubSystems.Length > 0)
        {
            switch (aiming)
            {
                case STMethods.AttackType.PrimaryWeaponSystemAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is PrimaryWeaponSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
                case STMethods.AttackType.SecondaryWeaponSystemAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is SecondaryWeaponSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
                case STMethods.AttackType.ImpulseSystemAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is ImpulsEngineSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
                case STMethods.AttackType.WarpEngingSystemAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is WarpEngineSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
                case STMethods.AttackType.WarpCoreAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is WarpCoreSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
                case STMethods.AttackType.LifeSupportSystemAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is LifeSupportSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
                case STMethods.AttackType.SensorsSystemAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is SensorSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
                case STMethods.AttackType.TractorBeamSystemAttack:
                    foreach (SubSystem system in SubSystems)
                    {
                        if (system is TractorBeamSS)
                        {
                            if (system.effects.AimingPoints != null && system.effects.AimingPoints.Length > 0)
                            {
                                if (system.effects.AimingPoints.Length >= 3)
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, 3)];
                                }
                                else
                                {
                                    returnTransform =
                                        STMethods.NearestTransformSortList(system.effects.AimingPoints, attackingVessel)
                                            [Random.Range(0, system.effects.AimingPoints.Length)];
                                }
                            }
                        }
                    }

                    break;
            }
        }

        if (returnTransform == null) returnTransform = transform;
        return returnTransform;
    }
    
    void OnCollisionStay(Collision collision)
    {
        if (Owner is ResourceSource)
        {
            ResourceSource res = Owner as ResourceSource;
            if (res.spawner != null)
            {
                if(res.spawner.curAsteroids.Any(x => x == res)) res.spawner.curAsteroids.Remove(res);
            }
        }
        if (GetComponentInChildren<ImpulsEngineSS>())
        {
            ImpulsEngineSS _ie = GetComponentInChildren<ImpulsEngineSS>();
            _ie.Hit();
            if (Owner.effectManager.ImpulseEngineEffectActivity() &&
                 _ie.SubSystemCurHealth > _ie.SubSystemMaxHealth * 0.125f)
            {
                if (MaxCrew > 0 && (int) curCrew > 0)
                {
                    return;
                }

                if (MaxCrew <= 0)
                {
                    return;
                }
            }
        }

        Rigidbody otherRigitbody = collision.transform.GetComponent<Rigidbody>();
        SelectableObject otherSel = collision.transform.GetComponent<SelectableObject>();

        float massDifference;
        float maxMass;
        
        if (otherSel is ResourceSource)
        {
            ResourceSource res = otherSel as ResourceSource;
            if (res.spawner != null)
            {
                if(res.spawner.curAsteroids.Any(x => x == res)) res.spawner.curAsteroids.Remove(res);
            }
        }
        
        if (Owner.rigitBody.mass > otherRigitbody.mass)
        {
            maxMass = Owner.rigitBody.mass;
            massDifference = otherRigitbody.mass / Owner.rigitBody.mass;

            foreach (ContactPoint contact in collision.contacts)
            {
                if (collision.relativeVelocity.magnitude < 5)
                {
                    ApplyDamage(0, STMethods.AttackType.NormalAttack, contact.point);
                    if (otherSel.healthSystem)
                        otherSel._hs.ApplyDamage(0, STMethods.AttackType.NormalAttack, contact.point);
                }
                else
                {
                    ApplyDamage(collision.relativeVelocity.magnitude * maxMass * massDifference / 1000 * Time.deltaTime,
                        STMethods.AttackType.NormalAttack, contact.point);
                    if (otherSel.healthSystem)
                        otherSel._hs.ApplyDamage(
                            collision.relativeVelocity.magnitude * maxMass / massDifference / 200 * Time.deltaTime,
                            STMethods.AttackType.NormalAttack, contact.point);
                }
            }
        }
        else
        {
            maxMass = otherRigitbody.mass;
            massDifference = Owner.rigitBody.mass / otherRigitbody.mass;

            foreach (ContactPoint contact in collision.contacts)
            {
                if (collision.relativeVelocity.magnitude < 5)
                {
                    ApplyDamage(0, STMethods.AttackType.NormalAttack, contact.point);
                    if (otherSel.healthSystem)
                        otherSel._hs.ApplyDamage(0, STMethods.AttackType.NormalAttack, contact.point);
                }
                else
                {
                    ApplyDamage(collision.relativeVelocity.magnitude * maxMass * massDifference / 200 * Time.deltaTime,
                        STMethods.AttackType.NormalAttack, contact.point);
                    if (otherSel.healthSystem)
                        otherSel._hs.ApplyDamage(
                            collision.relativeVelocity.magnitude * maxMass / massDifference / 1000 * Time.deltaTime,
                            STMethods.AttackType.NormalAttack, contact.point);
                }
            }
        }
    }
}