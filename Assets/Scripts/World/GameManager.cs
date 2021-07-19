using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary> Игроки. </summary>
    public PlayerInfo[] Players = new PlayerInfo[8];

    private PlayerCameraControll[] cameras;
    
    public List<SelectableObject> SelectableObjects;

    public static GameManager instance;

    public TecnoTrees tecnoTrees;

    public NameIndexes NamesIndexes;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        NamesIndexes = new NameIndexes();
        UpdateList();
        
        cameras = FindObjectsOfType<PlayerCameraControll>().ToArray();

        foreach (PlayerInfo player in Players)
        {
            switch (player.race)
            {
                case STMethods.Races.Federation:
                    player.GroupsNames = new string[10]{"Group Alpha", "Group Beta", "Group Gamma", "Group Delta", "Group Epsilon", "Group Zeta", "Group Eta", "Group Theta", "Group Iota", "Group Kappa"};        
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cameras != null && cameras.Length > 0)
        {
            if (SelectableObjects != null && SelectableObjects.Count > 0)
            {
                foreach (SelectableObject _so in SelectableObjects)
                {
                    if (_so.isSelected)
                    {
                        bool isSelected = false;
                        foreach (PlayerCameraControll _pc in cameras)
                        {
                            if (_pc.SelectionList.Any(x => x == _so))
                            {
                                isSelected = true;
                            }
                        }

                        if (!isSelected)
                        {
                            _so.isSelected = false;
                            _so.ShowSelectionEffect(0);
                        }
                    }
                }
            }
        }
    }

    public void UpdateList()
    {
        SelectableObjects = GameObject.FindObjectsOfType<SelectableObject>().ToList();
    }

    public void UpdateResearchList(SciController noSciModule)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (i == noSciModule.system.Owner.PlayerNum - 1)
            {
                bool fedTec = false;
                bool kliTec = false;
                bool romTec = false;
                bool carTec = false;
                bool borgTec = false;
                bool unTec = false;
                foreach (SelectableObject obj in SelectableObjects)
                {
                    if (obj.GetComponent<SciController>())
                    {
                        SciController con = obj.GetComponent<SciController>();
                        if (con.OpenTecTree == STMethods.Races.Federation)
                        {
                            fedTec = true;
                        }
                        if (con.OpenTecTree == STMethods.Races.Klingon)
                        {
                            kliTec = true;
                        }
                        if (con.OpenTecTree == STMethods.Races.Romulan)
                        {
                            romTec = true;
                        }
                        if (con.OpenTecTree == STMethods.Races.Cardassian)
                        {
                            carTec = true;
                        }
                        if (con.OpenTecTree == STMethods.Races.Borg)
                        {
                            borgTec = true;
                        }
                        if (con.OpenTecTree == STMethods.Races.S8472)
                        {
                            unTec = true;
                        }
                    }
                }

                if (fedTec)
                {
                    addTecnology(i,tecnoTrees.FedTree);
                    Players[i].fedTecTree = true;
                }
                else
                {
                    removeTecnology(i,tecnoTrees.FedTree);
                    Players[i].fedTecTree = false;
                }
                
                if (kliTec)
                {
                    addTecnology(i,tecnoTrees.KliTree);
                    Players[i].kliTecTree = true;
                }
                else
                {
                    removeTecnology(i,tecnoTrees.KliTree);
                    Players[i].kliTecTree = false;
                }
                
                if (romTec)
                {
                    addTecnology(i,tecnoTrees.RomTree);
                    Players[i].romTecTree = true;
                }
                else
                {
                    removeTecnology(i,tecnoTrees.RomTree);
                    Players[i].romTecTree = false;
                }
                
                if (carTec)
                {
                    addTecnology(i,tecnoTrees.CarTree);
                    Players[i].carTecTree = true;
                }
                else
                {
                    removeTecnology(i,tecnoTrees.CarTree);
                    Players[i].carTecTree = false;
                }
                
                if (borgTec)
                {
                    addTecnology(i,tecnoTrees.BorgTree);
                    Players[i].borgTecTree = true;
                }
                else
                {
                    removeTecnology(i,tecnoTrees.BorgTree);
                    Players[i].borgTecTree = false;
                }
                
                if (unTec)
                {
                    addTecnology(i,tecnoTrees.UnTree);
                    Players[i].undineTecTree = true;
                }
                else
                {
                    removeTecnology(i,tecnoTrees.UnTree);
                    Players[i].undineTecTree = false;
                }

                if (Players[i].CameraControll != null)
                {
                    Players[i].CameraControll.GetComponent<PlayerCameraControll>().globalInterface.scienceSubModule.UpdateSciList();
                }
            }
        }
    }

    void addTecnology(int PlayerNum, SciContract[] side)
    {
        foreach (SciContract _sc in side)
        {
            if (!Players[PlayerNum].AbleToResearch.Any(x => x == _sc))
            {
                Players[PlayerNum].AbleToResearch.Add(_sc);
            }
        }
    }
    void removeTecnology(int PlayerNum, SciContract[] side)
    {
        foreach (SciContract _sc in side)
        {
            if (Players[PlayerNum].AbleToResearch.Any(x => x == _sc))
            {
                Players[PlayerNum].AbleToResearch.Remove(_sc);
            }
        }
    }

    public void ResearchReady(SciController noSciModule, SciContract research)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (i == noSciModule.system.Owner.PlayerNum - 1)
            {
                if (!researchTypeInList(Players[i].ResearchesReady, research))
                {
                    Players[i].ResearchesReady.Add(research);
                    
                    for (int j = 0; j < Players[i].ResearchesInProgress.Count; j++)
                    {
                        if (Players[i].ResearchesInProgress[j].GetType() == research.GetType())
                        {
                            Players[i].ResearchesInProgress.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
        }
    }

    bool researchTypeInList(List<SciContract> list, SciContract type)
    {
        bool inList = false;
        foreach (SciContract tar in list)
        {
            if (tar.GetType() == type.GetType())
            {
                inList = true;
            }
        }

        return inList;
    }
}
[System.Serializable]
public class PlayerInfo
{
    public string PlayerName;
    
    public int TeamNum;
    public STMethods.Races race;
    public GameObject CameraControll;
    public Color PlayerColor;

    public float Titanium;
    public float Dilithium;
    public float Biomatter;
    public float Crew;

    public bool fedTecTree;
    public bool kliTecTree;
    public bool romTecTree;
    public bool carTecTree;
    public bool borgTecTree;
    public bool undineTecTree;
    public List<SciContract> AbleToResearch = new List<SciContract>();
    public List<SciContract> ResearchesReady = new List<SciContract>();
    public List<SciContract> ResearchesInProgress = new List<SciContract>();
    
    public string[] GroupsNames;
}
[System.Serializable]
public class TecnoTrees
{
    public SciContract[] FedTree = new SciContract[1]{new TestResearch1()};
    public SciContract[] KliTree;
    public SciContract[] RomTree;
    public SciContract[] CarTree;
    public SciContract[] BorgTree;
    public SciContract[] UnTree;
}
public class NameIndexes
{
    public List<int> AtlantiaIndexes = new List<int>();
    
    public List<int> DefianIndexes = new List<int>();
    public List<int> NovaIndexes = new List<int>();
    public List<int> SaberIndexes = new List<int>();
    
    public List<int> IntrepidIndexes = new List<int>();
    public List<int> LunaIndexes = new List<int>();
    public List<int> SteamrunnerIndexes = new List<int>();
    
    public List<int> AkiraIndexes = new List<int>();
    public List<int> PrometheusIndexes = new List<int>();
    public List<int> NebulaIndexes = new List<int>();
    public List<int> GalaxyIndexes = new List<int>();
    public List<int> SovereignIndexes = new List<int>();
    public List<int> ExcaliburIndexes = new List<int>();
}