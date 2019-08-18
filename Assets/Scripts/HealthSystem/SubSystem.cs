using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSystem : MonoBehaviour
{
    /// <summary> Система жизней. </summary>
    public HealthSystem healthSystem;
    
    /// <summary> Максимальная жизнь подсистемы. </summary>
    public float SubSystemMaxHealth; 
    /// <summary> Текущая жизнь подсистемы. </summary>
    public float SubSystemCurHealth;
    /// <summary> Эффективность подсистемы. </summary>
    public float efficiency;
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    /// <summary> Основная механика подсистемы. </summary>
    public virtual void Update()
    {
        ChangeEfficiency();

        SubSystemCurHealth = Mathf.Clamp(SubSystemCurHealth, 0, SubSystemMaxHealth);

        if (SubSystemCurHealth < SubSystemMaxHealth && SubSystemCurHealth > 0)
        {
            RegenerateSystem();
        }
    }
    /// <summary> Полевой ремонт подсистемы. </summary>
    private void RegenerateSystem()
    {
        if (SubSystemCurHealth < SubSystemMaxHealth)
        {
            SubSystemCurHealth += Time.deltaTime;
        }
        else if (SubSystemCurHealth > SubSystemMaxHealth)
        {
            SubSystemCurHealth = SubSystemMaxHealth;
        }
    }
    /// <summary> Изменение эффективности. </summary>
    void ChangeEfficiency()
    {
        efficiency = SubSystemCurHealth / SubSystemMaxHealth;
        if (efficiency < 0)
        {
            efficiency = 0;
        }
    }
    /// <summary> Инициализация жизни подсистемы. </summary>
    public SubSystem InitSystemHealth(float Health)
    {
        SubSystemMaxHealth = Health;
        SubSystemCurHealth = Health;
        return this;
    }
}
