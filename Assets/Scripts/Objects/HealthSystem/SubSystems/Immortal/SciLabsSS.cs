using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SciLabsSS : SubSystem
{
    public SciController _sc;
    public STMethods.Races RacePlaneOpening;
    void Awake()
    {
        Immortal = true;
        SubSystemMaxHealth = 100;
        SubSystemCurHealth = 100;
    }

    public SubSystem SetStationRace(STMethods.Races OpenTecnologyPlane, SelectableObject ow)
    {
        Owner = ow;
        RacePlaneOpening = OpenTecnologyPlane;
        isCreated();
        return this;
    }

    public override void isCreated()
    {
        if (!Owner.GetComponent<SciController>())
        {
            _sc = Owner.gameObject.AddComponent<SciController>();
            _sc.OpenTecTree = RacePlaneOpening;
            _sc.system = this;
        }
    }
}
