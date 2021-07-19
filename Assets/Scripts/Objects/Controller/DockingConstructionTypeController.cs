using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DockingConstructionTypeController : MonoBehaviour
{
    public List<ConstructionContract> AbleToConstruct = new List<ConstructionContract>();
    
    public StationDockingHubSS HubSS;

    public int HubCount;

    public List<ConstructionContract> ShipsInList = new List<ConstructionContract>();
    
    public FlagControll ExitFlag;
    
    private HealthSystem _hs = null;

    public List<BuildAnimationScript> anims = new List<BuildAnimationScript>();
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<HealthSystem>()) _hs = gameObject.GetComponent<HealthSystem>();
        
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

            ExitFlag.ExitFlag = transform.position + (transform.rotation * HubSS.Hubs[0].ExitPoint)*2;
        }
    }

    void UpdateAnimations()
    {
        anims = GetComponentsInChildren<BuildAnimationScript>().ToList();
    }
    void DeactiveAnimations()
    {
        if (anims == null && anims.Count <= 0) return;
        foreach (BuildAnimationScript _bas in anims)
        {
            _bas.enabled = false;
            if (_bas.GetComponentInChildren<ParticleSystem>())
            {
                ParticleSystem _ps = _bas.GetComponentInChildren<ParticleSystem>();
                _ps.Stop();
            }
        }
    }
    void ActiveAnimations()
    {
        if (anims == null && anims.Count <= 0) return;
        foreach (BuildAnimationScript _bas in anims)
        {
            _bas.enabled = true;
            if (_bas.GetComponentInChildren<ParticleSystem>())
            {
                ParticleSystem _ps = _bas.GetComponentInChildren<ParticleSystem>();
                _ps.Play();
            }
        }
    }
    // Update is called once per frame
    private void LateUpdate()
    {
        if(ShipsInList.Count <= 0) return;
        UpdateAnimations();
        if (_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0)
        {
            if (anims != null && anims.Count > 0)
            {
                foreach (BuildAnimationScript _bas in anims)
                {
                    if (_bas.enabled)
                    {
                        DeactiveAnimations();
                        return;
                    }
                }
            }
        }
        else
        {
            if (anims != null && anims.Count > 0)
            {
                foreach (BuildAnimationScript _bas in anims)
                {
                    if (!_bas.enabled)
                    {
                        ActiveAnimations();
                        return;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0)
        {
            return;
        }
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
                                UndockingCommand _uc = new UndockingCommand();                                
                                
                                _uc.DocingStation = HubSS.Owner;
                                _uc.Hub = HubSS.Hubs[i];
                                _uc.AwaitingPoint = ExitFlag.ExitFlag;
                                
                                Vector3 relRot = transform.position + (transform.rotation * HubSS.Hubs[i].ExitPoint) -
                                                 transform.position + (transform.rotation * HubSS.Hubs[i].StayPoint);
                                Destroy(HubSS.Hubs[i].constructingObject.Animation);
                                GameObject newShip = new GameObject();
                                newShip.transform.position = transform.position + (transform.rotation * HubSS.Hubs[i].StayPoint);
                                newShip.transform.rotation = Quaternion.LookRotation(relRot);
                                newShip.name = HubSS.Hubs[i].constructingObject.Object;
                                SelectableObject ns = STMethods.addObjectClass(HubSS.Hubs[i].constructingObject.Object, newShip);
                                ns.PlayerNum = HubSS.Owner.PlayerNum;
                                ns.nameIndex = HubSS.Hubs[i].constructingObject.NameIndex;
                                (ns as Mobile)._uc = _uc;
                                (ns as Mobile).ConstructedOnDock = true;
                                
                                HubSS.Hubs[i].constructingObject = null;
                                HubSS.Hubs[i].IsConstructing = false;

                                HubSS.Hubs[i].EnteringShip = ns as Mobile;

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

    public void BuildShip(ConstructionContract cont)
    {
        if (ShipsInList.Count < 10)
        {
            if (cont.CanBeBuild(HubSS.Owner.PlayerNum) && cont.IndexList.Count < cont.MaxIndexCount)
            {
                int nameIndex = GetRandomIndex(cont.IndexList, cont.MaxIndexCount);
                cont.RemoveRes(HubSS.Owner.PlayerNum);
                ShipsInList.Add(STMethods.CreateCopy(cont));
                ShipsInList[ShipsInList.Count - 1].NameIndex = nameIndex;
                cont.IndexList.Add(nameIndex);
            }
        }
    }
    public void CanselShip(ConstructionContract cont)
    {
        cont.ReturnRes(HubSS.Owner.PlayerNum);
        foreach (DockingHub hub in HubSS.Hubs)
        {
            if (hub.constructingObject == cont)
            {
                Destroy(hub.constructingObject.Animation);
                hub.constructingObject = null;
                hub.IsConstructing = false;
            }   
        }
        cont.IndexList.Remove(cont.NameIndex);
        ShipsInList.Remove(cont);
    }

    private void OnDestroy()
    {
        if (ShipsInList != null && ShipsInList.Count > 0)
        {
            for (int i = ShipsInList.Count-1; i >= 0; i--)
            {
                ShipsInList[i].ReturnRes(HubSS.Owner.PlayerNum);
                foreach (DockingHub hub in HubSS.Hubs)
                {
                    if (hub.constructingObject == ShipsInList[i])
                    {
                        Destroy(hub.constructingObject.Animation);
                        hub.constructingObject = null;
                        hub.IsConstructing = false;
                    }
                }

                ShipsInList[i].IndexList.Remove(ShipsInList[i].NameIndex);
                ShipsInList.Remove(ShipsInList[i]);   
            }
        }
    }

    int GetRandomIndex(List<int> tar, int maxCount)
    {
        if (tar.Count < maxCount)
        {
            int num = UnityEngine.Random.Range(-1, maxCount);
            while (tar.Any(x => x == num))
            {
                num = UnityEngine.Random.Range(-1, maxCount);
            }
            return num;
        }
        else
        {
            return -2;
        }
    }
}