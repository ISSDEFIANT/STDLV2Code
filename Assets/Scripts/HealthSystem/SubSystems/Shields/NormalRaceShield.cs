using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRaceShield : SubSystem
{
    public float Delay = 180;

    public float RechargeDivider = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (SubSystemCurHealth > SubSystemMaxHealth / 8)
        {
            if (SubSystemCurHealth < SubSystemMaxHealth)
            {
                SubSystemCurHealth += Time.deltaTime * SubSystemMaxHealth / RechargeDivider;
            }
            else
            {
                SubSystemCurHealth = SubSystemMaxHealth;
            }
        }
        else
        {
            if (Delay > 0)
            {
                Delay -= Time.deltaTime;
            }
            else
            {
                SubSystemCurHealth = SubSystemMaxHealth;
                Delay = 180;
            }
        }
    }
    public SubSystem InitShield(float _delay, float _rechargeDivider, float Health)
    {
        SubSystemMaxHealth = Health;
        SubSystemCurHealth = Health;
        Delay = _delay;
        RechargeDivider = _rechargeDivider;
        return this;
    }
}
