using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRaceShield : SubSystem
{
    /// <summary> Время полной перезагрузки щитов. </summary>
    public float Delay = 180;
    /// <summary> Делитель для дозарядки щита. </summary>
    public float RechargeDivider = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    /// <summary> Основная механика перезарядки и дозарядки щита. </summary>
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
    /// <summary> Инициализация щита. </summary>
    public SubSystem InitShield(float _delay, float _rechargeDivider, float Health)
    {
        SubSystemMaxHealth = Health;
        SubSystemCurHealth = Health;
        Delay = _delay;
        RechargeDivider = _rechargeDivider;
        return this;
    }
}
