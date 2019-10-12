using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class STMethods
{
    /// <summary> Тревоги. </summary>

    public enum Alerts
    {
        RedAlert,
        YellowAlert,
        GreenAlert
        
    }
    
    /// <summary> Наведение на определённые системы. </summary>
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
    
    public enum ResourcesType
    {
        Titanium,
        Dilithium,
        Biomatter,
        Crew
    }
    
    public enum PlayerNum
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6,
        Player7,
        Player8
    }

    /// <summary> Тип проигрывания движения по рельсе. </summary>

    public enum RailPlaymode
    {
        Linear,
        Catmull
    }

    /// <summary> Обнаружение ближайшей объекта из списка. </summary>

    public static Transform NearestTransform(Transform[] transformList, Transform target)
    {
        float nearest = Mathf.Infinity;
        Transform closest = null;

        foreach (Transform all in transformList)
        {
            float distence = Vector3.Distance(all.position, target.position);

            if (distence < nearest)
            {
                nearest = distence;
                closest = all;
            }
        }

        return closest;
    }
    
    /// <summary> Обнаружение номера ближайшего объекта из списка. </summary>
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
    
    /// <summary> Найти в листе (GameObject). </summary>
    public static bool FindInList(GameObject obj, List<GameObject> list)
    {
        foreach (GameObject selObj in list)
        {
            if (selObj == obj)
                return true;
        }
        return false;
    }
    /// <summary> Найти в листе (AudioClip). </summary>
    public static bool FindInList(AudioClip obj, List<AudioClip> list)
    {
        foreach (AudioClip selObj in list)
        {
            if (selObj == obj)
                return true;
        }
        return false;
    }
    /// <summary> Найти в листе (Mobile). </summary>
    public static bool FindInList(Mobile obj, List<Mobile> list)
    {
        foreach (Mobile selObj in list)
        {
            if (selObj == obj)
                return true;
        }
        return false;
    }
    /// <summary> Найти в листе (SelectableObject). </summary>
    public static bool FindInList(SelectableObject obj, List<SelectableObject> list)
    {
        foreach (SelectableObject selObj in list)
        {
            if (selObj == obj)
                return true;
        }
        return false;
    }
    /// <summary> Удалить все пустые области из листа. </summary>
    public static void RemoveAllNullsFromList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (GameObjectExtensions.IsDestroyed(list[i]))
            {
                list.Remove(list[i]);
            }
        }
    }
}
[System.Serializable]
public class FireDegreesLockSystem
{
    /// <summary> Инверсировать X. </summary>
    public bool InvertX;

    /// <summary> Минимальный X. </summary>
    public float MinX;
    /// <summary> Максимальный X. </summary>
    public float MaxX;
    
    /// <summary> Инверсировать Y. </summary>
    public bool InvertY;

    /// <summary> Минимальный Y. </summary>
    public float MinY;
    /// <summary> Максимальный Y. </summary>
    public float MaxY;
} 
public static class GameObjectExtensions
{
    /// <summary>
    /// Checks if a GameObject has been destroyed.
    /// </summary>
    /// <param name="gameObject">GameObject reference to check for destructedness</param>
    /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
    public static bool IsDestroyed<T>(this T obj)
    {
        // UnityEngine overloads the == opeator for the GameObject type
        // and returns null when the object has been destroyed, but 
        // actually the object is still there but has not been cleaned up yet
        // if we test both we can determine if the object has been destroyed.
        return obj == null && !ReferenceEquals(obj, null);
    }
}