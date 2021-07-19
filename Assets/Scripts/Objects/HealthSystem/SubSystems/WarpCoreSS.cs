using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpCoreSS : SubSystem
{
    /// <summary> Вспышка взрыва варп ядра. </summary>
    public GameObject WarpCoreExplosion;
    // Start is called before the first frame update
    public override void isCreated()
    {
        Owner.effectManager.warpCore = this;
    }

    /// <summary> Проверка работоспособности системы. </summary>
    public override void Update()
    {
        base.Update();
        if (efficiency == 0)
        {
            DeActive();
        }
    }
    /// <summary> Взрыв ядра. </summary>
    public void DeActive()
    {
        Instantiate(WarpCoreExplosion, transform.position, Quaternion.identity);
        healthSystem.Timer = 0.05f;
        healthSystem.curHull = 0;
        this.enabled = false;
    }
}
