using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [HideInInspector]public float collisionDelay;
    
    public STMethods.AttackType attackType;

    public Transform target;

    public int damage;
    public int moveSpeed = 1;

    public float Radius;
    public float Fuild;

    private float MaxFuild;

    public GameObject ExplosionEffect;

    // Use this for initialization
    void Awake()
    {
        MaxFuild = Fuild;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
                        coll.GetComponent<HealthSystem>().ApplyDamage(damage, attackType,transform.forward * moveSpeed);
                    }

                    Instantiate(ExplosionEffect, transform.position, transform.rotation);
                    Debug.Log(coll.name);
                    DestroyAlternative();
                }
            }
            else
            {
                collisionDelay -= Time.deltaTime;
            }
        }
    }
    void DestroyAlternative()
    {
        DiactivateObject _d = gameObject.GetComponent<DiactivateObject>();
        Fuild = MaxFuild;
        _d.Diactivate();
    }
}