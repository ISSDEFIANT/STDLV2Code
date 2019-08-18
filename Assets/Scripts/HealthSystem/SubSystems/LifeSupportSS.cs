using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSupportSS : SubSystem
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (efficiency <= 0.1)
        {
            DeActive();
        }
    }

    public void DeActive()
    {
        if (healthSystem.curCrew > healthSystem.MaxCrew / 100)
        {
            healthSystem.CrewDamage(Time.deltaTime * healthSystem.MaxCrew / 100);
        }
        else
        {
            healthSystem.CrewDamage(Time.deltaTime * healthSystem.MaxCrew / 10000);
        }
    }
}