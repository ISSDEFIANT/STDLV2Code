using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class STMethods
{
    static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }
    
    /// <summary> Оси. </summary>
    public enum Axis
    {
        XAxis,
        YAxis,
        ZAxis
    }
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
    
    public enum Races
    {
        Federation,
        Klingon,
        Romulan,
        Cardassian,
        S8472,
        Borg,
        None
    }

    /// <summary> Тип проигрывания движения по рельсе. </summary>

    public enum RailPlaymode
    {
        Linear,
        Catmull
    }
    
    /// <summary> Тревоги. </summary>
    public enum PlayerCameraState
    {
        Normal,
        OrderSetting,
        PatrolSetting,
        BuildingPlacement,
        FlagPlacing,
        Lock
    }
    
    /// <summary> Вид объекта. </summary>
    public enum Visibility
    {
        Invisible,
        FarNoise,
        NearNoise,
        Visible
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
    
    public static Transform[] NearestTransformSortList(Transform[] transformList, Transform target)
    {
        Transform[] output = transformList.OrderBy((d) => (d.position - target.position).sqrMagnitude).ToArray();
        return output;
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
        for (int i = list.Count-1; i >= 0; i--)
        {
            //if (list[i] != null) {}else{nulls.Add(i);}
            if (list[i].IsDestroyed()) {nulls.Add(i);}
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
        for (int i = 0; i < list.Count; i++)
        {
            if (max < list[i].ObjectRadius)
            {
                max = list[i].ObjectRadius;
            }
        }

        return max;
    }

    public static SelectableObject addObjectClass(string className, GameObject obj)
    {
        SelectableObject newComponent = null;
        switch (className)
        {
            case "AtlantiaClass":
                newComponent = obj.AddComponent<Atlantia>();
                break;
            case "AtlantiaClassCargo":
                //newComponent = obj.AddComponent<Atlantia>();
                break;
            
            case "DefiantClass":
                newComponent = obj.AddComponent<Defiant>();
                break;
            case "NovaClass":
                newComponent = obj.AddComponent<Nova>();
                break;
            case "SaberClass":
                newComponent = obj.AddComponent<Saber>();
                break;
            
            case "IntrepidClass":
                //newComponent = obj.AddComponent<Intrepid>();
                break;
            case "LunaClass":
                newComponent = obj.AddComponent<Luna>();
                break;
            case "SteamrunnerClass":
                newComponent = obj.AddComponent<Steamrunner>();
                break;

            case "AkiraClass":
                newComponent = obj.AddComponent<Akira>();
                break;
            case "PrometheusClass":
                newComponent = obj.AddComponent<Prometheus>();
                break;
            case "NebulaClass":
                newComponent = obj.AddComponent<Nebula>();
                break;
            case "GalaxyClass":
                newComponent = obj.AddComponent<Galaxy>();
                break;
            case "SovereignClass":
                newComponent = obj.AddComponent<Sovereign>();
                break;
            case "ExcaliburClass":
                newComponent = obj.AddComponent<Excalibur>();
                break;
            
            case "FedStarBase":
                newComponent = obj.AddComponent<FedStarBase>();
                break;
            case "FedStarBaseInProgress":
                newComponent = obj.AddComponent<FedStarBaseInProgress>();
                break;
            
            case "FedDrydock":
                newComponent = obj.AddComponent<FedDrydock>();
                break;
            case "FedDrydockInProgress":
                newComponent = obj.AddComponent<FedDrydockInProgress>();
                break;
            
            case "FedShipyard":
                newComponent = obj.AddComponent<FedAdvancedDrydock>();
                break;
            case "FedShipyardInProgress":
                newComponent = obj.AddComponent<FedShipyardInProgress>();
                break;
            
            case "FedSciStation":
                newComponent = obj.AddComponent<FedSciStation>();
                break;
            case "FedSciStationInProgress":
                newComponent = obj.AddComponent<FedSciStationInProgress>();
                break;
            
            case "FedMiningStation":
                newComponent = obj.AddComponent<FedMiningStation>();
                break;
            case "FedMiningStationInProgress":
                newComponent = obj.AddComponent<FedMiningStationInProgress>();
                break;
            
            case "FedOutpost":
                newComponent = obj.AddComponent<FedOutpost>();
                break;
            case "FedOutpostInProgress":
                newComponent = obj.AddComponent<FedOutpostInProgress>();
                break;
        }
        GameManager.instance.UpdateList();
        return newComponent;
    }

    public static ConstructionContract CreateCopy(ConstructionContract obj)
    {
        ConstructionContract _cc = new ConstructionContract();
        _cc.Icon = obj.Icon;
        _cc.ObjectName = obj.ObjectName;
        _cc.Object = obj.Object;
        _cc.ObjectUnderConstruction = obj.ObjectUnderConstruction;
        _cc.Animation = obj.Animation;
        _cc.Ghost = obj.Ghost;
        _cc.TitaniumCost = obj.TitaniumCost;
        _cc.DilithiumCost = obj.DilithiumCost;
        _cc.BiomatterCost = obj.BiomatterCost;
        _cc.CrewCost = obj.CrewCost;
        _cc.ConstructionTime = obj.ConstructionTime;
        _cc.MaxConstructionTime = obj.MaxConstructionTime;
        _cc.ObjectRadius = obj.ObjectRadius;
        _cc.ObjectCategory = obj.ObjectCategory;
        _cc.BuilderEfficiency = obj.BuilderEfficiency;
        _cc.NameIndex = obj.NameIndex;
        _cc.IndexList = obj.IndexList;
        _cc.MaxIndexCount = obj.MaxIndexCount;

        return _cc;
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
    public bool Warp = false;
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
public class GuardCommand : PlayerCommands
{    
    public List<Vector3> fleetPattern;
    public SelectableObject guardTarget;
}

public class PatrolCommand : PlayerCommands
{
    public List<Vector3> targetVec = new List<Vector3>();
}

public class MiningCommand : PlayerCommands
{
    public ResourceSource ResTar;
    public SelectableObject UnloadPoint;
    public bool ToBase;

    public STMethods.ResourcesType Type;
}

public class FixingCommand : PlayerCommands
{
    public SelectableObject FixingPoint;
}

public class UndockingCommand : PlayerCommands
{
    public PlayerCommands commandAfterUndocking;
    public Vector3 AwaitingPoint;
    public SelectableObject DocingStation;
    public DockingHub Hub;
}

public class BuildingCommand : PlayerCommands
{
    public GameObject proTarget = null;
    public ConstructionContract target;
    public Vector3 posVec;
    public Vector3 rotVec;

    public GameObject ghost;
}

public class SettingAbilityTargetCommand : PlayerCommands
{
    public SelectableObject target;
    public Skill ability;
}

[System.Serializable]
public class FleetControllingFields
{
    public List<SelectableObject>[] fleets = new List<SelectableObject>[10];
}

[System.Serializable]
public class ConstructionContract
{
    /// <summary> Иконка того, что строим. </summary>
    public Sprite Icon;
    
    /// <summary> Как называется. </summary>
    public string ObjectName;
    
    /// <summary> Что строим. </summary>
    public string Object;
    
    /// <summary> В процессе. </summary>
    public string ObjectUnderConstruction;

    /// <summary> Анимация того, что строим. </summary>
    public GameObject Animation;
    
    /// <summary> Призрак того что строим. </summary>
    public GameObject Ghost;

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
    
    /// <summary> Сколько масксимально времени. </summary>
    public float MaxConstructionTime = 0;
    
    /// <summary> Радиус объекта. </summary>
    public float ObjectRadius = 0;
    
    /// <summary> Категория объекта. </summary>
    public int ObjectCategory = 0;
    
    /// <summary> Эффективность строителя. </summary>
    public float BuilderEfficiency = 25;
    
    /// <summary> Индекс имени. </summary>
    public int NameIndex = -1;
    
    /// <summary> Лист индексов. </summary>
    public List<int> IndexList;
    
    /// <summary> Максимальное количество имён. </summary>
    public int MaxIndexCount;

    public bool CanBeBuild(int playerNum)
    {
        if (GameManager.instance.Players[playerNum - 1].Titanium >= TitaniumCost &&
            GameManager.instance.Players[playerNum - 1].Dilithium >= DilithiumCost &&
            GameManager.instance.Players[playerNum - 1].Biomatter >= BiomatterCost && GameManager.instance.Players[playerNum - 1].Crew >= CrewCost)
        {
            return true;
        }
        return false;
    }
    public void RemoveRes(int playerNum)
    {
        GameManager.instance.Players[playerNum - 1].Titanium -= TitaniumCost;
        GameManager.instance.Players[playerNum - 1].Dilithium -= DilithiumCost;
        GameManager.instance.Players[playerNum - 1].Biomatter -= BiomatterCost;
        GameManager.instance.Players[playerNum - 1].Crew -= CrewCost;
    }
    public void ReturnRes(int playerNum)
    {
        GameManager.instance.Players[playerNum - 1].Titanium += TitaniumCost;
        GameManager.instance.Players[playerNum - 1].Dilithium += DilithiumCost;
        GameManager.instance.Players[playerNum - 1].Biomatter += BiomatterCost;
        GameManager.instance.Players[playerNum - 1].Crew += CrewCost;
    }
}