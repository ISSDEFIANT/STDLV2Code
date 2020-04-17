using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class TorpedoLauncher : MonoBehaviour
{
    /// <summary> Подсистема, влияющая на орудие. </summary>
    public SecondaryWeaponSS NecessarySystem;

    /// <summary> Прицеливание. </summary>
    public STMethods.AttackType AimingTarget;

    /// <summary> Торпеда (снаряд). </summary>
    public GameObject shell;

    /// <summary> Время до активации коллизии. </summary>
    public float collisionDelay;

    /// <summary> Максимальное количество торпед (снарядов). </summary>
    public int maxTorpidos;

    /// <summary> Текущее количество торпед (снарядов). </summary>
    [HideInInspector] public int curTorpidos;

    /// <summary> Задержка между выстрелами. </summary>
    public float TorpedoRange = 1;

    /// <summary> Текущая задержка. </summary>
    [HideInInspector] public float curTorpedoRange = 0;

    /// <summary> Время перезарядки залпа. </summary>
    public float ReloadTime;

    /// <summary> Текущее время перезарядки залпа. </summary>
    [HideInInspector] public float curReloadTime;

    /// <summary> Блокировка орудий в градусной системе. </summary>
    public FireDegreesLockSystem DegreesLocking;

    /// <summary> Цель. </summary>
    public SelectableObject Target;

    /// <summary> Торпеды (снаряды). </summary>
    public GameObject[] AllTorpedose;

    /// <summary> Количество торпед (снарядов). </summary>
    private int poolSize;

    /// <summary> Производится ли перезарядка. </summary>
    private bool Reloading;

    /// <summary> Орудие активно. </summary>
    public void Active(SelectableObject target, STMethods.AttackType aiming)
    {
        AimingTarget = aiming;

        if (NecessarySystem != null)
        {
            if (NecessarySystem.efficiency < 0.1f)
            {
                return;
            }
        }

        FireCheck(target);
    }

    /// <summary> Может ли стрелять. </summary>
    void FireCheck(SelectableObject target)
    {
        if (SeeTarget(target.transform))
        {
            Target = target;
            RotateOnTarget(Target.transform);
        }
        else
        {
            Target = null;
        }
    }

    /// <summary> Разворот на цель. </summary>
    void RotateOnTarget(Transform target)
    {
        if (target != null)
        {
            Vector3 LookVector = (target.transform.position - this.transform.position);
            this.transform.rotation =
                Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(LookVector), 360);

        }
    }

    /// <summary> Видит ли цель. </summary>
    bool SeeTarget(Transform target)
    {
        RotateOnTarget(target);

        Vector3 _lv = gameObject.transform.localRotation.eulerAngles;

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

    /// <summary> Процесс атаки. </summary>
    void Attacking()
    {
        if (curTorpidos > 0)
        {
            if (curTorpedoRange <= 0)
            {
                InstantiateAlternative();

                curTorpidos -= 1;

                curTorpedoRange = TorpedoRange;

                Target = null;
                NecessarySystem.DeleteFromTargetList(Target);
            }
            else
            {
                if (NecessarySystem != null)
                {
                    curTorpedoRange -= Time.deltaTime * NecessarySystem.efficiency;
                }
                else
                {
                    curTorpedoRange -= Time.deltaTime;
                }
            }
        }
        else
        {
            if (curReloadTime <= 0)
            {
                curReloadTime = ReloadTime;
                Reloading = true;
            }
        }
    }

    /// <summary> Создание торпед (снарядов). </summary>
    void Awake()
    {
        curTorpidos = maxTorpidos;

        poolSize = maxTorpidos;

        AllTorpedose = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            AllTorpedose[i] = Instantiate(shell) as GameObject;
            AllTorpedose[i].SetActive(false);
        }
    }

    /// <summary> Активация торпед (снарядов). </summary>
    public void InstantiateAlternative()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (AllTorpedose[i].activeInHierarchy == false)
            {
                AllTorpedose[i].SetActive(true);
                AllTorpedose[i].transform.position = gameObject.transform.position;
                AllTorpedose[i].transform.rotation = gameObject.transform.rotation;
                Shell _s = AllTorpedose[i].GetComponent<Shell>();

                _s.attackType = AimingTarget;
                _s.collisionDelay = collisionDelay;
                _s.target = Target.transform;
                _s.PlayFireSound();
                break;
            }
        }
    }

    /// <summary> Уничтожение торпед (снарядов) вместе с орудийной системой. </summary>
    private void OnDestroy()
    {
        foreach (GameObject _t in AllTorpedose)
        {
            Destroy(_t);
        }
    }

    private void Update()
    {
        if (Target != null)
        {
            if (!Reloading)
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
            if (NecessarySystem.Gunner.TargetsUnderAttack.Any(x => x == Target))
            {
                NecessarySystem.Gunner.TargetsUnderAttack.Remove(Target);
            }
        }

        if (Reloading)
        {
            if (curTorpidos <= 0)
            {
                if (curReloadTime > 0)
                {
                    curReloadTime -= Time.deltaTime;
                }
                else
                {
                    curTorpidos = maxTorpidos;
                    Reloading = false;
                }
            }
        }
    }
}