using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class ImpulsEngineSS : SubSystem
{
    public Vector3 StartStabVector = Vector3.zero;
    public float TimerToStabilisation = 3;
    public bool NeedStabilisation = false;
    public float StabilisationAmount = 0;

    private bool hitted;
    private float hittedTime = 1;
    // Start is called before the first frame update
    public override void isCreated()
    {
        Owner.effectManager.impulsEngine = this;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (hitted)
        {
            if (hittedTime > 0)
            {
                hittedTime -= Time.deltaTime;
            }
            else
            {
                hitted = false;
                hittedTime = 1;
            }
        }
        
        if ((Owner._hs != null && Owner._hs.MaxCrew > 0 && (int) Owner._hs.curCrew <= 0) || !Owner.effectManager.ImpulseEngineEffectActivity() || efficiency < 0.125f || hitted)
        {
            if (Owner.captain.Pilot != null)
            {
                Owner.rigitBody.isKinematic = false;
            }

            Owner.rigitBody.drag = 0;
            Owner.rigitBody.angularDrag = 0;
            
            StartStabVector = Vector3.zero;
            NeedStabilisation = false;
            StabilisationAmount = 0;
            TimerToStabilisation = 3;
            return;
        }

        if (Owner.captain.Pilot != null)
        {
            if (Owner.captain.Pilot.Status == EngineController.State.Stopped)
            {
                Owner.rigitBody.drag = Owner.rigitBody.velocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency;
                Owner.rigitBody.angularDrag = Owner.rigitBody.angularVelocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency;
                Stabilisation();
                return;
            }

            if (Owner.captain.Pilot.Status == EngineController.State.Stopping)
            {
                Owner.rigitBody.drag = Owner.rigitBody.velocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency / 2;
                Owner.rigitBody.angularDrag = Owner.rigitBody.angularVelocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency / 2;

                StartStabVector = Vector3.zero;
                NeedStabilisation = false;
                StabilisationAmount = 0;
                TimerToStabilisation = 3;

                return;
            }

            if (Vector3.Angle(Owner.rigitBody.velocity, Owner.transform.forward) < 15)
            {
                Owner.rigitBody.drag = 0;
                Owner.rigitBody.angularDrag = 0;
            }
            else
            {
                Owner.rigitBody.drag = Owner.rigitBody.velocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency / 2;
                Owner.rigitBody.angularDrag = Owner.rigitBody.angularVelocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency / 2;
            }
        }
        else
        {
            Owner.rigitBody.isKinematic = true;
            Owner.rigitBody.drag = Owner.rigitBody.velocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency;
            Owner.rigitBody.angularDrag =
                Owner.rigitBody.angularVelocity.magnitude * (Owner.rigitBody.mass / 2) * efficiency;
            Stabilisation();
        }
    }

    void Stabilisation()
    {
        if (Owner.captain.Pilot != null && Owner.captain.Pilot.engines.Moving) return;
        if (Owner.transform.rotation.eulerAngles.x > 1 || Owner.transform.rotation.eulerAngles.x < -1 || Owner.transform.rotation.eulerAngles.z > 1 || Owner.transform.rotation.eulerAngles.z < -1) NeedStabilisation = true;
        if (NeedStabilisation)
        {
            if (TimerToStabilisation > 0)
            {
                TimerToStabilisation -= Time.deltaTime;
            }
            else
            {
                float x, z;
                if (Owner.transform.rotation.eulerAngles.x < 180)
                {
                    x = 0;
                }
                else
                {
                    x = 360;
                }
                if (Owner.transform.rotation.eulerAngles.z < 180)
                {
                    z = 0;
                }
                else
                {
                    z = 360;
                }

                if (StartStabVector == Vector3.zero)
                {
                    StartStabVector = Owner.transform.rotation.eulerAngles;
                }
                
                Vector3 targetVec = new Vector3(x, Owner.transform.rotation.eulerAngles.y, z);

                float maxAngleInSecond = Vector3.Angle(StartStabVector, targetVec) / 5;
                
                Owner.rigitBody.angularVelocity = Vector3.Lerp(Owner.rigitBody.angularVelocity, Vector3.zero, StabilisationAmount);
                
                Quaternion newRot = Quaternion.Euler(Vector3.Lerp(StartStabVector, targetVec, StabilisationAmount));
                if (Quaternion.Angle(Owner.transform.rotation, newRot) < maxAngleInSecond)
                {
                    Owner.transform.rotation = newRot;
                }
                if (StabilisationAmount < 1)
                {
                    StabilisationAmount += Time.deltaTime / 5;
                }
                else
                {
                    StartStabVector = Vector3.zero;
                    NeedStabilisation = false;
                    StabilisationAmount = 0;
                    TimerToStabilisation = 3;
                }
            }
        }
    }

    public void Hit()
    {
        StartStabVector = Vector3.zero;
        NeedStabilisation = false;
        StabilisationAmount = 0;
        TimerToStabilisation = 3;
    }

    void OnCollisionStay()
    {
        hitted = true;
        hittedTime = 1;
    }

    private void OnCollisionExit()
    {
        hitted = false;
    }
}
