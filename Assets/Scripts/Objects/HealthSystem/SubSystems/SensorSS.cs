using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Controllers;
using UnityEngine.UI;

public class SensorSS : SubSystem
{
    /// <summary> Радиус. </summary>
    public float radius;

    /// <summary> Все объекты в зоне видимости на текущей тревоге. </summary>
    public List<SelectableObject> ObjectsIsVisible = new List<SelectableObject>();
    
    /// <summary> Все объекты в максмальной зоне видимости. </summary>
    public List<SelectableObject> ObjectsIsMaxVisible = new List<SelectableObject>();

    /// <summary> Менеджер. </summary>
    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameManager.instance;
    }

    // Update is called once per frame
    public override void ChangeEfficiency()
    {
        base.ChangeEfficiency();
    }

    public override void Update()
    {
        base.Update();
        if (radius > 0)
        {
            Scan();
        }
    }

    void Scan()
    {
        foreach (SelectableObject obj in manager.SelectableObjects)
        {
            if (Vector3.Distance(obj.transform.position, Owner.transform.position) < (Owner.effectManager.SensorRange() +obj.ObjectRadius+Owner.ObjectRadius) && !obj.destroyed)
            {
                if (!ObjectsIsVisible.Any(x => x == obj))
                {
                    ObjectsIsVisible.Add(obj);
                }
            }
            else
            {
                if (ObjectsIsVisible.Any(x => x == obj))
                {
                    ObjectsIsVisible.Remove(obj);
                }
            }

            if (Vector3.Distance(obj.transform.position, Owner.transform.position) < (Owner.SensorRange +obj.ObjectRadius+Owner.ObjectRadius) && !obj.destroyed)
            {
                if (!ObjectsIsMaxVisible.Any(x => x == obj))
                {
                    ObjectsIsMaxVisible.Add(obj);
                    if (!Owner.destroyed)
                    {
                        obj.visibleFor.Add(this);
                        obj.UpdateVisibility();
                    }
                    else
                    {
                        if (obj.visibleFor.Any(x => x == obj))
                        {
                            obj.visibleFor.Remove(this);
                        }
                        if (obj.visibleFor.Count == 0)
                        {
                            obj.isVisible = STMethods.Visibility.Invisible;
                            obj.UpdateVisibility();
                        }
                    }
                }
            }
            else
            {
                if (ObjectsIsMaxVisible.Any(x => x == obj))
                {
                    obj.visibleFor.Remove(this);
                    if (obj.visibleFor.Count == 0)
                    {
                        obj.isVisible = STMethods.Visibility.Invisible;
                        obj.UpdateVisibility();
                    }
                    ObjectsIsMaxVisible.Remove(obj);
                }
            }
        }
    }

    public List<SelectableObject> EnemysInSensorRange()
    {
        if (ObjectsIsVisible.Count == 0) return new List<SelectableObject>();
            
        List<SelectableObject> result = ObjectsIsVisible;

        for (int i = result.Count - 1; i >= 0; i--)
        {
            if (Owner.PlayerNum == 0 || result[i].PlayerNum == 0 || manager.Players[result[i].PlayerNum - 1].TeamNum ==
                manager.Players[Owner.PlayerNum - 1].TeamNum)
            {
                result.Remove(result[i]);
            }
        }

        return result;
    }
    
    public List<SelectableObject> AllysInSensorRange()
    {
        if (ObjectsIsVisible.Count == 0) return new List<SelectableObject>();
            
        List<SelectableObject> result = ObjectsIsVisible;
        
        for(int i = result.Count - 1; i >= 0; i--) 
        {
            if (Owner.PlayerNum == 0 || result[i].PlayerNum != 0 && manager.Players[result[i].PlayerNum-1].TeamNum != manager.Players[Owner.PlayerNum-1].TeamNum)
            {
                result.Remove(result[i]);
            }
        }

        return result;
    }
    public List<SelectableObject> NeutralsInSensorRange()
    {
        if (ObjectsIsVisible.Count == 0) return new List<SelectableObject>();
            
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
        radius = Owner.SensorRange;
        
        Owner.effectManager.sensor = this;
        
        if (Owner.GetComponent<GunnerController>())
        {
            GunnerController Gunner = Owner.GetComponent<GunnerController>();
            Gunner.SenSS = this;
        }
    }
}
