using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static : SelectableObject
{
    /// <summary> Построен. </summary>
    public bool Constructed;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        stationSelectionType = true;
    }

    // Update is called once per frame
    /// <summary> Включение модулей в работу. </summary>
    public override void Update()
    {
        base.Update();
    }
}