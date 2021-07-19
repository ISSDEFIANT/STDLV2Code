using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class TorpedoLauncher : MonoBehaviour
{
    public bool ImpulsPhaser;
    /// <summary> Подсистема, влияющая на орудие (первичное). </summary>
    public PrimaryWeaponSS PriNecessarySystem;
    /// <summary> Подсистема, влияющая на орудие (вторичное). </summary>
    public SecondaryWeaponSS SecNecessarySystem;

    /// <summary> Прицеливание. </summary>
    public STMethods.AttackType AimingTarget;

    /// <summary> Торпеда (снаряд). </summary>
    public GameObject shell;

    /// <summary> Максимальное количество торпед (снарядов). </summary>
    public int maxTorpidos;

    /// <summary> Текущее количество торпед (снарядов). </summary>
    public int curTorpidos;

    /// <summary> Задержка между выстрелами. </summary>
    public float TorpedoRange = 1;

    /// <summary> Текущая задержка. </summary>
    [HideInInspector] public float curTorpedoRange = 0;

    /// <summary> Время перезарядки залпа. </summary>
    public float ReloadTime;

    /// <summary> Текущее время перезарядки залпа. </summary>
    public float curReloadTime;

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

    /// <summary> Звук для импульсных фазеров. </summary>
    private AudioSource impulseWeaponSound;

    public bool useParentForwardVector;
    public bool useParentBackVector;

    /// <summary> Орудие активно. </summary>
    public void Active(SelectableObject target, STMethods.AttackType aiming)
    {
        AimingTarget = aiming;

        if (ImpulsPhaser)
        {
            if (PriNecessarySystem != null)
            {
                if (PriNecessarySystem.Owner.effectManager.PrimaryWeaponFireReload(ReloadTime) == 0)
                {
                    return;
                }
            }
        }
        else
        {
            if (SecNecessarySystem != null)
            {
                if (SecNecessarySystem.Owner.effectManager.SecondaryWeaponFireReload(ReloadTime) == 0)
                {
                    return;
                }
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
            RotateOnTarget(Target.transform, Target.rigitBody);
        }
        else
        {
            Target = null;
        }
    }

    /// <summary> Разворот на цель. </summary>
    void RotateOnTarget(Transform target, Rigidbody rigitbody = null)
    {
        if (target != null)
        {
            Vector3 LookVector;
            if (rigitbody == null)
            {
                LookVector = (target.transform.position - gameObject.transform.position);
            }
            else
            {
                LookVector = (target.transform.position+rigitbody.velocity - gameObject.transform.position);   
            }
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(LookVector), 360);

        }
    }

    /// <summary> Видит ли цель. </summary>
    bool SeeTarget(Transform target)
    {
        RotateOnTarget(target);

        Vector3 _lv = gameObject.transform.localRotation.eulerAngles;

        if (DegreesLocking.InvertX && !DegreesLocking.InvertY)
        {
            if ((_lv.x <= DegreesLocking.MinX || _lv.x >= DegreesLocking.MaxX) && _lv.y >= DegreesLocking.MinY &&
                _lv.y <= DegreesLocking.MaxY)
            {
                return true;
            }

            return false;
        }

        if (DegreesLocking.InvertY && !DegreesLocking.InvertX)
        {
            if (_lv.x >= DegreesLocking.MinX && _lv.x <= DegreesLocking.MaxX &&
                (_lv.y <= DegreesLocking.MinY || _lv.y >= DegreesLocking.MaxY))
            {
                return true;
            }

            return false;
        }

        if (DegreesLocking.InvertX && DegreesLocking.InvertY)
        {
            if ((_lv.x <= DegreesLocking.MinX || _lv.x >= DegreesLocking.MaxX) &&
                (_lv.y <= DegreesLocking.MinY || _lv.y >= DegreesLocking.MaxY))
            {
                return true;
            }

            return false;
        }

        if (!DegreesLocking.InvertX && !DegreesLocking.InvertY)
        {
            if (_lv.x >= DegreesLocking.MinX && _lv.x <= DegreesLocking.MaxX && _lv.y >= DegreesLocking.MinY &&
                _lv.y <= DegreesLocking.MaxY)
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
        if (ImpulsPhaser)
        {
            if(PriNecessarySystem.Owner.effectManager.PrimaryWeaponFireReload(ReloadTime) == 0) return;
        }
        else
        {
            if(SecNecessarySystem.Owner.effectManager.SecondaryWeaponFireReload(ReloadTime) == 0) return;
        }
        if (curTorpidos > 0)
        {
            if (curTorpedoRange <= 0)
            {
                InstantiateAlternative();

                curTorpidos -= 1;

                curTorpedoRange = TorpedoRange;

                Target = null;
                if (ImpulsPhaser)
                {
                    PriNecessarySystem.DeleteFromTargetList(Target);
                }
                else
                {
                    SecNecessarySystem.DeleteFromTargetList(Target);
                }
            }
            else
            {
                if (ImpulsPhaser)
                {
                    if (PriNecessarySystem != null)
                    {
                        curTorpedoRange -= Time.deltaTime * PriNecessarySystem.efficiency;
                    }
                    else
                    {
                        curTorpedoRange -= Time.deltaTime;
                    }
                }
                else
                {
                    if (SecNecessarySystem != null)
                    {
                        curTorpedoRange -= Time.deltaTime * SecNecessarySystem.efficiency;
                    }
                    else
                    {
                        curTorpedoRange -= Time.deltaTime;
                    }
                }
            }
            if(impulseWeaponSound != null && !impulseWeaponSound.isPlaying)impulseWeaponSound.Play();
        }
        else
        {
            if (curReloadTime <= 0)
            {
                if (ImpulsPhaser)
                {
                    curReloadTime = PriNecessarySystem.Owner.effectManager.PrimaryWeaponFireReload(ReloadTime);
                }
                else
                {
                    curReloadTime = SecNecessarySystem.Owner.effectManager.SecondaryWeaponFireReload(ReloadTime);   
                }
                Reloading = true;
            }
            if(impulseWeaponSound != null && impulseWeaponSound.isPlaying)impulseWeaponSound.Stop();
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

        if (gameObject.GetComponent<AudioSource>()) impulseWeaponSound = gameObject.GetComponent<AudioSource>();
    }

    /// <summary> Активация торпед (снарядов). </summary>
    public void InstantiateAlternative()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (AllTorpedose[i].activeInHierarchy == false)
            {
                Shell _s = AllTorpedose[i].GetComponent<Shell>();
                _s.attackType = AimingTarget;

                _s.target = Target;
                if (ImpulsPhaser)
                {
                    _s.owner = PriNecessarySystem.Owner.gameObject;
                }
                else
                {
                    _s.owner = SecNecessarySystem.Owner.gameObject;
                }
                AllTorpedose[i].SetActive(true);
                AllTorpedose[i].transform.position = gameObject.transform.position;
                if (!useParentForwardVector && !useParentBackVector)
                {
                    AllTorpedose[i].transform.rotation = gameObject.transform.rotation;
                }
                else if(useParentForwardVector)
                {
                    AllTorpedose[i].transform.rotation = gameObject.transform.root.rotation;
                }
                else if (useParentBackVector)
                {
                    AllTorpedose[i].transform.rotation = Quaternion.Inverse(gameObject.transform.root.rotation);
                }

                if(impulseWeaponSound == null) _s.PlayFireSound();
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
            else
            {
                if(impulseWeaponSound != null && impulseWeaponSound.isPlaying)impulseWeaponSound.Stop();
            }

            if (ImpulsPhaser)
            {
                if (!PriNecessarySystem.Gunner.TargetsUnderAttack.Any(x => x == Target))
                {
                    PriNecessarySystem.Gunner.TargetsUnderAttack.Add(Target);
                }
            }
            else
            {
                if (!SecNecessarySystem.Gunner.TargetsUnderAttack.Any(x => x == Target))
                {
                    SecNecessarySystem.Gunner.TargetsUnderAttack.Add(Target);
                }   
            }
        }
        else
        {
            if (ImpulsPhaser)
            {
                if (PriNecessarySystem.Gunner.TargetsUnderAttack.Any(x => x == Target))
                {
                    PriNecessarySystem.Gunner.TargetsUnderAttack.Remove(Target);
                }  
            }
            else
            {
                if (SecNecessarySystem.Gunner.TargetsUnderAttack.Any(x => x == Target))
                {
                    SecNecessarySystem.Gunner.TargetsUnderAttack.Remove(Target);
                }   
            }
            if(impulseWeaponSound != null && impulseWeaponSound.isPlaying)impulseWeaponSound.Stop();
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
            if(impulseWeaponSound != null && impulseWeaponSound.isPlaying)impulseWeaponSound.Stop();
        }
    }
}