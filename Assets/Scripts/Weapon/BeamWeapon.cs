using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamWeapon : MonoBehaviour
{
    /// <summary> Орудийная система. </summary>
    public WeaponModule WeaponSystem;
    /// <summary> Подсистема, влияющая на орудие. </summary>
    public SubSystem NecessarySystem;

    /// <summary> Является ли дугой. </summary>
    public bool Arc;
    /// <summary> Путь дуги. </summary>
    public Rail ArcRail;
    /// <summary> Карретка 1. </summary>
    public Mover L1;
    /// <summary> Карретка 2. </summary>
    public Mover L2;
    /// <summary> Свечение в основении луча. </summary>
    public GameObject BeamLight;

    /// <summary> Дуга заряжается. </summary>
    public bool ChargeringArc;

    /// <summary> Время перезарядки. </summary>
    public float ReloadTime;
    /// <summary> Текущее время перезарядки. </summary>
    [HideInInspector] public float curReloadTime;
    /// <summary> Время огня. </summary>
    public float FireTime;
    /// <summary> Текущее время огня. </summary>
    [HideInInspector] public float curFireTime;
    /// <summary> Урон. </summary>
    public float Damage;
    /// <summary> Текущий урон. </summary>
    [HideInInspector] public float curDamage;

    /// <summary> Блокировка орудий в градусной системе. </summary>
    public FireDegreesLockSystem DegreesLocking;

    /// <summary> Цель. </summary>
    public SelectableObject Target;

    /// <summary> Система запуска луча. </summary>
    private ArcReactor_Launcher _arl;

    /// <summary> Нахождение системы запуска луча. </summary>
    void Start()
    {
        if (_arl == null)
        {
            if (!Arc)
            {
                _arl = gameObject.GetComponent<ArcReactor_Launcher>();
            }
            else
            {
                _arl = gameObject.GetComponentInChildren<ArcReactor_Launcher>();
            }
        }
    }

    /// <summary> Активация орудия. </summary>
    public void Active(List<BeamWeapon> _otherWeapon)
    {
        if (NecessarySystem != null)
        {
            curDamage = Damage * NecessarySystem.efficiency;

            if (NecessarySystem.efficiency < 0.1f)
            {
                return;
            }
        }
        else
        {
            curDamage = Damage;
        }

        TargetSelecting();

        if (Target != null)
        {
            if (_otherWeapon.Count > 1)
            {
                _otherWeapon.Remove(this);
                foreach (BeamWeapon beam in _otherWeapon)
                {
                    if (beam.ChargeringArc)
                    {
                        DeactiveArcLights();
                        return;
                    }
                }
                Attacking();
            }
            else
            {
                Attacking();
            }
        }
        else
        {
            if (Arc)
            {
                DeactiveArcLights();
            }
        }


        if (curReloadTime > 0)
        {
            curReloadTime -= Time.deltaTime;
        }

        if (curFireTime > 0)
        {
            if (Target != null)
            {
                Vector3 LookVector = (Target.transform.position - this.transform.position);
                    
                Target._hs.ApplyDamage(curDamage, WeaponSystem.Aiming, LookVector);
                curFireTime -= Time.deltaTime;
                BeamLight.SetActive(true);
            }
            else
            {
                curFireTime = 0;
                BeamLight.SetActive(false);
            }
        }
        else
        {
            if (BeamLight.activeSelf)
            {
                BeamLight.SetActive(false);
                ChargeringArc = false;
            }
        }
    }

    /// <summary> Выключение корреток и их света. </summary>
    void DeactiveArcLights()
    {
        L1.gameObject.SetActive(false);
        L1.transform.position = ArcRail.nodes[0].position;
        L2.gameObject.SetActive(false);
        L1.transform.position = ArcRail.nodes.Last().position;

        BeamLight.SetActive(false);

        SetLightsOnPosition = false;
    }
    /// <summary> Выбор цели. </summary>
    void TargetSelecting()
    {
        if (WeaponSystem.MainTarget != null)
        {
            if (SeeTarget(WeaponSystem.MainTarget.transform))
            {
                Target = WeaponSystem.MainTarget;
                RotateBeamOnTarget(Target.transform);
            }
            else
            {
                if (WeaponSystem.Owner.Alerts == STMethods.Alerts.RedAlert)
                {
                    if (WeaponSystem.Targets.Count > 0 && Target == null)
                    {
                        foreach (SelectableObject targets in WeaponSystem.Targets)
                        {
                            if (SeeTarget(targets.transform))
                            {
                                Target = targets;
                                RotateBeamOnTarget(Target.transform);
                            }
                        }
                    }
                    else if(Target != null)
                    {
                        if (!SeeTarget(Target.transform))
                        {
                            Target = null;
                        }
                    }
                    else if(WeaponSystem.Targets.Count == 0)
                    {
                        Target = null;
                    }
                }
                else
                {
                    Target = null;
                }
            }
        }
        else
        {
            if (WeaponSystem.Owner.Alerts == STMethods.Alerts.RedAlert)
            {
                if (WeaponSystem.Targets.Count > 0 && Target == null)
                {
                    foreach (SelectableObject targets in WeaponSystem.Targets)
                    {
                        if (SeeTarget(targets.transform))
                        {
                            Target = targets;
                            RotateBeamOnTarget(Target.transform);
                        }
                    }
                }
                else if(Target != null)
                {
                    if (!SeeTarget(Target.transform))
                    {
                        Target = null;
                    }
                }
                else if(WeaponSystem.Targets.Count == 0)
                {
                    Target = null;
                }
            }
            else
            {
                Target = null;
            }
        }
    }
    /// <summary> Разворот орудия на цель. </summary>
    void RotateBeamOnTarget(Transform target)
    {
        if (target != null)
        {
            if (!Arc)
            {
                Vector3 LookVector = (target.transform.position - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(LookVector), 360);
            }
            else
            {
                Vector3 LookVector = (target.transform.position - STMethods.NearestTransform(ArcRail.nodes, target.transform).transform.position);
                _arl.transform.rotation = Quaternion.Slerp(STMethods.NearestTransform(ArcRail.nodes, target.transform).transform.rotation, Quaternion.LookRotation(LookVector), 360);
            }
        }
    }
    /// <summary> Ведение огня. </summary>
    void Attacking()
    {
        if (curReloadTime <= 0)
        {
            if (!Arc)
            {
                Fire();
                curReloadTime = ReloadTime;
            }
            else
            {
                ArcAttackSequence(STMethods.NearestTransformInt(ArcRail.nodes, Target.transform));
            }
        }
    }
    /// <summary> Создание луча и начало нанесения урона. </summary>
    void Fire()
    {
        if (!Arc)
        {
            curFireTime = FireTime;
            _arl.PhaserFire(Target.transform);
        }
    }
    /// <summary> Если цель видна. </summary>
    bool SeeTarget(Transform target)
    {        
        RotateBeamOnTarget(target);
        
        Vector3 _lv;
        if (!Arc)
        {
            _lv = gameObject.transform.localRotation.eulerAngles;
        }
        else
        {
           _lv = _arl.transform.localRotation.eulerAngles;
        }

        if (DegreesLocking.InvertX && !DegreesLocking.InvertY)
        {
            if ((_lv.x < DegreesLocking.MinX || _lv.x > DegreesLocking.MaxX) && _lv.y > DegreesLocking.MinY &&
                _lv.y < DegreesLocking.MaxY)
            {
                return true;
            }

            return false;
        }

        if (DegreesLocking.InvertY && !DegreesLocking.InvertX)
        {
            if (_lv.x > DegreesLocking.MinX && _lv.x < DegreesLocking.MaxX &&
                (_lv.y < DegreesLocking.MinY || _lv.y > DegreesLocking.MaxY))
            {
                return true;
            }

            return false;
        }

        if (DegreesLocking.InvertX && DegreesLocking.InvertY)
        {
            if ((_lv.x < DegreesLocking.MinX || _lv.x > DegreesLocking.MaxX) &&
                (_lv.y < DegreesLocking.MinY || _lv.y > DegreesLocking.MaxY))
            {
                return true;
            }

            return false;
        }

        if (!DegreesLocking.InvertX && !DegreesLocking.InvertY)
        {
            if (_lv.x > DegreesLocking.MinX && _lv.x < DegreesLocking.MaxX && _lv.y > DegreesLocking.MinY &&
                _lv.y < DegreesLocking.MaxY)
            {
                return true;
            }

            return false;
        }

        return false;
    }
    /// <summary> Производится ли установка корреток на позиции. </summary>
    private bool SetLightsOnPosition;
    /// <summary> Последовательность атаки дугового орудия. </summary>
    private void ArcAttackSequence(int node)
    {

        if (!SetLightsOnPosition)
        {
            ChargeringArc = true;
            
            LControllVoid(L2, node, true);
            LControllVoid(L1, node, false);


            L1.gameObject.SetActive(true);
            L2.gameObject.SetActive(true);

            SetLightsOnPosition = true;
        }

        if (L1.gameObject.activeSelf && L2.gameObject.activeSelf &&
            Vector3.Distance(L1.transform.position, L2.transform.position) < 0.1f)
        {
            _arl.gameObject.transform.position = L1.transform.position;
            _arl.PhaserFire(Target.transform);

            L1.gameObject.SetActive(false);
            L1.transform.position = ArcRail.nodes[0].position;
            L2.gameObject.SetActive(false);
            L1.transform.position = ArcRail.nodes.Last().position;

            SetLightsOnPosition = false;

            curFireTime = FireTime;
            curReloadTime = ReloadTime;
        }
    }
    /// <summary> Работа корретки. </summary>
    void LControllVoid(Mover light, int node, bool Revert)
    {
        float LSpeed = light.speed;

        light.isReversed = Revert;

        if (Revert)
        {
            if (node + 2 <= ArcRail.nodes.Length - 2)
            {
                light.currentSeg = node + 2;
                light.transition = 1;
                light.curSpeed = LSpeed;
            }
            else if (node + 1 <= ArcRail.nodes.Length - 1)
            {
                light.currentSeg = ArcRail.nodes.Length - 2;
                light.transition = 1;
                light.curSpeed = LSpeed / 2;
            }
            else if (node == ArcRail.nodes.Length - 1)
            {
                light.currentSeg = ArcRail.nodes.Length - 2;
                light.transition = 1;
                light.curSpeed = 0;
            }
        }
        else
        {
            if (node - 2 >= 0)
            {
                light.currentSeg = node - 2;
                light.transition = 0;
                light.curSpeed = LSpeed;
            }
            else if (node - 1 >= 0)
            {
                light.currentSeg = node - 1;
                light.transition = 0;
                light.curSpeed = LSpeed / 2;
            }
            else if (node == 0)
            {
                light.currentSeg = 0;
                light.transition = 0;
                light.curSpeed = LSpeed / 2;
            }
        }
    }
}