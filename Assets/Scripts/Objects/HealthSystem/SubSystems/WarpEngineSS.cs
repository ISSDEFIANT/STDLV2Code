using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpEngineSS : SubSystem
{
    // Start is called before the first frame update
    public override void isCreated()
    {
        Owner.effectManager.warpEngine = this;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
