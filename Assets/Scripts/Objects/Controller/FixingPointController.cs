using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FixingPointController : MonoBehaviour
{
    public StationDockingHubSS HubSS;

    public int HubCount;

    public List<Mobile> AwaitingShipsOnHub;

    public Vector3 AwaitingPoint;

    public FlagControll ExitFlag;
    // Start is called before the first frame update
    void Start()
    {
        if (!gameObject.GetComponent<FlagControll>())
        {
            ExitFlag = gameObject.AddComponent<FlagControll>();
        }
        else
        {
            ExitFlag = gameObject.GetComponent<FlagControll>();
        }
        
        if (HubSS.Hubs != null && HubSS.Hubs.Length > 0)
        {
            foreach (DockingHub _h in HubSS.Hubs)
            {
                if (_h.ShipFixing)
                {
                    HubCount++;
                } 
            }

            ExitFlag.ExitFlag = transform.position + (transform.rotation * HubSS.Hubs[0].ExitPoint);
        }
        AwaitingShipsOnHub = new List<Mobile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AwaitingShipsOnHub.Count > 0)
        {
            foreach (DockingHub _h in HubSS.Hubs)
            {
                if (_h.EnteringShip == null && !_h.IsConstructing)
                {
                    _h.EnteringShip = AwaitingShipsOnHub[0];
                    AwaitingShipsOnHub.RemoveAt(0);
                }
            }

            List<int> nulls = new List<int>();
            for (int i = 0; i < AwaitingShipsOnHub.Count; i++)
            {
                if (!(AwaitingShipsOnHub[i].captain.curCommandInfo is MiningCommand)) nulls.Add(i);
            }

            foreach (int tar in nulls)
            {
                AwaitingShipsOnHub.RemoveAt(tar);
            }
        }
    }

    public void DockingCall(Mobile _ship)
    {
        bool isEntering = false;
        for (int i = 0; i < HubCount; i++)
        {
            if (HubSS.Hubs[i].EnteringShip == _ship)
            {
                isEntering = true;
            }

            if (!isEntering)
            {
                if (HubSS.Hubs[i].EnteringShip == null && !HubSS.Hubs[i].IsConstructing)
                {
                    isEntering = true;
                    HubSS.Hubs[i].EnteringShip = _ship;
                }
            }
            else
            {
                break;
            }
        }

        if (!isEntering)
        {
            if(!AwaitingShipsOnHub.Any(x => x == _ship))AwaitingShipsOnHub.Add(_ship);
        }
    }
    
    public List<Vector3> WaitingPoints()
    {
        List<Vector3> list = new List<Vector3>();
        
		Vector3 waitingPointsOrigin = this.transform.position + (this.transform.rotation * AwaitingPoint);                  // начальная точка, откуда начинается очередь
		Vector3 rotationVector = Vector3.forward;     // нормаль, вдоль которой будут строиться корабли (длина 1 метр/единица)
		Vector3 waitingPointOffset = waitingPointsOrigin;                                   // временная точка которая как раз и поможет выстраивать корабли, при этом не нужно лишний раз лезть к предыдущему кораблю

		for (int i = 0; i < AwaitingShipsOnHub.Count; i++)      //цикл по кораблям - можно переписать на foreach
		{
			float shipRadius = AwaitingShipsOnHub[i].GetComponent<SelectableObject>().ObjectRadius;   // нам нужен только радиус корабля

			if (i == 0)                                 // если это первый в ожидании корабль
			{                                           // то говорим тчо он должен стоять на начальной точке
				waitingPointOffset = waitingPointsOrigin;
			}
			else                                        // иначе
			{                                           // от предыдущей границы мы отобдвинемся на радиус корабля
				waitingPointOffset = waitingPointOffset + rotationVector * (shipRadius + 5);
			}
			
            list.Add(new Vector3(waitingPointOffset.x, gameObject.transform.position.y, waitingPointOffset.z));

			waitingPointOffset = waitingPointOffset + rotationVector * (shipRadius + 5);    // отодвигаем границу ещё на один радиус ткущего корабля
		}

        return list.ToList();
    }
}
