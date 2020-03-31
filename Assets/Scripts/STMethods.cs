using System;
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
    
    public enum Races
    {
        Federation,
        Klingon,
        Romulan,
        Cardassian,
        S8472,
        Borg,
    }

    /// <summary> Тип проигрывания движения по рельсе. </summary>

    public enum RailPlaymode
    {
        Linear,
        Catmull
    }

    /// <summary> Обнаружение ближайшего из списка. </summary>
    public static SelectableObject NearestSelObj(SelectableObject tar, List<SelectableObject> list)
    {
        float nearest = Mathf.Infinity;
        SelectableObject closest = null;

        foreach (SelectableObject all in list)
        {
            float distence = Vector3.Distance(all.transform.position, tar.transform.position);

            if (distence < nearest)
            {
                nearest = distence;
                closest = all;
            }
        }

        return closest;
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
    
    
    /// <summary> Удалить все пустые области из листа. </summary>
    public static void RemoveAllNullsFromList<T>(List<T> list)
    {
        List<int> nulls = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) nulls.Add(i);
        }

        foreach (int tar in nulls)
        {
            list.RemoveAt(tar);
        }
    }
    
    static Vector3[] screenSpaceCorners;
	public static RendererBoundsInScreenSpaceInfo RendererBoundsInScreenSpace(Renderer r) {
		// This is the space occupied by the object's visuals
		// in WORLD space.
		Bounds bigBounds = r.bounds;

		if(screenSpaceCorners == null)
			screenSpaceCorners = new Vector3[8];

		Camera theCamera = Camera.main;

		// For each of the 8 corners of our renderer's world space bounding box,
		// convert those corners into screen space.
		screenSpaceCorners[0] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
		screenSpaceCorners[1] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );
		screenSpaceCorners[2] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
		screenSpaceCorners[3] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );
		screenSpaceCorners[4] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
		screenSpaceCorners[5] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );
		screenSpaceCorners[6] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
		screenSpaceCorners[7] = theCamera.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );

		// Now find the min/max X & Y of these screen space corners.
		float min_x = screenSpaceCorners[0].x;
		float min_y = screenSpaceCorners[0].y;
		float max_x = screenSpaceCorners[0].x;
		float max_y = screenSpaceCorners[0].y;

		for (int i = 1; i < 8; i++) {
			if(screenSpaceCorners[i].x < min_x) {
				min_x = screenSpaceCorners[i].x;
			}
			if(screenSpaceCorners[i].y < min_y) {
				min_y = screenSpaceCorners[i].y;
			}
			if(screenSpaceCorners[i].x > max_x) {
				max_x = screenSpaceCorners[i].x;
			}
			if(screenSpaceCorners[i].y > max_y) {
				max_y = screenSpaceCorners[i].y;
			}
		}

        RendererBoundsInScreenSpaceInfo output = new RendererBoundsInScreenSpaceInfo();

        output.MinX = min_x;
        output.MinY = min_y;
        output.MaxX = max_x;
        output.MaxY = max_y;

        return output;
    }
	
    public static SelectableObject MobileToSelectableObject(Mobile m)
    {
        return m as SelectableObject;
    }
    
    public static float MaxRadiusInFleet(List<Mobile> list)
    {
        float max = 0;
        for (int i = 0; i < list.Count-1; i++)
        {
            if (max < list[i].ObjectRadius)
            {
                max = list[i].ObjectRadius;
            }
        }

        return max;
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
[System.Serializable]
public class AttackPatternProbability 
{
    /// <summary> Вероятность паттерна Alpha. </summary>
    public float AlphaProbability;
    /// <summary> Вероятность паттерна Beta. </summary>
    public float BetaProbability;
    /// <summary> Вероятность паттерна Gamma. </summary>
    public float GammaProbability;
}

[System.Serializable]
public class RendererBoundsInScreenSpaceInfo 
{
    /// <summary> Минимальный X. </summary>
    public float MinX;
    /// <summary> Минимальный Y. </summary>
    public float MinY;
    /// <summary> Максимальный X. </summary>
    public float MaxX;
    /// <summary> Максимальный Y. </summary>
    public float MaxY;
}

[System.Serializable]
public class LightBlinkColors 
{
    /// <summary> Нормальный цвет. </summary>
    public Color NormalColor;
    /// <summary> Цвет мерцания при повреждении. </summary>
    public Color DamagedColor;
    /// <summary> Цвет мерцания при сильных повреждениях. </summary>
    public Color HeavyDamagedColor;
}
[System.Serializable]

public class LightControlElement
{
    public bool hasSystem;
    public SubSystem SubSystem;
    public List<Renderer> Parts;
    public List<Light> Lights;
    public LightBlinkColors Colors;
    public LightBlinkColors BorgColors;

    public float timer = 1f;
    public float curMaxtimer;
    public float maxTimer;
}

public class PlayerCommands 
{
    /// <summary> Команда. </summary>
    public string command;
}

public class MoveCommand : PlayerCommands
{    
    public List<Vector3> targetVec;
}
public class AttackCommand : PlayerCommands
{    
    public SelectableObject attackTarget;
}
public class HideCoverCommand : PlayerCommands
{
    //coverignship в случае, если это команда спрятаться - корабль, за который спрятаться (который прикрывает), если же это команда прикрыть, то это тот, кого надо прикрыть.
    public SelectableObject coverignShip;
    public SelectableObject ThreateningEnemyObjects;
    //Команда спрятаться или прикрыть относится к вспомогательным, и если есть главная, нужно иметь возможность к ней вернуться.
    public PlayerCommands MainCommand;
}
[System.Serializable]
public class FleetControllingFields
{
    public List<SelectableObject>[] fleets = new List<SelectableObject>[10];
}
public class GuardCommand : PlayerCommands
{    
    public List<Vector3> fleetPattern;
    public SelectableObject guardTarget;
}