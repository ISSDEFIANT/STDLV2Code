using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SciController : MonoBehaviour
{
    public STMethods.Races OpenTecTree;

    public bool Researching;

    public SciContract curResearch;

    public SciLabsSS system;
    
    private HealthSystem _hs = null;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<HealthSystem>()) _hs = gameObject.GetComponent<HealthSystem>();
        GameManager.instance.UpdateResearchList(this);
    }
    void OnDestroy()
    {
        GameManager.instance.UpdateResearchList(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0)
        {
            return;
        }
        if (Researching)
        {
            if (curResearch.curConstructionTime > 0)
            {
                curResearch.curConstructionTime -= Time.deltaTime;
            }
            else
            {
                GameManager.instance.ResearchReady(this, curResearch);
                Researching = false;
                curResearch = null;
            }
        }
    }

    public void BeginResearch(SciContract tar)
    {
        foreach (SciContract obj in GameManager.instance.Players[system.Owner.PlayerNum-1].ResearchesReady)
        {
            if (obj.GetType() == tar.GetType()) return;
        }
        foreach (SciContract obj in GameManager.instance.Players[system.Owner.PlayerNum-1].ResearchesInProgress)
        {
            if (obj.GetType() == tar.GetType()) return;
        }
        if (!tar.CanBeBuild(system.Owner.PlayerNum)) return;

        tar.RemoveRes(system.Owner.PlayerNum);
        GameManager.instance.Players[system.Owner.PlayerNum-1].ResearchesInProgress.Add(tar);
        curResearch = tar;
        Researching = true;
    }
    
    public void CanselResearch()
    {
        for (int j = 0; j < GameManager.instance.Players[system.Owner.PlayerNum-1].ResearchesInProgress.Count; j++)
        {
            if (GameManager.instance.Players[system.Owner.PlayerNum-1].ResearchesInProgress[j].GetType() == curResearch.GetType())
            {
                GameManager.instance.Players[system.Owner.PlayerNum-1].ResearchesInProgress.RemoveAt(j);
                j--;
            }
        }
        curResearch.curConstructionTime = curResearch.ConstructionTime;
        curResearch.ReturnRes(system.Owner.PlayerNum);
        curResearch = null;
        Researching = false;
    }
}
[System.Serializable]
public class SciContract
{
    /// <summary> Сколько титана. </summary>
    public virtual float TitaniumCost
    {
        get { return titanium; }
        set { titanium = value; }
    }

    /// <summary> Сколько дилития. </summary>
    public virtual float DilithiumCost
    {
        get { return dilithium; }
        set { dilithium = value; }
    }

    /// <summary> Сколько биоматериала. </summary>
    public virtual float BiomatterCost
    {
        get { return biomatter; }
        set { biomatter = value; }
    }

    /// <summary> Сколько людей. </summary>
    public virtual float CrewCost
    {
        get { return crew; }
        set { crew = value; }
    }

    /// <summary> Сколько времени. </summary>
    public virtual float ConstructionTime
    {
        get { return ctime; }
        set { ctime = value; }
    }
    
    /// <summary> Иконка. </summary>
    public virtual Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }
    
    /// <summary> Название исследования. </summary>
    public virtual string Name
    {
        get { return name; }
        set { name = value; }
    }
    
    /// <summary> Описание исследования. </summary>
    public virtual string Description
    {
        get { return description; }
        set { description = value; }
    }
    
    /// <summary> Текущее временя. </summary>
    public float curConstructionTime = 0;

    public bool CanBeBuild(int playerNum)
    {
        GameManager _gm = GameManager.instance;
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
        GameManager _gm = GameManager.instance;

        _gm.Players[playerNum - 1].Titanium -= TitaniumCost;
        _gm.Players[playerNum - 1].Dilithium -= DilithiumCost;
        _gm.Players[playerNum - 1].Biomatter -= BiomatterCost;
        _gm.Players[playerNum - 1].Crew -= CrewCost;
    }
    
    public void ReturnRes(int playerNum)
    {
        GameManager _gm = GameManager.instance;

        _gm.Players[playerNum - 1].Titanium += TitaniumCost;
        _gm.Players[playerNum - 1].Dilithium += DilithiumCost;
        _gm.Players[playerNum - 1].Biomatter += BiomatterCost;
        _gm.Players[playerNum - 1].Crew += CrewCost;
    }

    private float titanium = 0;
    private float dilithium = 0;
    private float crew = 0;
    private float biomatter = 0;
    private float ctime = 0;
    private Sprite icon = null;
    private string name = String.Empty;
    private string description = String.Empty;
}