using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpController : MonoBehaviour
{
    public int curLevel = 1;

    public List<UpdatingContract> levels = new List<UpdatingContract>();

    public bool Updating;
    
    public UpdatingCentralSS UpSS;

    private LevelModelController _lmc;
    
    private HealthSystem _hs = null;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<HealthSystem>()) _hs = gameObject.GetComponent<HealthSystem>();
        _lmc = gameObject.GetComponentInChildren<LevelModelController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0)
        {
            return;
        }
        if (Updating)
        {
            _lmc.ChangeLevel(1-levels[0].curConstructionTime/levels[0].ConstructionTime,curLevel);
            if (levels[0].curConstructionTime > 0)
            {
                levels[0].curConstructionTime -= Time.deltaTime;
            }
            else
            {
                curLevel++;
                levels.RemoveAt(0);
                Updating = false;
            }
        }
    }

    public void UpdateObject()
    {
        if(levels[0].CanBeBuild(UpSS.Owner.PlayerNum))
        {
            levels[0].RemoveRes(UpSS.Owner.PlayerNum);
            Updating = true;
        }
    }
}
[System.Serializable]
public class UpdatingContract
{
    /// <summary> Название. </summary>
    public string Name;
    
    /// <summary> Картинка кнопочки. </summary>
    public Sprite ButtonImage;
    
    /// <summary> Иконка. </summary>
    public Sprite Icon;
    
    /// <summary> Сколько титана. </summary>
    public float TitaniumCost = 0;

    /// <summary> Сколько дилития. </summary>
    public float DilithiumCost = 0;

    /// <summary> Сколько биоматериала. </summary>
    public float BiomatterCost = 0;

    /// <summary> Сколько людей. </summary>
    public float CrewCost = 0;

    /// <summary> Сколько времени. </summary>
    public float ConstructionTime = 0;
    
    /// <summary> Сколько времени осталось. </summary>
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
    public void CanceldBuilding(int playerNum)
    {
        GameManager _gm = GameManager.instance;

        _gm.Players[playerNum - 1].Titanium += TitaniumCost;
        _gm.Players[playerNum - 1].Dilithium += DilithiumCost;
        _gm.Players[playerNum - 1].Biomatter += BiomatterCost;
        _gm.Players[playerNum - 1].Crew += CrewCost;
        
        curConstructionTime = ConstructionTime;
    }
}