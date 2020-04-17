using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationDockingHubSS : SubSystem
{
    public DockingHub[] Hubs;

    public ResourceUnloadingController _ruc;
    public FixingPointController _fpc;
    public DockingConstructionTypeController _dcc;
    public Vector3 AwayPoint;
    // Start is called before the first frame update
    void Awake()
    {
        Immortal = true;
        SubSystemMaxHealth = 100;
        SubSystemCurHealth = 100;
    }

    public override void isCreated()
    {
        foreach (DockingHub _dh in Hubs)
        {
            if (_dh.ResourceUnloading)
            {
                if (!Owner.GetComponent<ResourceUnloadingController>())
                {
                    _ruc = Owner.gameObject.AddComponent<ResourceUnloadingController>();
                    _ruc.HubSS = this;
                    _ruc.AwaitingPoint = AwayPoint;
                }        
            }

            if (_dh.ShipFixing)
            {
                if (!Owner.GetComponent<FixingPointController>())
                {
                    _fpc = Owner.gameObject.AddComponent<FixingPointController>();
                    _fpc.HubSS = this;
                    _fpc.AwaitingPoint = AwayPoint;
                }    
            }
            
            if (_dh.ShipBuilding)
            {
                if (!Owner.GetComponent<DockingConstructionTypeController>())
                {
                    _dcc = Owner.gameObject.AddComponent<DockingConstructionTypeController>();
                    _dcc.HubSS = this;
                    if (_dh.AbleToConstruct.Count > 0)
                    {
                        foreach (ConstructionContract cc in _dh.AbleToConstruct)
                        {
                            _dcc.AbleToConstruct.Add(cc);   
                        }
                    }
                }    
            }
        }
    }
    
    public SubSystem SetHubPurpose(DockingHub[] NewHubs, Vector3 AwaitingPoint, SelectableObject ow)
    {
        Hubs = NewHubs.ToArray();
        Owner = ow;
        AwayPoint = AwaitingPoint;
        isCreated();
        return this;
    }
}
[System.Serializable]
public class DockingHub
{
    public Vector3 EnterPoint;
    public Vector3 StayPoint;
    public Vector3 ExitPoint;

    public Mobile EnteringShip;
    
    public bool ResourceUnloading;
    public bool ShipFixing;
    public bool ShipBuilding;

    public List<ConstructionContract> AbleToConstruct;

    public bool IsConstructing;
    public ConstructionContract constructingObject;
}
