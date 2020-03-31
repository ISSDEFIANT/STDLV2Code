using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

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
    public bool Assimilated;
    
    /// <summary> Капитан или же главный ИИ. </summary>
    public Captain captain;
    
    /// <summary> Компонент физики. </summary>
    public Rigidbody rigitBody;
    
    /// <summary> Список объектов, которые атакуют объект. </summary>
    public List<SelectableObject> ThreateningEnemyObjects = new List<SelectableObject>();
    
    private GameManager manager;
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32 (127, 127, 127, 127);
        Gizmos.DrawSphere(transform.position, ObjectRadius);
    }
    
    // Start is called before the first frame update

    public virtual void Awake()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
        
        rigitBody = gameObject.AddComponent<Rigidbody>();
        rigitBody.useGravity = false;
    }

    // Update is called once per frame
    public virtual void Update()
    { 
        
    }

    private void OnDestroy()
    {
        manager.UpdateList();
    }

    public void FindInmodelElements()
    {
        if (model != null)
        {
            SelectionEffectObject = model.transform.Find("SelectionEffect").gameObject;
            modelEffects = model.GetComponentInChildren<MaterialEffectController>();
        }
    }

    public void ShowSelectionEffect(float alpha)
    {
        MeshRenderer[] SelectionEffectParts = SelectionEffectObject.GetComponentsInChildren<MeshRenderer>().ToArray();
        Color PlayerColor = manager.Players[PlayerNum - 1].PlayerColor;

        foreach (MeshRenderer _rp in SelectionEffectParts)
        {
            _rp.material.SetColor("_TintColor", new Color(PlayerColor.r, PlayerColor.g, PlayerColor.b, alpha));
        }
    }
}