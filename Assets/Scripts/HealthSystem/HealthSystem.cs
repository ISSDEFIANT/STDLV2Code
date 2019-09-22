using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    /// <summary> Владелец. </summary>
    public SelectableObject Owner;
    
    /// <summary> Максимальная целостность корпуса. </summary>
    public float MaxHull;
    /// <summary> Текущая целостность корпуса. </summary>
    public float curHull;

    /// <summary> Максимальный экипаж. </summary>
    public float MaxCrew;
    /// <summary> Текущий экипаж. </summary>
    public float curCrew;

    /// <summary> Щиты. </summary>
    public SubSystem[] Shilds;
    /// <summary> Подсистемы. </summary>
    public SubSystem[] SubSystems;
    /// <summary> Таймер от разрушения корпуса до взрыва. </summary>
    protected float Timer = 3;
    /// <summary> Взрыв. </summary>
    public GameObject ExplosionEffect;
    /// <summary> Эфффект щитов. </summary>
    private GameObject ShieldEffect;
    
    /// <summary> Настройка подсистем и щитов. </summary>
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

    /// <summary> Работа таймера в случае разрушения судна. </summary>
    void Update()
    {
        if (curHull <= 0)
        {
            Owner.destroyed = true;
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
    /// <summary> Применение урона к объекту. </summary>
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
    /// <summary> Уничтожение объекта. </summary>
    public void DestroyObject()
    {
        Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    /// <summary> Урон по экипажу. </summary>
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
    /// <summary> Инициализация корпуса и экипажа. </summary>
    public void InitHullAndCrew(float Hull, int Crew)
    {
        MaxHull = Hull;
        curHull = Hull;
        MaxCrew = Crew;
        curCrew = Crew;
    }
    /// <summary> Применение урона к подсистемам. </summary>
    void ApplyHullAndSubSystemDamage(float damage, STMethods.AttackType aiming)
    {
        switch (aiming)
        {
            case STMethods.AttackType.NormalAttack:
                curHull -= damage*0.6f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage*0.4f);
                }
                break;
            case STMethods.AttackType.PrimaryWeaponSystemAttack:
                curHull -= damage*0.2f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime * Random.Range(0, damage * 0.1f);
                    if (_ss is PrimaryWeaponSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.7f;
                    }
                }
                break;
            case STMethods.AttackType.SecondaryWeaponSystemAttack:
                curHull -= damage*0.4f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage*0.1f);
                    if (_ss is SecondaryWeaponSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.5f;
                    }
                }
                break;
            case STMethods.AttackType.ImpulseSystemAttack:
                curHull -= damage*0.1f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage*0.1f);
                    if (_ss is ImpulsEngineSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.8f;
                    }
                }
                break;
            case STMethods.AttackType.WarpEngingSystemAttack:
                curHull -= damage*0.1f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage*0.1f);
                    if (_ss is WarpEngineSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.7f;
                    }
                    if (_ss is WarpCoreSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.1f;
                    }
                }
                break;
            case STMethods.AttackType.WarpCoreAttack:
                curHull -= damage*0.3f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage*0.1f);
                    if (_ss is WarpEngineSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.1f;
                    }
                    if (_ss is WarpCoreSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.5f;
                    }
                }
                break;
            case STMethods.AttackType.LifeSupportSystemAttack:
                curHull -= damage*0.3f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage*0.1f);
                    if (_ss is LifeSupportSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.6f;
                    }
                }
                break;
            case STMethods.AttackType.SensorsSystemAttack:
                curHull -= damage*0.2f;
                foreach (SubSystem _ss in SubSystems)
                {
                    _ss.SubSystemCurHealth -= Time.deltaTime*Random.Range(0, damage*0.1f);
                    if (_ss is SensorSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.7f;
                    }
                }
                break;
            case STMethods.AttackType.TractorBeamSystemAttack:
                curHull -= damage*0.01f;
                foreach (SubSystem _ss in SubSystems)
                {
                    if (_ss is TractorBeamSS)
                    {
                        _ss.SubSystemCurHealth -= damage*0.99f;
                    }
                }
                break;
        }
    }
}