using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUnderConstruction : SelectableObject
{
    /// <summary> Контракт. </summary>
    public ConstructionContract Contract;
    
    /// <summary> Кто строит. </summary>
    public List<Mobile> Constucters = new List<Mobile>();

    public UnderConstructionManager _ucm;

    public float BuilderEfficiency = 25;


    public override void Awake()
    {
        base.Awake();
        stationSelectionType = true;

        healthSystem = true;

        _hs = gameObject.AddComponent<HealthSystem>();

        _hs.Owner = this;

        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.StationInProgress;
    }

    public override void Update()
    {
        base.Update();
        if(_ucm == null) return;
        foreach (MeshRenderer _mr in _ucm.ghost)
        {
            if (PlayerNum > 0)
            {
                _mr.material.SetColor("_InnerColor", new Color(GameManager.instance.Players[PlayerNum - 1].PlayerColor.r, GameManager.instance.Players[PlayerNum - 1].PlayerColor.g, GameManager.instance.Players[PlayerNum - 1].PlayerColor.b, 0.1f));
            }
            else
            {
                _mr.material.SetColor("_InnerColor", new Color(0.5f, 0.5f, 0.5f, 0.01f));
            }
        }
        
        foreach (BuildAnimationScript _bas in _ucm.anim)
        {
            if (_bas.amount < _hs.curHull / _hs.MaxHull)
            {
                _bas.amount = _hs.curHull / _hs.MaxHull;
            }
        }

        if (Constucters.Count > 0)
        {
            _hs.curHull += Time.deltaTime * BuilderEfficiency * Constucters.Count;
            List<int> indexes = new List<int>();
            for (int i = 0; i < Constucters.Count; i++)
            {
                if (!(Constucters[i].captain.curCommandInfo is BuildingCommand) || (Constucters[i].captain.curCommandInfo as BuildingCommand).proTarget != gameObject)
                {
                    indexes.Add(i);
                }
            }

            if (indexes.Count > 0)
            {
                foreach (int i in indexes)
                {
                    Constucters.RemoveAt(i);
                }
            }
        }

        if (_hs.curHull >= _hs.MaxHull)
        {
            foreach (Mobile builders in Constucters)
            {
                builders.captain.Command = Captain.PlayerCommand.None;
                builders.captain.curCommandInfo = null;
                return;
            }
            GameObject final = new GameObject();
            final.transform.position = transform.position;
            final.transform.rotation = transform.rotation;
            final.name = Contract.Object;
            SelectableObject ns = STMethods.addObjectClass(Contract.Object, final);
            ns.PlayerNum = PlayerNum;
            if (ns is Static) (ns as Static).Constructed = true;
            Destroy(gameObject);
        }
    }
}