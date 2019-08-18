using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamWeapon : MonoBehaviour
{
    public WeaponModule WeaponSystem;
    public SubSystem NecessarySystem;

    public bool Arc;
    public Rail ArcRail;
    public Mover L1;
    public Mover L2;
    public GameObject BeamLight;
    private int AttackPoint;

    public bool ChargeringArc;

    public float ReloadTime;
    [HideInInspector] public float curReloadTime;

    public float FireTime;
    [HideInInspector] public float curFireTime;

    public float Damage;
    [HideInInspector] public float curDamage;

    public FireDegreesLockSystem DegreesLocking;

    public SelectableObject Target;

    private ArcReactor_Launcher _arl;

    // Start is called before the first frame update
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

    // Update is called once per frame
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

    void DeactiveArcLights()
    {
        L1.gameObject.SetActive(false);
        L1.transform.position = ArcRail.nodes[0].position;
        L2.gameObject.SetActive(false);
        L1.transform.position = ArcRail.nodes.Last().position;

        BeamLight.SetActive(false);

        SetLightsOnPosition = false;
    }
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
                if (WeaponSystem.Alerts == STMethods.Alerts.RedAlert)
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
            if (WeaponSystem.Alerts == STMethods.Alerts.RedAlert)
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

    void Fire()
    {
        if (!Arc)
        {
            curFireTime = FireTime;
            _arl.PhaserFire(Target.transform);
        }
    }

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

    private bool SetLightsOnPosition;

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