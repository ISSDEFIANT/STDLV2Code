using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueStar : SystemCentralObject
{
     // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        ObjectIcon = DataLoader.Instance.ResourcesCache["Star/Blue/Icon"] as Sprite;
        
        Quaternion init = this.transform.rotation;

        model = Instantiate(DataLoader.Instance.ResourcesCache["Star/Blue"] as GameObject, transform.position, init);

        model.transform.parent = transform;
        model.transform.position = new Vector3(0, -250, 0);
        ObjectRadius = 750;

        ObjectBluePrint[8] = ObjectIcon;

        FindInmodelElements();

        gravityRadius = 3000;
        gravitySlow = 10;
    }
}