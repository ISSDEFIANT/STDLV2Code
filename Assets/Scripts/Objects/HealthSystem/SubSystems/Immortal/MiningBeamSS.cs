using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningBeamSS : SubSystem
{
    public MiningController _mc;

    private float MaxRes;
    private bool canDilithium;
    private bool canTitanium;
    private bool canBiomatter;
    
    /// <summary> Луч добычи. </summary>
    public MiningBeam Beam;
    // Start is called before the first frame update
    void Awake()
    {
        Immortal = true;
        SubSystemMaxHealth = 100;
        SubSystemCurHealth = 100;
    }
    public override void isCreated()
    {
        if (!Owner.GetComponent<MiningController>())
        {
            _mc = Owner.gameObject.AddComponent<MiningController>();
            _mc.MaxResources = MaxRes;
            _mc.Dilithium = canDilithium;
            _mc.Titanium = canTitanium;
            _mc.Biomatter = canBiomatter;
            _mc.System = this;
        }

        Beam = Owner.GetComponentInChildren<MiningBeam>();
    }

    public SubSystem SetMaxResources(float mRes, bool Dilithium, bool Titanium, bool Biomatter, SelectableObject ow)
    {
        Owner = ow;
        MaxRes = mRes;
        canDilithium = Dilithium;
        canTitanium = Titanium;
        canBiomatter = Biomatter;
        isCreated();
        return this;
    }

    public void ActiveBeam(Transform point)
    {
        Beam.Active(point);
    }
}
