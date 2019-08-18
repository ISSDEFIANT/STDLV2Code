using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class STMethods
{
    public enum Alerts
    {
        RedAlert,
        YellowAlert,
        GreenAlert
        
    }
    public enum AttackType
    {
        NormalAttack,
        
        PrimaryWeaponSystemAttack,
        SecondaryWeaponSystemAttack,
        ImpulseSystemAttack,
        WarpEngingSystemAttack,
        WarpCoreAttack,
        LifeSupportSystemAttack,
        SensorsSystemAttack,
        TractorBeamSystemAttack
    }

    public enum RailPlaymode
    {
        Linear,
        Catmull
    }

    public static Transform NearestTransform(Transform[] transformList, Transform target)
    {
        float nearest = Mathf.Infinity;
        Transform closest = null;

        foreach (Transform all in transformList)
        {
            float distence = Vector3.Distance(all.position, target.position);//(all.position - target.position).sqrMagnitude;

            if (distence < nearest)
            {
                nearest = distence;
                closest = all;
            }
        }

        return closest;
    }

    public static int NearestTransformInt(Transform[] list, Transform target)
    {
        float nearest = Mathf.Infinity;
        int num = 0;
        for (int i = 0; i < list.Length-1; i++)
        {
            if (Vector3.Distance(list[i].position, target.position) < nearest)
            {
                num = i;
                nearest = Vector3.Distance(list[i].position, target.position);
            }
        }

        return num;
    }
    
    public static bool FindInList(GameObject obj, List<GameObject> list)
    {
        foreach (GameObject selObj in list)
        {
            if (selObj == obj)
                return true;
        }
        return false;
    }
    public static bool FindInList(AudioClip obj, List<AudioClip> list)
    {
        foreach (AudioClip selObj in list)
        {
            if (selObj == obj)
                return true;
        }
        return false;
    }
}
[System.Serializable]
public class FireDegreesLockSystem
{
    public bool InvertX;

    public float MinX;
    public float MaxX;

    public bool InvertY;

    public float MinY;
    public float MaxY;
} 