using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamWeapon : MonoBehaviour
{
    /// <summary> Подсистема, влияющая на орудие. </summary>
    public PrimaryWeaponSS NecessarySystem;

    /// <summary> Прицеливание. </summary>
    public STMethods.AttackType AimingTarget;

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

    /// <summary> Другие орудия. </summary>
    public List<BeamWeapon> otherWeapon;

    /// <summary> Попадание фазера. </summary>
    public RaycastHit phaserHit;

    private Transform BeamEndTransform;

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

    void Update()
    {
        if (Target != null)
        {
            if (otherWeapon.Count > 1)
            {
                otherWeapon.Remove(this);
                foreach (BeamWeapon beam in otherWeapon)
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

            if (!NecessarySystem.Gunner.TargetsUnderAttack.Any(x => x == Target))
            {
                NecessarySystem.Gunner.TargetsUnderAttack.Add(Target);
            }
        }
        else
        {
            if (Arc)
            {
                DeactiveArcLights();
            }

            if (NecessarySystem.Gunner.TargetsUnderAttack.Any(x => x == Target))
            {
                NecessarySystem.Gunner.TargetsUnderAttack.Remove(Target);
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
                int layerMask = 1 << 13 | 1 << 9;

                SelectableObject DamageTarget;

                if (phaserHit.point == Vector3.zero)
                {
                    if (Physics.Raycast(_arl.transform.position, _arl.transform.TransformDirection(Vector3.forward),
                        out phaserHit, Mathf.Infinity, layerMask))
                    {
                        if (phaserHit.transform.root.GetComponent<SelectableObject>())
                        {
                            DamageTarget = phaserHit.transform.root.GetComponent<SelectableObject>();
                        }
                        else
                        {
                            DamageTarget = Target;
                        }
                    }
                    else
                    {
                        DamageTarget = Target;
                    }
                }
                else
                {
                    if (phaserHit.transform.root.GetComponent<SelectableObject>())
                    {
                        DamageTarget = phaserHit.transform.root.GetComponent<SelectableObject>();
                    }
                    else
                    {
                        DamageTarget = Target;
                    }
                }

                if (DamageTarget._hs.Shilds != null && Target._hs.Shilds.Length > 0)
                {
                    if (DamageTarget._hs.Shilds[0].SubSystemCurHealth <= 0)
                    {
                        if(BeamEndTransform != null) DamageTarget._hs.ApplyDamage(curDamage / 2, AimingTarget, BeamEndTransform.position);
                    }
                    else
                    {
                        if(BeamEndTransform != null) DamageTarget._hs.ApplyDamage(curDamage, AimingTarget, BeamEndTransform.position);
                    }
                }
                else
                {
                    if(BeamEndTransform != null) DamageTarget._hs.ApplyDamage(curDamage / 2, AimingTarget, BeamEndTransform.position);
                }

                if (DamageTarget.rigitBody.drag == 0)
                {
                    if (DamageTarget is ResourceSource)
                    {
                        ResourceSource res = DamageTarget as ResourceSource;
                        if (res.spawner != null)
                        {
                            if(res.spawner.curAsteroids.Any(x => x == res)) res.spawner.curAsteroids.Remove(res);
                        }
                        DamageTarget.rigitBody.AddForceAtPosition((DamageTarget.transform.position - transform.position).normalized * curDamage * (DamageTarget.rigitBody.mass / 10), BeamEndTransform.position);
                    }
                    else
                    {
                        DamageTarget.rigitBody.AddForceAtPosition((DamageTarget.transform.position - transform.position).normalized * curDamage * (DamageTarget.rigitBody.mass / 1000), BeamEndTransform.position);
                    }
                }
                
                curFireTime -= Time.deltaTime;
                BeamLight.SetActive(true);
            }
            else
            {
                phaserHit.point = Vector3.zero;
                curFireTime = 0;

                if (BeamLight.activeSelf)
                {
                    BeamLight.SetActive(false);
                    ChargeringArc = false;
                    NecessarySystem.DeleteFromTargetList(Target);
                    Target = null;
                }
            }
        }
        else
        {
            phaserHit.point = Vector3.zero;
            if (BeamLight.activeSelf)
            {
                BeamLight.SetActive(false);
                ChargeringArc = false;
                NecessarySystem.DeleteFromTargetList(Target);
                Target = null;
            }
        }
    }

    /// <summary> Активация орудия. </summary>
    public void Active(SelectableObject target, STMethods.AttackType aiming)
    {

        if (target == null) return;
        AimingTarget = aiming;

        if (NecessarySystem != null)
        {
            curDamage = NecessarySystem.Owner.effectManager.PrimaryWeaponDamage(Damage);

            if (curDamage == 0)
            {
                return;
            }
        }
        else
        {
            curDamage = Damage;
        }

        FireCheck(target);
    }

    /// <summary> Выключение корреток и их света. </summary>
    void DeactiveArcLights()
    {
        L1.gameObject.SetActive(false);
        L1.transform.position = ArcRail.nodes[0].position;
        L2.gameObject.SetActive(false);
        L2.transform.position = ArcRail.nodes.Last().position;

        BeamLight.SetActive(false);

        SetLightsOnPosition = false;
    }

    /// <summary> Выбор цели. </summary>
    /// <summary> Может ли стрелять. </summary>
    void FireCheck(SelectableObject target)
    {
        if (SeeTarget(target.transform))
        {
            Target = target;
        }
        else
        {
            Target = null;
            ChargeringArc = false;
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
                this.transform.rotation =
                    Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(LookVector), 360);
            }
            else
            {
                Vector3 LookVector = (target.transform.position -
                                      STMethods.NearestTransform(ArcRail.nodes, target.transform).transform.position);
                _arl.transform.rotation =
                    Quaternion.Slerp(STMethods.NearestTransform(ArcRail.nodes, target.transform).transform.rotation,
                        Quaternion.LookRotation(LookVector), 360);
            }
        }
    }

    /// <summary> Ведение огня. </summary>
    void Attacking()
    {
        if (curReloadTime <= 0)
        {
            int layerMask = 1 << 9;

            SelectableObject DamageTarget = Target;

            RaycastHit seeHit;

            if (Physics.Raycast(_arl.transform.position, _arl.transform.TransformDirection(Vector3.forward),
                out seeHit, Mathf.Infinity, layerMask))
            {
                DamageTarget = seeHit.transform.root.GetComponent<SelectableObject>();
            }

            if (!Arc)
            {
                if (DamageTarget != Target)
                {
                    if (GameManager.instance.Players[NecessarySystem.Owner.PlayerNum].TeamNum !=
                        GameManager.instance.Players[DamageTarget.PlayerNum].TeamNum)
                    {
                        Fire(DamageTarget);
                    }
                }
                else
                {
                    Fire(Target);
                }

                curReloadTime = NecessarySystem.Owner.effectManager.PrimaryWeaponFireReload(ReloadTime);
            }
            else
            {
                if (DamageTarget != Target)
                {
                    if (GameManager.instance.Players[NecessarySystem.Owner.PlayerNum].TeamNum !=
                        GameManager.instance.Players[DamageTarget.PlayerNum].TeamNum)
                    {
                        ArcAttackSequence(STMethods.NearestTransformInt(ArcRail.nodes, Target.transform), DamageTarget);
                    }
                }
                else
                {
                    ArcAttackSequence(STMethods.NearestTransformInt(ArcRail.nodes, Target.transform), Target);
                }
            }
        }
    }

    /// <summary> Создание луча и начало нанесения урона. </summary>
    void Fire(SelectableObject target)
    {
        if (!Arc)
        {
            curFireTime = FireTime;
            if (NecessarySystem.Owner.isVisible == STMethods.Visibility.Visible)
            {
                if (target._hs.ShieldsEnable())
                {
                    BeamEndTransform = _arl.LaunchRayAndGetTransform();
                }
                else
                {
                    BeamEndTransform = target._hs.getRandomFirePoint(AimingTarget, transform);
                    _arl.PhaserFire(BeamEndTransform);
                }
            }
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
    private void ArcAttackSequence(int node, SelectableObject target)
    {
        if (SeeTarget(target.transform))
        {
            if (!SetLightsOnPosition)
            {
                ChargeringArc = true;

                LControllVoid(L2, node, true);
                LControllVoid(L1, node, false);

                if (NecessarySystem.Owner.isVisible == STMethods.Visibility.Visible)
                {
                    L1.gameObject.SetActive(true);
                    L2.gameObject.SetActive(true);
                }

                SetLightsOnPosition = true;
            }

            if (L1.gameObject.activeSelf && L2.gameObject.activeSelf &&
                Vector3.Distance(L1.transform.position, L2.transform.position) < 0.2f)
            {
                if (NecessarySystem.Owner.isVisible == STMethods.Visibility.Visible)
                {
                    _arl.gameObject.transform.position = L1.transform.position;
                    if (target._hs.ShieldsEnable())
                    {
                        BeamEndTransform = _arl.LaunchRayAndGetTransform();
                    }
                    else
                    {
                        BeamEndTransform = target._hs.getRandomFirePoint(AimingTarget, transform);
                        _arl.PhaserFire(BeamEndTransform);
                    }
                }

                L1.gameObject.SetActive(false);
                L1.transform.position = ArcRail.nodes[0].position;
                L2.gameObject.SetActive(false);
                L2.transform.position = ArcRail.nodes.Last().position;

                SetLightsOnPosition = false;

                curFireTime = FireTime;
                curReloadTime = NecessarySystem.Owner.effectManager.PrimaryWeaponFireReload(ReloadTime);
            }
        }
        else
        {
            ChargeringArc = false;
            BeamLight.SetActive(false);

            L1.gameObject.SetActive(false);
            L1.transform.position = ArcRail.nodes[0].position;
            L2.gameObject.SetActive(false);
            L2.transform.position = ArcRail.nodes.Last().position;

            SetLightsOnPosition = false;

            curReloadTime = NecessarySystem.Owner.effectManager.PrimaryWeaponFireReload(ReloadTime);
            DeactiveArcLights();
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