using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuilderConstructionSS : SubSystem
{
    public List<ConstructionContract> objectsToConstruct;
    public BuilderConstructionController _bcc;
    // Start is called before the first frame update
    void Awake()
    {
        Immortal = true;
        SubSystemMaxHealth = 100;
        SubSystemCurHealth = 100;
    }

    // Update is called once per frame
    public override void isCreated()
    {
        if (!Owner.GetComponent<BuilderConstructionController>())
        {
            _bcc = Owner.gameObject.AddComponent<BuilderConstructionController>();
            _bcc.system = this;
            if (objectsToConstruct.Count > 0)
            {
                _bcc.AbleToConstruct.AddRange(objectsToConstruct);
            }
        }
    }
    
    public SubSystem SetBuilderConstructionList(List<ConstructionContract> ableToConstruct, SelectableObject ow)
    {
        Owner = ow;
        objectsToConstruct = ableToConstruct.ToList();
        isCreated();
        return this;
    }
}