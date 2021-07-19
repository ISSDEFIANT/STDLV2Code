using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AwaitingPointController : MonoBehaviour
{
    public Vector3 AwaitingPoint;
    public Vector3 AwaitingPointRotation;
    
    public List<Mobile> AwaitingShipsOnHub;

    private HealthSystem _hs = null;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<HealthSystem>()) _hs = gameObject.GetComponent<HealthSystem>();
        AwaitingShipsOnHub = new List<Mobile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0)
        {
            if(AwaitingShipsOnHub.Count > 0) AwaitingShipsOnHub.Clear();
            return;
        }
        if (AwaitingShipsOnHub.Count > 0)
        {
            List<int> nulls = new List<int>();
            for (int i = 0; i < AwaitingShipsOnHub.Count; i++)
            {
                if (AwaitingShipsOnHub[i].destroyed || (!(AwaitingShipsOnHub[i].captain.curCommandInfo.command == "Deassembling") &&
                    !(AwaitingShipsOnHub[i].captain.curCommandInfo is FixingCommand) &&
                    !(AwaitingShipsOnHub[i].captain.curCommandInfo is MiningCommand))) nulls.Add(i);
            }

            if (nulls.Count > 0)
            {
                foreach (int tar in nulls)
                {
                    AwaitingShipsOnHub.RemoveAt(tar);
                }
            }
        }
    }
    
    public List<Vector3> WaitingPoints(Mobile enteringShip = null)
    {
        List<Vector3> list = new List<Vector3>();
        
        Vector3 waitingPointsOrigin =  this.transform.position + (this.transform.rotation * AwaitingPoint);                 // начальная точка, откуда начинается очередь
        Vector3 rotationVector = AwaitingPointRotation;     // нормаль, вдоль которой будут строиться корабли (длина 1 метр/единица)
        Vector3 waitingPointOffset = waitingPointsOrigin;                                   // временная точка которая как раз и поможет выстраивать корабли, при этом не нужно лишний раз лезть к предыдущему кораблю

        for (int i = 0; i < AwaitingShipsOnHub.Count; i++)      //цикл по кораблям - можно переписать на foreach
        {
            float shipRadius = AwaitingShipsOnHub[i].GetComponent<SelectableObject>().ObjectRadius;   // нам нужен только радиус корабля

            if (i == 0)                                 // если это первый в ожидании корабль
            {                                           // то говорим тчо он должен стоять на начальной точке
                if (enteringShip == null)
                {
                    waitingPointOffset = waitingPointsOrigin;
                }
                else
                {
                    waitingPointOffset = waitingPointOffset + rotationVector * (enteringShip.ObjectRadius + 5);
                }
            }
            else                                        // иначе
            {                                           // от предыдущей границы мы отобдвинемся на радиус корабля
                if (enteringShip == null)
                {
                    waitingPointOffset = waitingPointOffset + rotationVector * (shipRadius + 5);
                }
                else
                {
                    waitingPointOffset = waitingPointOffset + rotationVector * (shipRadius + 5) + rotationVector * (enteringShip.ObjectRadius + 5);
                }
            }
			
            list.Add(new Vector3(waitingPointOffset.x, gameObject.transform.position.y, waitingPointOffset.z));

            waitingPointOffset = waitingPointOffset + rotationVector * (shipRadius + 5);    // отодвигаем границу ещё на один радиус ткущего корабля
        }

        return list.ToList();
    }
}
