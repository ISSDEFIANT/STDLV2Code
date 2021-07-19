using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatingCentralSS : SubSystem
{
    public LevelUpController _lvuc;

    private UpdatingContract[] _levels;
    // Start is called before the first frame update
    public override void isCreated()
    {
        if (!Owner.GetComponent<LevelUpController>())
        {
            _lvuc = Owner.gameObject.AddComponent<LevelUpController>();
            _lvuc.UpSS = this;
            
            _lvuc.levels.AddRange(_levels);
        }
    }

    void Awake()
    {
        Immortal = true;
        SubSystemMaxHealth = 100;
        SubSystemCurHealth = 100;
    }
    
    public SubSystem SetLevels(UpdatingContract[] levels, SelectableObject ow)
    {
        _levels = levels;
        Owner = ow;
        isCreated();
        return this;
    }
}
