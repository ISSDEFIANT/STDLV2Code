using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeassemblingPointController : MonoBehaviour
{
    public StationDockingHubSS HubSS;

    public int HubCount;

    public AwaitingPointController awaitingPoint;
    
    private HealthSystem _hs = null;

    // Start is called before the first frame update
    void Start()
    {   
        if (gameObject.GetComponent<HealthSystem>()) _hs = gameObject.GetComponent<HealthSystem>();
        
        if (HubSS.Hubs != null && HubSS.Hubs.Length > 0)
        {
            foreach (DockingHub _h in HubSS.Hubs)
            {
                if (_h.ShipFixing)
                {
                    HubCount++;
                } 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0)
        {
            if(awaitingPoint.AwaitingShipsOnHub.Count > 0) awaitingPoint.AwaitingShipsOnHub.Clear();
            return;
        }

        foreach (DockingHub _h in HubSS.Hubs)
        {
            if (awaitingPoint.AwaitingShipsOnHub.Count > 0)
            {
                if (_h.EnteringShip != null)
                {
                    if (awaitingPoint.AwaitingShipsOnHub.Any(x => x == _h.EnteringShip) &&
                        _h.EnteringShip.captain.ToStayPoint) awaitingPoint.AwaitingShipsOnHub.Remove(_h.EnteringShip);
                }

                if (_h.EnteringShip == null && !_h.IsConstructing)
                {
                    _h.EnteringShip = awaitingPoint.AwaitingShipsOnHub[0];
                    awaitingPoint.AwaitingShipsOnHub.RemoveAt(0);
                }
            }
            if (_h.EnteringShip != null)
            {
                if (_h.EnteringShip.captain != null && _h.EnteringShip.captain.curCommandInfo != null && (!(_h.EnteringShip.captain.curCommandInfo.command == "Deassembling") &&
                                                                                                          !(_h.EnteringShip.captain.curCommandInfo is FixingCommand) &&
                                                                                                          !(_h.EnteringShip.captain.curCommandInfo is UndockingCommand)))
                {
                    if(_h.EnteringShip.destroyed) _h.EnteringShip = null;
                    if(!(_h.EnteringShip.captain.curCommandInfo is MiningCommand))
                    {
                        _h.EnteringShip = null;
                    }
                    else
                    {
                        if (!_h.EnteringShip.captain.Miner.ToBase)
                        {
                            _h.EnteringShip = null;
                        }
                    }
                }
            }
        }
    }

    public void DockingCall(Mobile _ship)
    {
        if (_hs != null && _hs.MaxCrew > 0 && _hs.curCrew <= 0)
        {
            return;
        }
        bool isEntering = false;
        for (int i = 0; i < HubCount; i++)
        {
            if (HubSS.Hubs[i].EnteringShip == _ship)
            {
                isEntering = true;
            }
        }
        if (!isEntering)
        {
            if(!awaitingPoint.AwaitingShipsOnHub.Any(x => x == _ship))awaitingPoint.AwaitingShipsOnHub.Add(_ship);
        }
    }
}
