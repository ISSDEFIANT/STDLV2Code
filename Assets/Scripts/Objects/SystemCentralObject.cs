using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCentralObject : SelectableObject
{
    public float gravityRadius;
    public float gravitySlow;
    public override void Awake()
    {
        base.Awake();
        stationSelectionType = true;

        healthSystem = false;
        AlwaseVisible = true;
        
        rigitBody.isKinematic = true;
        
        ControllingFraction = STMethods.Races.None;

        GlobalMinimapRender = GlobalMinimapMark.ShowingStats.Star;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (SelectableObject obj in GameManager.instance.SelectableObjects)
        {
            if (obj != this && (obj.rigitBody.drag == 0 && !obj.rigitBody.isKinematic))
            {
                obj.rigitBody.AddForce((transform.position - obj.transform.position).normalized * obj.rigitBody.mass /
                                       (gravitySlow * (Vector3.Distance(obj.transform.position, transform.position) /
                                                       gravityRadius)));
            }
        }
    }
    void OnCollisionStay(Collision collision)
    {
        SelectableObject otherSel = collision.transform.GetComponent<SelectableObject>();

        otherSel.destroyed = true;
        if (otherSel.healthSystem)
        {
            otherSel._hs.curHull = 1;
            otherSel._hs.ApplyDamage(1,STMethods.AttackType.NormalAttack,Vector3.zero,true);
        }
        else
        {
            Destroy(otherSel.gameObject);
        }
    }

    public Vector3 GravityForce(SelectableObject obj)
    {
        return (transform.position - obj.transform.position).normalized * obj.rigitBody.mass /
               (gravitySlow * (Vector3.Distance(obj.transform.position, transform.position) /
                               gravityRadius));
    }
}