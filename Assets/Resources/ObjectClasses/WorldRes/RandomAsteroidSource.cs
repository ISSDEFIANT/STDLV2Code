using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAsteroidSource : ResourceSource
{
    // Start is called before the first frame update
    public override void Awake()
    {
        int selection = Random.Range(0, 6);
        
        int size = Random.Range(5, 25); 
        
        base.Awake();
        
        Quaternion init = this.transform.rotation;

        switch (selection)
        {
            case 0:
                model = Instantiate(DataLoader.Instance.ResourcesCache["Asteroid 1"] as GameObject, transform.position, init);
                break;
            case 1:
                model = Instantiate(DataLoader.Instance.ResourcesCache["Asteroid 2"] as GameObject, transform.position, init);
                break;
            case 2:
                model = Instantiate(DataLoader.Instance.ResourcesCache["Asteroid 3"] as GameObject, transform.position, init);
                break;
            case 3:
                model = Instantiate(DataLoader.Instance.ResourcesCache["Asteroid 4"] as GameObject, transform.position, init);
                break;
            case 4:
                model = Instantiate(DataLoader.Instance.ResourcesCache["Asteroid 5"] as GameObject, transform.position, init);
                break;
            case 5:
                model = Instantiate(DataLoader.Instance.ResourcesCache["Asteroid 6"] as GameObject, transform.position, init);
                break;
        }

        model.transform.parent = transform;
        model.transform.localScale = new Vector3(size,size,size);

        ObjectRadius = size;
        SensorRange = 0;
        
        WeaponRange = 0;
        MaxAttackTargetCount = 0;
        
        _hs.InitHullAndCrew(Random.Range(5, 20) * size, 0);

        _hs.ExplosionEffect = (GameObject)DataLoader.Instance.ResourcesCache["AsteroidExplosion"];
        _hs.ExplosionEffectScale = size;
        
        FindInmodelElements();
        
        modelEffects.Deactivate = true;
        
        rigitBody.mass = _hs.curHull;

        curTitanium = Random.Range(0, 10) * size*size;
        maxTitanium = curTitanium;
        curDilithium = Random.Range(0, 3) * size*size;
        maxDilithium = curDilithium;
    }
}