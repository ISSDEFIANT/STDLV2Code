using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpCoreSS : SubSystem
{
    public GameObject WarpCoreExplosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (efficiency == 0)
        {
            DeActive();
        }
    }
    public void DeActive()
    {
        Instantiate(WarpCoreExplosion, transform.position, Quaternion.identity);
        healthSystem.DestroyObject();
    }
}
