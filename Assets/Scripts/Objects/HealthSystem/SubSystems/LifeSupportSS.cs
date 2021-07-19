using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSupportSS : SubSystem
{
    // Start is called before the first frame update
    public override void isCreated()
    {
        Owner.effectManager.lifeSupport = this;
    }

    /// <summary> Проверка работоспособности системы. </summary>
    public override void Update()
    {
        base.Update();
        Owner._hs.curCrew = Owner.effectManager.UpdateCrew(Owner._hs.curCrew, Owner._hs.MaxCrew, this);
    }
    /// <summary> В случае отказа системы. </summary>
    public void DeActive()
    {

    }
}