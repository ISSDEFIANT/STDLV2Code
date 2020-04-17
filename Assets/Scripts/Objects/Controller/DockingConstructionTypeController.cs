using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingConstructionTypeController : MonoBehaviour
{
    public List<ConstructionContract> AbleToConstruct = new List<ConstructionContract>();
    
    public StationDockingHubSS HubSS;

    public int HubCount;

    public List<ConstructionContract> ShipsInList = new List<ConstructionContract>();
    
    public FlagControll ExitFlag;

    private GameManager _gm;
    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.FindObjectOfType<GameManager>();
        
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
    }

    // Update is called once per frame
    void Update()
    {
        if (ShipsInList.Count > 0)
        {
            foreach (ConstructionContract _cc in ShipsInList)
            {
                for (int i = 0; i < HubCount; i++)
                {
                    if (HubSS.Hubs[i].ShipBuilding)
                    {
                        if (HubSS.Hubs[i].EnteringShip == null && !HubSS.Hubs[i].IsConstructing &&
                            HubSS.Hubs[i].constructingObject == null)
                        {
                            foreach (ConstructionContract ab in HubSS.Hubs[i].AbleToConstruct)
                            {
                                if (ab.Object == _cc.Object)
                                {
                                    Vector3 relRot = transform.position + (transform.rotation * HubSS.Hubs[i].ExitPoint) -
                                                     transform.position + (transform.rotation * HubSS.Hubs[i].StayPoint);

                                    HubSS.Hubs[i].constructingObject = _cc;
                                    GameObject anim = (GameObject) Instantiate(HubSS.Hubs[i].constructingObject.Animation,
                                        transform.position + (transform.rotation * HubSS.Hubs[i].StayPoint),
                                        Quaternion.LookRotation(relRot));
                                    anim.transform.parent = transform;
                                    HubSS.Hubs[i].constructingObject.Animation = anim;
                                    HubSS.Hubs[i].IsConstructing = true;       
                                }
                            }
                        }

                        if (HubSS.Hubs[i].IsConstructing && HubSS.Hubs[i].constructingObject != null)
                        {
                            if (HubSS.Hubs[i].constructingObject.ConstructionTime > 0)
                            {
                                HubSS.Hubs[i].constructingObject.ConstructionTime -= Time.deltaTime;
                            }
                            else
                            {
                                MoveCommand _nmc = new MoveCommand();
                                _nmc.command = "Move";
                                _nmc.targetVec = new List<Vector3>(); 
                                _nmc.targetVec.Add(ExitFlag.ExitFlag);
                                _nmc.Warp = false;
                                
                                UndockingCommand _uc = new UndockingCommand();                                
                                
                                _uc.commandAfterUndocking = _nmc;
                                _uc.DocingStation = HubSS.Owner;
                                _uc.Hub = HubSS.Hubs[i];
                                
                                Vector3 relRot = transform.position + (transform.rotation * HubSS.Hubs[i].ExitPoint) -
                                                 transform.position + (transform.rotation * HubSS.Hubs[i].StayPoint);
                                Destroy(HubSS.Hubs[i].constructingObject.Animation);
                                GameObject newShip = new GameObject();
                                newShip.transform.position = transform.position + (transform.rotation * HubSS.Hubs[i].StayPoint);
                                newShip.transform.rotation = Quaternion.LookRotation(relRot);
                                newShip.name = HubSS.Hubs[i].constructingObject.Object;
                                SelectableObject ns = STMethods.addObjectClass(HubSS.Hubs[i].constructingObject.Object, newShip);
                                ns.PlayerNum = HubSS.Owner.PlayerNum;
                                (ns as Mobile)._uc = _uc;
                                (ns as Mobile).ConstructedOnDock = true;
                                
                                HubSS.Hubs[i].constructingObject = null;
                                HubSS.Hubs[i].IsConstructing = false;

                                HubSS.Hubs[i].EnteringShip = ns as Mobile;
                                
                                _gm.UpdateList();
                                
                                RemoveConstructedShips();
                                return;
                            }
                        }
                        else
                        {
                            HubSS.Hubs[i].constructingObject = null;
                            HubSS.Hubs[i].IsConstructing = false;
                        }
                    }
                }
            }     
        }

        if (Input.GetKeyDown("1"))
        {
            BuildShip(0);
        }
    }

    void RemoveConstructedShips()
    {
        List<int> nulls = new List<int>();
        for (int i = 0; i < ShipsInList.Count; i++)
        {
            if (ShipsInList[i].ConstructionTime <= 0) nulls.Add(i);
        }

        foreach (int tar in nulls)
        {
            ShipsInList.RemoveAt(tar);
        }
    }

    public void BuildShip(int num)
    {
        if (AbleToConstruct[num].CanBeBuild(HubSS.Owner.PlayerNum))
        {
            AbleToConstruct[num].RemoveRes(HubSS.Owner.PlayerNum);
            ShipsInList.Add(STMethods.CreateCopy(AbleToConstruct[num]));
        }
    }
}

[System.Serializable]
public class ConstructionContract
{
    /// <summary> Что строим. </summary>
    public string Object;

    /// <summary> Анимация того, что строим. </summary>
    public GameObject Animation;

    /// <summary> Сколько Титана. </summary>
    public float TitaniumCost = 0;

    /// <summary> Сколько Дилития. </summary>
    public float DilithiumCost = 0;

    /// <summary> Сколько Биоматериала. </summary>
    public float BiomatterCost = 0;

    /// <summary> Сколько Людей. </summary>
    public float CrewCost = 0;

    /// <summary> Сколько Времени. </summary>
    public float ConstructionTime = 0;

    public bool CanBeBuild(int playerNum)
    {
        GameManager _gm = GameObject.FindObjectOfType<GameManager>();
        if (_gm.Players[playerNum - 1].Titanium >= TitaniumCost &&
            _gm.Players[playerNum - 1].Dilithium >= DilithiumCost &&
            _gm.Players[playerNum - 1].Biomatter >= BiomatterCost && _gm.Players[playerNum - 1].Crew >= CrewCost)
        {
            return true;
        }
        return false;
    }
    public void RemoveRes(int playerNum)
    {
        GameManager _gm = GameObject.FindObjectOfType<GameManager>();

        _gm.Players[playerNum - 1].Titanium -= TitaniumCost;
        _gm.Players[playerNum - 1].Dilithium -= DilithiumCost;
        _gm.Players[playerNum - 1].Biomatter -= BiomatterCost;
        _gm.Players[playerNum - 1].Crew -= CrewCost;
    }
}