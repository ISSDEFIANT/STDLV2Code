using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TorpedoLauncher : MonoBehaviour
{
    public WeaponModule WeaponSystem;
    public SubSystem NecessarySystem;

    public GameObject shell;

    public float collisionDelay;

    public int maxTorpidos;
    [HideInInspector] public int curTorpidos;

    public float TorpedoRange = 1;
    [HideInInspector] public float curTorpedoRange = 0;

    public float ReloadTime;
    [HideInInspector] public float curReloadTime;

    public FireDegreesLockSystem DegreesLocking;

    public SelectableObject Target;

    public GameObject[] AllTorpedose;
    private int poolSize;

    private bool Reloading;

    // Start is called before the first frame update

    public void Active()
    {
        if (NecessarySystem != null)
        {
            if (NecessarySystem.efficiency < 0.1f)
            {
                return;
            }
        }

        TargetSelecting();

        if (Target != null && !Reloading)
        {
            Attacking();
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

    void TargetSelecting()
    {
        if (WeaponSystem.MainTarget != null)
        {
            if (SeeTarget(WeaponSystem.MainTarget.transform))
            {
                Target = WeaponSystem.MainTarget;
                RotateOnTarget(Target.transform);
            }
            else
            {
                if (WeaponSystem.Alerts == STMethods.Alerts.RedAlert)
                {
                    if (WeaponSystem.Targets.Count > 0 && Target == null)
                    {
                        foreach (SelectableObject targets in WeaponSystem.Targets)
                        {
                            if (SeeTarget(targets.transform))
                            {
                                Target = targets;
                                RotateOnTarget(Target.transform);
                            }
                        }
                    }
                    else if (Target != null)
                    {
                        if (!SeeTarget(Target.transform))
                        {
                            Target = null;
                        }
                    }
                    else if (WeaponSystem.Targets.Count == 0)
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
            if (WeaponSystem.Alerts == STMethods.Alerts.RedAlert)
            {
                if (WeaponSystem.Targets.Count > 0 && Target == null)
                {
                    foreach (SelectableObject targets in WeaponSystem.Targets)
                    {
                        if (SeeTarget(targets.transform))
                        {
                            Target = targets;
                            RotateOnTarget(Target.transform);
                        }
                    }
                }
                else if (Target != null)
                {
                    if (!SeeTarget(Target.transform))
                    {
                        Target = null;
                    }
                }
                else if (WeaponSystem.Targets.Count == 0)
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

    void RotateOnTarget(Transform target)
    {
        if (target != null)
        {
            Vector3 LookVector = (target.transform.position - this.transform.position);
            this.transform.rotation =
                Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(LookVector), 360);

        }
    }

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

    void Attacking()
    {
        if (curTorpidos > 0)
        {
            if (curTorpedoRange <= 0)
            {
                InstantiateAlternative();

                curTorpidos -= 1;

                curTorpedoRange = TorpedoRange;
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

    // Update is called once per frame
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

                _s.attackType = WeaponSystem.Aiming;
                _s.collisionDelay = collisionDelay;
                _s.target = Target.transform;
                break;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject _t in AllTorpedose)
        {
            Destroy(_t);
        }
    }
}