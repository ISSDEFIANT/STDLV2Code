using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEffect : ObjectEffect
{
    // Start is called before the first frame update
    void Start()
    {
        effectName = "Star radiation";
        LifeTime = 1f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (target.healthSystem)
        {
            RaycastHit hit;
            int layerMask = 1 << 13 | 1 << 9;
            if (Physics.Raycast(SoucePosition, transform.position - SoucePosition,
                out hit, Mathf.Infinity, layerMask))
            {
                if (target._hs.ShieldsEnable())
                {
                    target._hs.ApplyDamage(5, STMethods.AttackType.LifeSupportSystemAttack, hit.point, false, 0);
                }
                else
                {
                    target._hs.ApplyDamage(0.5f, STMethods.AttackType.LifeSupportSystemAttack, hit.point);
                }
            }
        }
    }
}
