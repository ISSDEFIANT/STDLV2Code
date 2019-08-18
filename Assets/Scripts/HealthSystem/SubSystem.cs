using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSystem : MonoBehaviour
{
    public HealthSystem healthSystem;
    
    public float SubSystemMaxHealth;
    public float SubSystemCurHealth;
    
    public float efficiency;
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        ChangeEfficiency();

        SubSystemCurHealth = Mathf.Clamp(SubSystemCurHealth, 0, SubSystemMaxHealth);

        if (SubSystemCurHealth < SubSystemMaxHealth && SubSystemCurHealth > 0)
        {
            RegenerateSystem();
        }
    }
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
    void ChangeEfficiency()
    {
        efficiency = SubSystemCurHealth / SubSystemMaxHealth;
        if (efficiency < 0)
        {
            efficiency = 0;
        }
    }

    public SubSystem InitSystemHealth(float Health)
    {
        SubSystemMaxHealth = Health;
        SubSystemCurHealth = Health;
        return this;
    }
}
