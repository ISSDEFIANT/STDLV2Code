using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningController : MonoBehaviour
{
    //Точка своза ресурсов
    /// <summary> Максимальное количество ресурсво. </summary>
    public float MaxResources;

    /// <summary> Текущее количество ресурсов. </summary>
    public float curResources;

    /// <summary> Текущий тип ресурсов. </summary>
    public STMethods.ResourcesType curResourcesType;

    /// <summary> Может ли добывать титан. </summary>
    public bool Titanium;

    /// <summary> Может ли добывать дилитий. </summary>
    public bool Dilithium;

    /// <summary> Может ли добывать биоматерию. </summary>
    public bool Biomatter;

    /// <summary> Возвращается на базу. </summary>
    public bool ToBase;

    /// <summary> Подсистема. </summary>
    public MiningBeamSS System;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Mine(ResourceSource Target)
    {
        if (curResources > 0)
        {
            if (Target.type != curResourcesType) return;
        }
        else
        {
            curResourcesType = Target.type;
        }
        if (Target.curResources > 0 && curResources < MaxResources && curResourcesType == Target.type)
        {
            if (!Target.isInfinite)
            {
                Target.curResources -= Time.deltaTime * 10;
            }

            curResources += Time.deltaTime * 10;
            
            System.ActiveBeam(Target.transform);
        }
    }
}