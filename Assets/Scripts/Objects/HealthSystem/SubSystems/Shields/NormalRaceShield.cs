using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRaceShield : SubSystem
{
    /// <summary> Время полной перезагрузки щитов. </summary>
    public float Delay = 180;

    /// <summary> Делитель для дозарядки щита. </summary>
    public float RechargeDivider = 100;
    
    /// <summary> Щит во время отключения. </summary>
    public float DisabledShield = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    /// <summary> Основная механика перезарядки и дозарядки щита. </summary>
    void Update()
    {
        if (Owner.Alerts == STMethods.Alerts.GreenAlert)
        {
            if (DisabledShield == -1)
            {
                DisabledShield = SubSystemCurHealth;
            }
            SubSystemCurHealth = 0;

            if (DisabledShield > SubSystemMaxHealth / 8)
            {
                if (DisabledShield < SubSystemMaxHealth)
                {
                    DisabledShield += Time.deltaTime * SubSystemMaxHealth / RechargeDivider;
                }
                else
                {
                    DisabledShield = SubSystemMaxHealth;
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
                    DisabledShield = SubSystemMaxHealth;
                    Delay = 180;
                }
            }
            return;
        }
        if (DisabledShield != -1)
        {
            SubSystemCurHealth = DisabledShield;
            DisabledShield = -1;
        }
        if (SubSystemCurHealth > SubSystemMaxHealth / 8)
        {
            if (SubSystemCurHealth < SubSystemMaxHealth)
            {
                switch (Owner.Alerts)
                { 
                    case STMethods.Alerts.RedAlert:
                        SubSystemCurHealth += Time.deltaTime * SubSystemMaxHealth / (RechargeDivider*2);
                        break;
                    case STMethods.Alerts.YellowAlert:
                        SubSystemCurHealth += Time.deltaTime * SubSystemMaxHealth / (RechargeDivider*1.5f);
                        break;
                }
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
                switch (Owner.Alerts)
                { 
                    case STMethods.Alerts.RedAlert:
                        Delay -= Time.deltaTime/2;
                        break;
                    case STMethods.Alerts.YellowAlert:
                        Delay -= Time.deltaTime/1.5f;
                        break;
                }
            }
            else
            {
                SubSystemCurHealth = SubSystemMaxHealth;
                Delay = 180;
            }
        }
    }
    /// <summary> Инициализация щита. </summary>
    public SubSystem InitShield(float _delay, float _rechargeDivider, float Health, SelectableObject ow)
    {
        Owner = ow;
        SubSystemMaxHealth = Health;
        SubSystemCurHealth = Health;
        Delay = _delay;
        RechargeDivider = _rechargeDivider;
        return this;
    }
}
