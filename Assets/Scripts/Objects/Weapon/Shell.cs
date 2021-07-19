using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class Shell : MonoBehaviour
{
    /// <summary> Кто выпустил. </summary>
    public GameObject owner;
    /// <summary> Наведение. </summary>
    public STMethods.AttackType attackType;
    /// <summary> Цель. </summary>
    public SelectableObject target;
    /// <summary> Урон. </summary>
    public int damage;
    /// <summary> Скорость движения. </summary>
    public int moveSpeed = 1;
    /// <summary> Радиус. </summary>
    public float Radius;
    /// <summary> Наведение по дистанции. </summary>
    public bool DistanceAiming;
    /// <summary> Прошлая дистанции. </summary>
    public float Distance;
    /// <summary> Missed. </summary>
    public bool Missed;
    /// <summary> Топливо. </summary>
    public float Fuild;
    /// <summary> Масксимальное топливо. </summary>
    private float MaxFuild;
    /// <summary> Эффект взрыва снаряда. </summary>
    public GameObject ExplosionEffect;
    /// <summary> Звук выстрела. </summary>
    public AudioClip FireSound;
    /// <summary> Звук попадания в щит. </summary>
    public AudioClip ShieldHit;
    /// <summary> Звук попадания в корпус. </summary>
    public AudioClip HullHit;
    
    /// <summary> Импульсное оружие. </summary>
    public bool isImpulseWeapon;

    /// <summary> Игнорирует щиты. </summary>
    public bool ignoreShields;
    /// <summary> Мина. </summary>
    public bool isMine;
    /// <summary> Зона обнаружения врага миной. </summary>
    public float scanRange;
    /// <summary> Игрок, поставивший. </summary>
    public int PlayerNum;

    private AudioSource _as;

    /// <summary> Первая установка топлива. </summary>
    void Awake()
    {
        if(gameObject.GetComponent<AudioSource>())
        _as = gameObject.GetComponent<AudioSource>();
        MaxFuild = Fuild;
    }

    // Start is called before the first frame update
    void Start()
    {
        Distance = Mathf.Infinity;
    }

    /// <summary> Основная механика снаряда. </summary>
    void Update()
    {
        if (owner.IsDestroyed()) owner = null;
        int mask = 1 << 9;
        if (isMine && target == null)
        {
            Collider[] scanColls = Physics.OverlapSphere(transform.position, scanRange, mask);
            if (scanColls.Length > 0)
            {
                float distance = Mathf.Infinity;
                SelectableObject tar = null;
                foreach (Collider coll in scanColls)
                {
                    if (coll.transform.root.GetComponent<SelectableObject>() &&
                        coll.transform.root.GetComponent<SelectableObject>().PlayerNum > 0)
                    {
                        if (GameManager.instance.Players[PlayerNum - 1].TeamNum != GameManager.instance
                            .Players[coll.transform.root.GetComponent<SelectableObject>().PlayerNum - 1].TeamNum)
                        {
                            if (distance > Vector3.Distance(transform.position, coll.transform.root.position))
                            {
                                tar = coll.transform.root.GetComponent<SelectableObject>();
                                distance = Vector3.Distance(transform.position, coll.transform.root.position);
                            }
                        }

                    }
                }

                if (tar == null) return;
                target = tar;
                Vector3 LookVector = (target.transform.position - transform.position);
                transform.rotation = Quaternion.LookRotation(LookVector);
                return;
            }

            return;
        }
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        if (target != null)
        {
            if (Fuild > 0)
            {
                Vector3 LookVector = ((target.transform.position + target.rigitBody.velocity) - transform.position);
                transform.rotation =
                    Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(LookVector), 0.05f);
                Fuild -= Time.deltaTime;
            }
            else
            {
                if (DistanceAiming)
                {
                    if (!Missed)
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) < Distance)
                        {
                            Vector3 LookVector =
                                ((target.transform.position + target.rigitBody.velocity) - transform.position);
                            transform.rotation =
                                Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(LookVector), 0.05f);
                            Distance = Vector3.Distance(transform.position, target.transform.position);
                        }
                        else
                        {
                            Missed = true;
                        }
                    }
                }
            }
        }
        Collider[] colls = Physics.OverlapSphere(transform.position, Radius, mask);
        foreach (Collider coll in colls)
        {
            if(owner != null && hitOwner(coll, owner)) return;
            if (!coll.transform.root.GetComponent<Shell>())
            {
                if (coll.transform.root.GetComponent<HealthSystem>())
                {
                    HealthSystem _TarHS = coll.transform.root.GetComponent<HealthSystem>();
                    
                    if (ignoreShields)
                    {
                        _TarHS.ApplyDamage(damage, attackType, transform.position + transform.forward*-Radius, true, default, true);
                        _as.clip = HullHit;
                        _as.Play();
                    }

                    if (_TarHS.Shilds != null && _TarHS.Shilds.Length > 0)
                    {
                        if (_TarHS.Shilds[0].SubSystemCurHealth <= 0)
                        {
                            _TarHS.ApplyDamage(damage, attackType, transform.position + transform.forward*-Radius, default, default, true);
                            _as.clip = HullHit;
                            _as.Play();
                        }
                        else
                        {
                            if (isImpulseWeapon)
                            {
                                _TarHS.ApplyDamage(damage, attackType, transform.position + transform.forward*-Radius, default, default, true);
                            }
                            else
                            {
                                _TarHS.ApplyDamage(damage / 2, attackType, transform.position + transform.forward*-Radius, default, default, true);
                            }
                            _as.clip = ShieldHit;
                            _as.Play();
                        }
                    }
                    else
                    {
                        _TarHS.ApplyDamage(damage, attackType, transform.position + transform.forward*-Radius, true, default, true);
                        _as.clip = HullHit;
                        _as.Play();
                    }
                    
                    if (_TarHS.Owner.rigitBody.drag == 0)
                    {
                        if (_TarHS.Owner is ResourceSource)
                        {
                            ResourceSource res = _TarHS.Owner as ResourceSource;
                            if (res.spawner != null)
                            {
                                if(res.spawner.curAsteroids.Any(x => x == res)) res.spawner.curAsteroids.Remove(res);
                            }
                            _TarHS.Owner.rigitBody.AddForceAtPosition((_TarHS.Owner.transform.position - transform.position).normalized * damage * (_TarHS.Owner.rigitBody.mass / 10), transform.position);
                        }
                        else
                        {
                            _TarHS.Owner.rigitBody.AddForceAtPosition((_TarHS.Owner.transform.position - transform.position).normalized * damage * (_TarHS.Owner.rigitBody.mass / 1000), transform.position);
                        }
                    }
                }

                Instantiate(ExplosionEffect, transform.position, transform.rotation);
                if (isMine)
                {
                    Destroy(gameObject);
                    return;
                }
                DestroyAlternative();
            }
        }
    }
    /// <summary> Отключение снаряда. </summary>
    void DestroyAlternative()
    {
        Distance = Mathf.Infinity;
        Missed = false;
        DiactivateObject _d = gameObject.GetComponent<DiactivateObject>();
        Fuild = MaxFuild;
        _d.Diactivate();
    }

    public void PlayFireSound()
    {
        _as.clip = FireSound;
        _as.Play();
    }
    
    public bool hitOwner(Collider hit, GameObject owner)
    {
        if (hit.transform.root == owner.transform) return true;
        return false;
    }
}