using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    /// <summary> Время до активации обнаружения столкновений. </summary>
    [HideInInspector]public float collisionDelay;
    /// <summary> Наведение. </summary>
    public STMethods.AttackType attackType;
    /// <summary> Цель. </summary>
    public Transform target;
    /// <summary> Урон. </summary>
    public int damage;
    /// <summary> Скорость движения. </summary>
    public int moveSpeed = 1;
    /// <summary> Радиус. </summary>
    public float Radius;
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

    }

    /// <summary> Основная механика снаряда. </summary>
    void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        if (target != null)
        {
            if (Fuild > 0)
            {
                Vector3 LookVector = (target.transform.position - this.transform.position);
                this.transform.rotation =
                    Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(LookVector), 0.05f);
                Fuild -= Time.deltaTime;
            }
        }

        Collider[] colls = Physics.OverlapSphere(transform.position, Radius);
        foreach (Collider coll in colls)
        {
            if (collisionDelay <= 0)
            {
                if (!coll.GetComponent<Shell>())
                {
                    if (coll.GetComponent<HealthSystem>())
                    {
                        HealthSystem _TarHS = coll.GetComponent<HealthSystem>();
                        _TarHS.ApplyDamage(damage, attackType,transform.forward * moveSpeed);
                        if (_TarHS.Shilds.Length > 0)
                        {
                            if (_TarHS.Shilds[0].SubSystemCurHealth <= 0)
                            {
                                _as.clip = HullHit;
                                _as.Play();
                            }
                            else
                            {
                                _as.clip = ShieldHit;
                                _as.Play();
                            }
                        }
                        else
                        {
                            _as.clip = HullHit;
                            _as.Play();
                        }
                    }

                    Instantiate(ExplosionEffect, transform.position, transform.rotation);
                    DestroyAlternative();
                }
            }
            else
            {
                collisionDelay -= Time.deltaTime;
            }
        }
    }
    /// <summary> Отключение снаряда. </summary>
    void DestroyAlternative()
    {
        DiactivateObject _d = gameObject.GetComponent<DiactivateObject>();
        Fuild = MaxFuild;
        _d.Diactivate();
    }

    public void PlayFireSound()
    {
        _as.clip = FireSound;
        _as.Play();
    }
}