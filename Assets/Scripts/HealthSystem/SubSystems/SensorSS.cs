using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Controllers;

public class SensorSS : SubSystem
{
    /// <summary> Радиус. </summary>
    public float radius;
    
    /// <summary> Текущий радиус. </summary>
    public float curRadius;

    /// <summary> Все объекты в зоне видимости. </summary>
    public List<SelectableObject> ObjectsIsVisible = new List<SelectableObject>();

    /// <summary> Менеджер. </summary>
    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (efficiency > 0.5f)
        {
            curRadius = radius;
            Scan();
        }
        else if (efficiency < 0.5f && efficiency > 0.2)
        {
            curRadius = radius/2;
            Scan();
        }
        else if (efficiency < 0.5f && efficiency > 0.2)
        {
            curRadius = 0;
        }
    }

    void Scan()
    {
        STMethods.RemoveAllNullsFromList(ObjectsIsVisible);
        foreach (SelectableObject obj in manager.SelectableObjects)
        {
            if (Vector3.Distance(obj.transform.position, Owner.transform.position) > (curRadius+obj.ObjectRadius+Owner.ObjectRadius))
            {
                if (STMethods.FindInList(obj, ObjectsIsVisible))
                {
                    ObjectsIsVisible.Remove(obj);
                }
            }
            else
            {
                if (!STMethods.FindInList(obj, ObjectsIsVisible))
                {
                    ObjectsIsVisible.Add(obj);
                }
            }
        }
    }
    
    public List<SelectableObject> EnemysInSensorRange()
    {
        if (ObjectsIsVisible.Count == 0) return null;
            
        List<SelectableObject> result = ObjectsIsVisible;
        
        for(int i = result.Count - 1; i >= 0; i--) 
        {
            if (manager.PlayersTeam[result[i].PlayerNum-1] == manager.PlayersTeam[Owner.PlayerNum-1])
            {
                result.Remove(result[i]);
            }
        }

        return result;
    }
    
    public List<SelectableObject> AllysInSensorRange()
    {
        if (ObjectsIsVisible.Count == 0) return null;
            
        List<SelectableObject> result = ObjectsIsVisible;
        
        for(int i = result.Count - 1; i >= 0; i--) 
        {
            if (manager.PlayersTeam[result[i].PlayerNum-1] != manager.PlayersTeam[Owner.PlayerNum-1])
            {
                result.Remove(result[i]);
            }
        }

        return result;
    }
    public List<SelectableObject> NeutralsInSensorRange()
    {
        if (ObjectsIsVisible.Count == 0) return null;
            
        List<SelectableObject> result = ObjectsIsVisible;
        
        for(int i = result.Count - 1; i >= 0; i--) 
        {
            if (result[i].PlayerNum > 0)
            {
                result.Remove(result[i]);
            }
        }

        return result;
    }
    
    public override void isCreated()
    {
        if (Owner.GetComponent<GunnerController>())
        {
            GunnerController Gunner = Owner.GetComponent<GunnerController>();
            Gunner.SenSS = this;
        }

        radius = Owner.SensorRange;
    }

}
