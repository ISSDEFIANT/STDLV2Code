using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class SelectableObject : MonoBehaviour
{
    /// <summary> Номер игрок. </summary>
    public int PlayerNum;
    
    /// <summary> Кто контролирует судно. </summary>
    public STMethods.Races ControllingFraction;
    
    /// <summary> Выделяется одновременно только один. </summary>
    public bool stationSelectionType;
    /// <summary> Можно ли выделить. </summary>
    public bool selectionLock;
    /// <summary> Наведена ли на объект мышь. </summary>
    public bool isHovering;
    
    /// <summary> Выделен ли на объект мышь. </summary>
    public bool isSelected;
    
    /// <summary> Имеет ли систему жизней. </summary>
    public bool healthSystem;
    
    /// <summary> Тревога. </summary>
    public STMethods.Alerts Alerts;    
    /// <summary> Система жизней. </summary>
    [HideInInspector] public HealthSystem _hs;
    
    /// <summary> Радиус объекта. </summary>
    public float ObjectRadius;
    
    /// <summary> Зона сенсоров. </summary>
    public float SensorRange;
    
    /// <summary> Радиус орудий. </summary>
    public float WeaponRange;
    /// <summary> До скольки целей может атаковать одновременно. </summary>
    public int MaxAttackTargetCount;
    
    /// <summary> Список кораблей, которые защищают объект. </summary>
    public List<Mobile> ProtectionFleet = new List<Mobile>();

    /// <summary> Уничтожен. </summary>
    public bool destroyed;

    /// <summary> Модель. </summary>
    public GameObject model;
    
    /// <summary> Эффект ассимиляции и повреждений. </summary>
    public MaterialEffectController modelEffects;
    
    /// <summary> Эффект выделения. </summary>
    public GameObject SelectionEffectObject;

    /// <summary> Эффект Ассимиляции. </summary>
    public bool Assimilated = false;
    
    /// <summary> Капитан или же главный ИИ. </summary>
    public Captain captain = null;
    
    /// <summary> Компонент физики. </summary>
    public Rigidbody rigitBody;
    
    /// <summary> Список объектов, которые атакуют объект. </summary>
    public List<SelectableObject> ThreateningEnemyObjects = new List<SelectableObject>();
    
    private GameManager manager;
    
    /// <summary> Текущая высота применения приказа для данного объекта. </summary>
    public float ObjectOrdersHigh = 0;
    
    /// <summary> Можно ли разобрать. </summary>
    public bool canBeDeassembled;
    
    /// <summary> Анимация разборки. </summary>
    public GameObject DeassembledAnim;
    
    /// <summary> Время разборки. </summary>
    public float DeassebleTime = 0;
    
    /// <summary> Стоимость объекта в дилитии. </summary>
    public float DilithiumCost = 0;
    /// <summary> Стоимость объекта в титане. </summary>
    public float TitaniumCost = 0;
    /// <summary> Стоимость объекта в биоматерии. </summary>
    public float BiomatterCost = 0;
    /// <summary> Стоимость объекта в экипаже. </summary>
    public float CrewCost = 0;
    
    /// <summary> Иконка объекта. </summary>
    public Sprite ObjectIcon;
    
    /// <summary> Схема объекта. </summary>
    public Sprite[] ObjectBluePrint = new Sprite[9];
    
    /// <summary> Название объекта. </summary>
    public string ObjectName;
    /// <summary> Название объекта, отображающееся на модели. </summary>
    public string ObjectOnModelName;
    /// <summary> Идентификационный номер объекта. </summary>
    public string ObjectNCC;
    /// <summary> Индекс имени. </summary>
    public int nameIndex = -1;
    /// <summary> Лист индекса имени. </summary>
    public List<int> nameIndexList = null;
    
    /// <summary> Класс объекта. </summary>
    public string ObjectClass;
    
    /// <summary> Контроллер эффектов. </summary>
    public EffectManager effectManager;

    /// <summary> Всегда виден. </summary>
    public bool AlwaseVisible;
    /// <summary> Виден ли объект. </summary>
    public STMethods.Visibility isVisible;
    /// <summary> Был ли виден только что. </summary>
    private STMethods.Visibility oldIsVisible = STMethods.Visibility.Visible;
    /// <summary> Объект с шумом. </summary>
    public NoiseObject SelfNoise;

    /// <summary> Кому виден этот объект. </summary>
    public List<SensorSS> visibleFor = new List<SensorSS>();
    
    /// <summary> Тип рендера на карте. </summary>
    public GlobalMinimapMark.ShowingStats GlobalMinimapRender;
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32 (127, 127, 127, 127);
        Gizmos.DrawSphere(transform.position, ObjectRadius);
    }
    
    // Start is called before the first frame update

    public virtual void Awake()
    {   
        rigitBody = gameObject.AddComponent<Rigidbody>();
        rigitBody.useGravity = false;
        
        effectManager = gameObject.AddComponent<EffectManager>();
        effectManager.owner = this;
    }

    private void Start()
    {
        manager = GameManager.instance;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (ThreateningEnemyObjects.Any())
        {
            for (int i = ThreateningEnemyObjects.Count - 1; i >= 0; i--)
            {
                if (ThreateningEnemyObjects[i].destroyed)
                {
                    ThreateningEnemyObjects.RemoveAt(i);
                    continue;
                }
                if (ThreateningEnemyObjects[i].captain != null && ThreateningEnemyObjects[i].captain.Gunner != null && (ThreateningEnemyObjects[i].captain.Gunner.MainTarget != this ||
                    !ThreateningEnemyObjects[i].captain.Gunner.Targets.Any(x => x == this)))
                {
                    ThreateningEnemyObjects.RemoveAt(i);
                    continue;
                }

                if (ThreateningEnemyObjects[i].isThreatingFor() == null || (ThreateningEnemyObjects[i].isThreatingFor().PlayerNum != 0 && ThreateningEnemyObjects[i].isThreatingFor().PlayerNum == PlayerNum))
                {
                    ThreateningEnemyObjects.RemoveAt(i);
                }
            }
        }

        if (!AlwaseVisible)
        {
            if (oldIsVisible != isVisible)
            {
                SelfNoise.UpdateModel();
                oldIsVisible = isVisible;
            }

            if (visibleFor.Count > 0)
            {
                if (visibleFor.Count == 1 && visibleFor[0] == effectManager.sensor) return;
                UpdateVisibility();
            }
        }
        else
        {
            oldIsVisible = STMethods.Visibility.Visible;
            isVisible = STMethods.Visibility.Visible;
        }

        if (_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0)
        {
            PlayerNum = 0;
            ControllingFraction = STMethods.Races.None;
        }

        if (this is Static || this is Mobile)
        {
            if (effectManager.impulsEngine == null)
            {
                CollisionDetecting();
            }
            else
            {
                if (!effectManager.ImpulseEngineEffectActivity() || effectManager.impulsEngine.efficiency <= 0.125f)
                {
                    CollisionDetecting();    
                }
            }
        }
    }

    private void CollisionDetecting()
    {
        int layerMask = 1 << 9;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rigitBody.velocity, out hit, rigitBody.velocity.magnitude * 30, layerMask))
        {
            if (hit.transform.root.GetComponent<SelectableObject>())
            {
                hit.transform.root.GetComponent<SelectableObject>().ThreateningEnemyObjects.Add(this);
            }
        }
    }

    public SelectableObject isThreatingFor()
    {
        if (destroyed) return null;
        if (rigitBody.drag == 0)
        {
            int layerMask = 1 << 9;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rigitBody.velocity, out hit, rigitBody.velocity.magnitude * (10 + ObjectRadius), layerMask))
            {
                if (hit.transform.root.GetComponent<SelectableObject>())
                {
                    return this;
                }
                return null;
            }
        }
        return null;
    }

    private void OnDestroy()
    {
        manager.UpdateList();
        if (nameIndexList != null) nameIndexList.Remove(nameIndex);
    }

    public void FindInmodelElements()
    {
        if (model != null)
        {
            SelectionEffectObject = model.transform.Find("SelectionEffect").gameObject;
            modelEffects = model.GetComponentInChildren<MaterialEffectController>();

            if(AlwaseVisible) return;
            SelfNoise = gameObject.GetComponentInChildren<NoiseObject>();
            SelfNoise.owner = this;
        }
    }

    public void ShowSelectionEffect(float alpha)
    {
        MeshRenderer[] SelectionEffectParts = SelectionEffectObject.GetComponentsInChildren<MeshRenderer>().ToArray();
        Color PlayerColor;
        if (PlayerNum > 0)
        {
            if (alpha != 0)
            {
                PlayerColor = manager.Players[PlayerNum - 1].PlayerColor;
            }
            else
            {
                PlayerColor = Color.black;
            }
        }
        else
        {
            PlayerColor = Color.gray;
        }

        foreach (MeshRenderer _rp in SelectionEffectParts)
        {
            if (alpha == 0)
            {
                _rp.enabled = false;
            }
            else
            {
                _rp.material.SetColor("_TintColor", new Color(PlayerColor.r, PlayerColor.g, PlayerColor.b, alpha));
                _rp.enabled = true;
            }
        }
    }

    public void GetRandomIndex(List<int> tar, int maxCount)
    {
        if (tar.Count < maxCount)
        {
            int num = UnityEngine.Random.Range(-1, maxCount);
            while (tar.Any(x => x == num))
            {
                num = UnityEngine.Random.Range(-1, maxCount);
            }
            nameIndex = num;
            tar.Add(num);
        }
        else
        {
            nameIndex = -2;
        }
    }

    public void InitNames(NameSystem tar)
    {
        if (nameIndex < 0) return;
        ObjectName = tar.Names[nameIndex];
        ObjectOnModelName = tar.NamesOnHull[nameIndex];
        ObjectNCC = tar.NCC[nameIndex];

        foreach (ShipNameNCC textTar in gameObject.GetComponentsInChildren<ShipNameNCC>())
        {
            textTar.UpdateData();
        }
    }

    public void UpdateVisibility()
    {
        if (AlwaseVisible) return;
        if (visibleFor.Count == 0)
        {
            isVisible = STMethods.Visibility.Invisible;
            return;
        }
        bool cameraPlayer = false;
        int cameraTeam = 0;
        
        for (int i = 0; i < GameManager.instance.Players.Length; i++)
        {
            if (GameManager.instance.Players[i].CameraControll != null) cameraTeam = GameManager.instance.Players[i].TeamNum;
        }
        foreach (SensorSS sensores in visibleFor)
        {
            if (sensores.Owner.PlayerNum > 0 && GameManager.instance.Players[sensores.Owner.PlayerNum - 1].TeamNum == cameraTeam) cameraPlayer = true;
        }
        STMethods.Visibility curStatus = STMethods.Visibility.Invisible;

        if (!cameraPlayer)
        {
            isVisible = curStatus;
            return;
        }
        
        foreach (SensorSS sensores in visibleFor)
        {
            if (sensores == null) continue;
            if (sensores.Owner.PlayerNum > 0 && GameManager.instance.Players[sensores.Owner.PlayerNum-1].TeamNum == cameraTeam && !sensores.Owner.destroyed)
            {
                switch (sensores.Owner.Alerts)
                {
                    case STMethods.Alerts.GreenAlert:
                        if (Vector3.Distance(sensores.Owner.transform.position, transform.position) <=
                            (sensores.Owner.effectManager.SensorRange() + ObjectRadius + sensores.Owner.ObjectRadius))
                        {
                            curStatus = STMethods.Visibility.Visible;
                        }

                        break;
                    case STMethods.Alerts.YellowAlert:
                        if (Vector3.Distance(transform.position, sensores.Owner.transform.position) >
                            (sensores.Owner.effectManager.SensorRange() + ObjectRadius + sensores.Owner.ObjectRadius)
                            && Vector3.Distance(transform.position, sensores.Owner.transform.position) <
                            ((sensores.Owner.effectManager.SensorRange() * 1.25f) + ObjectRadius +
                             sensores.Owner.ObjectRadius))
                        {
                            if (curStatus != STMethods.Visibility.Visible) curStatus = STMethods.Visibility.NearNoise;
                            continue;
                        }

                        if (Vector3.Distance(transform.position, sensores.Owner.transform.position) <=
                            (sensores.Owner.effectManager.SensorRange() + ObjectRadius + sensores.Owner.ObjectRadius))
                        {
                            curStatus = STMethods.Visibility.Visible;
                        }

                        break;
                    case STMethods.Alerts.RedAlert:
                        if (Vector3.Distance(transform.position, sensores.Owner.transform.position) >
                            ((sensores.Owner.effectManager.SensorRange() * 1.25f) + ObjectRadius +
                             sensores.Owner.ObjectRadius)
                            && Vector3.Distance(transform.position, sensores.Owner.transform.position) <
                            ((sensores.Owner.effectManager.SensorRange() * 1.5f) + ObjectRadius +
                             sensores.Owner.ObjectRadius))
                        {
                            if (curStatus != STMethods.Visibility.Visible && curStatus != STMethods.Visibility.NearNoise) curStatus = STMethods.Visibility.FarNoise;
                            continue;
                        }

                        if (Vector3.Distance(transform.position, sensores.Owner.transform.position) >
                            (sensores.Owner.effectManager.SensorRange() + ObjectRadius + sensores.Owner.ObjectRadius)
                            && Vector3.Distance(transform.position, sensores.Owner.transform.position) <
                            ((sensores.Owner.effectManager.SensorRange() * 1.25f) + ObjectRadius +
                             sensores.Owner.ObjectRadius))
                        {
                            if (curStatus != STMethods.Visibility.Visible) curStatus = STMethods.Visibility.NearNoise;
                            continue;
                        }

                        if (Vector3.Distance(transform.position, sensores.Owner.transform.position) <=
                            (sensores.Owner.effectManager.SensorRange() + ObjectRadius + sensores.Owner.ObjectRadius))
                        {
                            curStatus = STMethods.Visibility.Visible;
                        }

                        break;
                }
            }
        }
        isVisible = curStatus;
    }
}