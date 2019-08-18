using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShipType1 : Mobile
{
    protected GameObject subModulesObj;
    
    protected GameObject ShildsObj;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        frameSelection = true;
        healthSystem = true;
        
        _hs = gameObject.AddComponent<HealthSystem>();
        
        subModulesObj = new GameObject();
        subModulesObj.transform.parent = transform;
        subModulesObj.transform.localPosition = Vector3.zero;
        subModulesObj.name = "SubSystems";
        
        ShildsObj = new GameObject();
        ShildsObj.transform.parent = transform;
        ShildsObj.transform.localPosition = Vector3.zero;
        ShildsObj.name = "Shilds";
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    protected void initShilds(int count, GameObject ShildObject, HealthSystem _hs, float shildForce, float shieldDelay, float shieldDivider)
    {
        _hs.Shilds = Enumerable.Range(0,count).Select(x=>ShildObject.AddComponent<NormalRaceShield>().InitShield(shieldDelay,shieldDivider,shildForce)).ToArray();
    }
}