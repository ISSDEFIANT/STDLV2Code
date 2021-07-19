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
    public DeassemblingPointController _dpc;
    public AwaitingPointController _apc;
    public Vector3 AwayPoint, AwayPointRotation;
    // Start is called before the first frame update

    void Start()
    {
        foreach (DockingHub _dh in Hubs)
        {
            if (_dh.AbleToConstruct != null && _dh.AbleToConstruct.Count > 0)
            {
                foreach (ConstructionContract _c in _dh.AbleToConstruct)
                {
                    setIndexList(_c);
                }
            }
        }
    }
    void Awake()
    {
        Immortal = true;
        SubSystemMaxHealth = 100;
        SubSystemCurHealth = 100;
    }

    public override void isCreated()
    {
        if (!Owner.GetComponent<AwaitingPointController>())
        {
            _apc = Owner.gameObject.AddComponent<AwaitingPointController>();
            _apc.AwaitingPoint = AwayPoint;
            _apc.AwaitingPointRotation = AwayPointRotation;
        } 
        foreach (DockingHub _dh in Hubs)
        {
            if (_dh.ResourceUnloading)
            {
                if (!Owner.GetComponent<ResourceUnloadingController>())
                {
                    _ruc = Owner.gameObject.AddComponent<ResourceUnloadingController>();
                    _ruc.HubSS = this;
                    _ruc.awaitingPoint = _apc;
                }        
            }

            if (_dh.ShipFixing)
            {
                if (!Owner.GetComponent<FixingPointController>())
                {
                    _fpc = Owner.gameObject.AddComponent<FixingPointController>();
                    _fpc.HubSS = this;
                    _fpc.awaitingPoint = _apc;
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
                        _dcc.AbleToConstruct.AddRange(_dh.AbleToConstruct);
                    }
                }    
            }
            
            if (_dh.ShipDeassembling)
            {
                if (!Owner.GetComponent<DeassemblingPointController>())
                {
                    _dpc = Owner.gameObject.AddComponent<DeassemblingPointController>();
                    _dpc.HubSS = this;
                    _dpc.awaitingPoint = _apc;
                }    
            }
        }
    }
    
    public SubSystem SetHubPurpose(DockingHub[] NewHubs, Vector3 AwaitingPoint, Vector3 AwaitingPointRotation, SelectableObject ow)
    {
        Hubs = NewHubs.ToArray();
        Owner = ow;
        AwayPoint = AwaitingPoint;
        AwayPointRotation = AwaitingPointRotation;
        isCreated();
        return this;
    }

    public void setIndexList(ConstructionContract vessel)
    {
        switch (vessel.Object)
        {
            case "AtlantiaClass":
                vessel.IndexList = GameManager.instance.NamesIndexes.AtlantiaIndexes;
                break;
            
            case "DefiantClass":
                vessel.IndexList = GameManager.instance.NamesIndexes.DefianIndexes;
                break;
            case "NovaClass":
                vessel.IndexList = GameManager.instance.NamesIndexes.NovaIndexes;
                break;
        }
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
    public bool ShipDeassembling;

    public List<ConstructionContract> AbleToConstruct;

    public bool IsConstructing;
    public ConstructionContract constructingObject;
}
