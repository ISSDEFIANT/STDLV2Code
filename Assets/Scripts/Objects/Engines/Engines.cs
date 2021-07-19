using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Controllers;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

/// <summary> Компонент, отвечающий за перемещение корабля в 3D-пространстве. </summary>
[RequireComponent(typeof(Rigidbody))]
public class Engines : MonoBehaviour
{
    public EngineController Pilot;
    
    /// <summary> Двигается ли корабль. </summary>
    public bool Moving;
    
    /// <summary> Двигается ли корабль на варпе. </summary>
    public bool Warping;

    /// <summary> Допустимая ошибка. </summary>
    public float Threshold = 1f;
    
    /// <summary> Коэффициент крена </summary>
    protected const float RollSpeed = 0.05f;
    
    [Tooltip("Объект к которому надо лететь")]
    public Vector3 Target;
    
    [Tooltip("Ускорение корабля")]
    public float Acceleration = 20f;
    
    [Tooltip("Максимальная возможная скорость корабля")]
    public float MaxSpeed = 30f;
    
    [Tooltip("Максимальная скорость корабля")]
    public float CurMaxSpeed = 30f;
    
    public float maxAvailableSpeed()
    {
        return Owner.effectManager.ImpulseEngineSpeed();
    }
    
    public float maxAvailableWarpSpeed()
    {
        return Owner.effectManager.WarpEngineSpeed();
    }

    public bool WarpIn;
    public bool WarpCourceCorrection;

    public float CurForwardSpeed;

    public SubSystem impulseControlingSubSystem;
    public SubSystem warpControlingSubSystem;
    public SubSystem warpcoreControllingSystem;

    public GameObject WarpBlink;
    public GameObject BorgWarpBlink;

    public float fleetWarpIn = 2;

    public bool CanMove()
    {
        if (Owner.effectManager.ImpulseEngineEffectActivity() && Owner.effectManager.ImpulseEngineSpeed() > 0)
        {
            return true;
        }
        return false;
    }

    public bool CanWarp;

    [Tooltip("Дочерний объект с мешем корабля")]
    public Transform Model;

    [Tooltip("Максимальный угол крена")]
    public float MaxRollAngle = 30f;
    
    /// <summary> Нормированный вектор направления на цель </summary>
    public Vector3 DirectionToTarget { get; protected set; }
    
    /// <summary> Расстояние до цели. </summary>
    public float DistanceToTarget { get; protected set; }

    /// <summary> Расстояние которое пройдет корабль до остановки. </summary>
    /*public float DistanceToFullStop (float accele)
    {
        float velocity = _rigidbody.velocity.magnitude;
        return velocity * velocity / 2 / accele;
    }*/
    
    public float DistanceToFullStop (float accele)
    {
        float velocity = CurForwardSpeed;
        return velocity * velocity / 2 / accele;
    }

    /// <summary> Допустимая ошибка по углу. </summary>
    /// <remarks> Зависит от расстояния до цели. </remarks>
    public float AngularThreshold => 2 * Mathf.Acos(Threshold / DistanceToTarget);

    public Mobile Owner;
    
    private HealthSystem _hs = null;
    
    #region Unity event functions

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (gameObject.GetComponent<HealthSystem>()) _hs = gameObject.GetComponent<HealthSystem>();
        
        Threshold = Owner.Threshold;
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.angularDrag = 10f;
        
        if (!gameObject.GetComponent<EngineController>())
        {
            Pilot = gameObject.AddComponent<EngineController>();
            Pilot.engines = this;
            Pilot.Owner = Owner;
        }
        else
        {
            Pilot = gameObject.GetComponent<EngineController>();
            Pilot.engines = this;
            Pilot.Owner = Owner;
        }
        
        if (Owner._hs.SubSystems[0].gameObject.GetComponent<ImpulsEngineSS>()) impulseControlingSubSystem = Owner._hs.SubSystems[0].gameObject.GetComponent<ImpulsEngineSS>();
        if (Owner._hs.SubSystems[0].gameObject.GetComponent<WarpEngineSS>()) warpControlingSubSystem = Owner._hs.SubSystems[0].gameObject.GetComponent<WarpEngineSS>();
        if (Owner._hs.SubSystems[0].gameObject.GetComponent<WarpCoreSS>()) warpcoreControllingSystem = Owner._hs.SubSystems[0].gameObject.GetComponent<WarpCoreSS>();
        
        fleetWarpIn = Random.Range(1.5f, 2f);
    }

    public void LateUpdate()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        if ((_hs != null && _hs.MaxCrew > 0 && (int)_hs.curCrew <= 0) || !Owner.effectManager.ImpulseEngineEffectActivity() || impulseControlingSubSystem.SubSystemCurHealth < impulseControlingSubSystem.SubSystemMaxHealth * 0.125f)
        {
            return;
        }
        
        var localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
        CurForwardSpeed = Mathf.Max(0, localVelocity.z);
        
        if (CanMove ())
        {
            if (Moving)
            {
                CalculateTargetParameters();
                if (!Warping)
                {
                    MoveToTarget(Target);

                    if (DistanceToTarget < Threshold && _rigidbody.velocity.magnitude < Threshold)
                    {
                        Moving = false;
                        Pilot.Status = EngineController.State.Stopped;
                    }
                }
                else
                {
                    Vector3 targetDir = Target - transform.position;
                    float angle = Vector3.Angle(targetDir, transform.forward);
                    
                    if (angle < 5)
                    {
                        WarpCourceCorrection = false;
                    }

                    if (!WarpCourceCorrection)
                    {
                        if (Vector3.Distance(transform.position,Target) > Owner.ObjectRadius * 10)
                        {
                            if (CanWarp)
                            {
                                WarpToTarget(Target);
                            }
                            else
                            {
                                WarpOut();
                            }

                            if (angle > 15f)
                            {
                                WarpCourceCorrection = true;
                            }
                        }
                        else
                        {
                            WarpOut();
                        }
                    }
                    else
                    {
                        WarpOut();
                    }
                }
            }
            else
            {
                if (_rigidbody.velocity != new Vector3(0, 0, 0))
                {
                    StartCoroutine(stopAllMovment());
                }
            }
        }
        else
        {
            if (Warping) WarpOut();
            Moving = false;
        }
    }

    private void WarpOut()
    {
        if (WarpIn)
        {
            GameObject wBlink; 
            if (Owner.Assimilated)
            {
                wBlink = Instantiate(BorgWarpBlink, transform.position, transform.rotation);
            }
            else
            {
                wBlink = Instantiate(WarpBlink, transform.position, transform.rotation);
            }

            wBlink.transform.localScale =
                new Vector3(Owner.ObjectRadius / 2, Owner.ObjectRadius / 2, Owner.ObjectRadius / 2);
            WarpIn = false;
        }

        fleetWarpIn = Random.Range(1.5f, 2f);
        Warping = false;
    }

    #endregion

    #region Private definitions

    private Rigidbody _rigidbody;

    /// <summary> Функция движения к цели </summary>
    /// <remarks> Должна вызываться в FixedUpdate </remarks>
    /// <param name="targetPosition"> Координаты цели. </param>
    private void MoveToTarget(Vector3 targetPosition)
    {
        ClipVelocity(CurMaxSpeed);

        if (_rigidbody.velocity.magnitude > Threshold)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_rigidbody.velocity),CurForwardSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 relativePos = targetPosition - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos),Acceleration/10 * Time.deltaTime);
        }

        if (DistanceToTarget < Threshold
            && _rigidbody.velocity.magnitude < Threshold * Acceleration)
        {
            if (targetPosition.FloatEquals(transform.position)) return;
            
            _rigidbody.velocity = Vector3.zero;

            RollShip(true);
            return;
        }

        RollShip();
        
        Vector3 force = CalculateForce(targetPosition);
        if (force.magnitude > CurMaxSpeed) return;
        _rigidbody.AddRelativeForce(force, ForceMode.Acceleration);
    }

    public IEnumerator StabPosition(Vector3 targetPosition)
    {
        for (float ft = 1f; ft >= 0; ft -= Time.deltaTime) 
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, ft);
            yield return null;
        }
    }
    
    private void WarpToTarget(Vector3 targetPosition)
    {
        ClipVelocity(250);

        if (_rigidbody.velocity.magnitude > Threshold)
        {
            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
            if (!WarpIn)
            {
                GameObject wBlink;
                if (Owner.Assimilated)
                {
                    wBlink = Instantiate(BorgWarpBlink, transform.position, transform.rotation);
                }
                else
                {
                    wBlink = Instantiate(WarpBlink, transform.position, transform.rotation);
                }
                wBlink.transform.localScale = new Vector3(Owner.ObjectRadius / 2,Owner.ObjectRadius / 2, Owner.ObjectRadius / 2);
                WarpIn = true;
            }
        }
        else
        {
            Vector3 relativePos = targetPosition - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos),Acceleration/10 * Time.deltaTime);
        }

        if (DistanceToTarget < Threshold
            && _rigidbody.velocity.magnitude < Threshold * Acceleration)
        {
            if (targetPosition.FloatEquals(transform.position)) return;
            
            _rigidbody.velocity = Vector3.zero;

            RollShip(true);
            return;
        }

        RollShip();
        
        Vector3 force = CalculateForce(targetPosition, 250);
        _rigidbody.AddRelativeForce(force, ForceMode.Acceleration);
    }

    /// <summary> Каждый кадр рассчитывает параметры цели, такие как направление на цель и т.п. </summary>
    private void CalculateTargetParameters()
    {
        Vector3 toTarget = Target - transform.position;    
        DirectionToTarget = toTarget.normalized;
        DistanceToTarget = toTarget.magnitude;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(0, 255, 0, 100);
        Gizmos.DrawSphere(Target, 5);
    }
    
    /// <summary>
    /// Рассчитывает относительную силу, которую надо приложить, чтобы корабль двигался в нужном направлении.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private Vector3 CalculateForce(Vector3 targetPosition, float Multiplier = 1f)
    {
        Vector3 force = Vector3.zero;
        Vector3 velocity = _rigidbody.velocity;
        float speed = CurForwardSpeed;
        bool accelerationNeeded = DistanceToTarget > DistanceToFullStop (Acceleration*Multiplier);
        
        // Если корабль направлен не на цель -- поворачиваем
        float errorAngle = Vector3.Angle(DirectionToTarget, velocity);
        float angleThreshold = AngularThreshold;

        float angleToTarget = Vector3.Angle(DirectionToTarget, transform.forward);
        
        // Если корабль стоит на месте, то сначала немного разгоняемся, чтобы поворот не выглядел резко
        if (accelerationNeeded && speed < (CurMaxSpeed*Multiplier) / 4)
        {
            if (angleToTarget < 10 || speed > Threshold)
            {
                force += Acceleration*Multiplier * Vector3.forward;
                return force;
            }
            return force;
        }
        
        
        if (angleToTarget > angleThreshold)
        {
            Vector3 forceDir = new Vector3(DirectionToTarget.ProjectOn(transform.right), 
                                              DirectionToTarget.ProjectOn(transform.up), 
                                           0);
            forceDir.Normalize();

            // Если нормальное ускорение слишком большое и корабль отклоняется с перелётом,
            // то уменьшаем его до необходимого.
            Vector3 newVel = velocity + Acceleration * Time.fixedDeltaTime * forceDir;
            float newErrorAngle = Vector3.Angle(DirectionToTarget, newVel);
            if (newErrorAngle > errorAngle && newErrorAngle < 2 * angleThreshold)
            {
                force += Mathf.Abs(Mathf.Tan(errorAngle)) * speed * forceDir;
            }
            else
            {
                force += Acceleration * forceDir;
            }
            
            // Если возможно, закладываем вираж не снижая скорости.
            // Вираж возможен, если в результате него, корабль не пролетит мимо цели.
            // Корабль пролетит мимо цели если цель находится внутри окружности, по которой будет лететь корабль.
            float rotationRadius = speed * speed / (Acceleration*Multiplier);
            Vector3 circleCenter = transform.position + forceDir * rotationRadius;
            if ((targetPosition - circleCenter).magnitude < rotationRadius
                || !accelerationNeeded)
            {
                force -= Acceleration*Multiplier * Vector3.forward;
            }
            return force;
        }
        
        if (accelerationNeeded)
        {
            if (Math.Abs(speed - CurMaxSpeed) > float.Epsilon)
            {
                force += Acceleration*Multiplier * Vector3.forward;
            }
        }
        else
        {
            force -= Acceleration*Multiplier * Vector3.forward;
        }
        
        return force;
    }

    /// <summary> Вычисляет и применяет крен корабля во время поворота. </summary>
    /// <param name="toZero"> true -- если надо убрать крен. </param>
    private void RollShip(bool toZero = false, bool revert = false)
    {
        Vector3 directionToTarget = DirectionToTarget;
        Vector3 shipDirection = transform.forward;
        directionToTarget.y = 0;
        shipDirection.y = 0;
        
        float rollAngle = Vector3.Angle(shipDirection, directionToTarget);
        
        if (rollAngle < AngularThreshold || toZero)
        {
            rollAngle = 0;
            
        }
        else if (rollAngle > MaxRollAngle)
        {
            rollAngle = MaxRollAngle;
        }

        if (!revert)
        {
            rollAngle *= -Mathf.Sign(directionToTarget.ProjectOn(transform.right));
        }
        else
        {
            rollAngle *= -Mathf.Sign(directionToTarget.ProjectOn(transform.right))*-1;   
        }

        Model.transform.localRotation = Quaternion.Lerp(Model.transform.localRotation,
                                                        Quaternion.Euler(0, 0, rollAngle),
                                                        RollSpeed);
    }

    private void ShiftShip(Vector3 target)
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
    }

    private IEnumerator stopAllMovment()
    {
        for (float ft = 2f; ft >= 0; ft -= Time.deltaTime) 
        {
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, ft);
            _rigidbody.angularVelocity = Vector3.Lerp(_rigidbody.angularVelocity, Vector3.zero, ft);
            yield return null;
        }
    }

    private void Awake()
    {
        Target = transform.position;
    }
    /// <summary> Начало движения в определённую точку. </summary>
    public void Move(Vector3 target, float speed = float.NaN)
    {
        float finalSpeed;
        if (speed != float.NaN)
        {
            finalSpeed = speed;
        }
        else
        {
            finalSpeed = MaxSpeed;
        }
        
        if (maxAvailableSpeed() > 0)
        {
            if (maxAvailableSpeed() < finalSpeed)
            {
                CurMaxSpeed = maxAvailableSpeed();
            }
            else
            {
                CurMaxSpeed = finalSpeed;
            }
        }

        Target = target;
        Moving = true;
        if (Warping) WarpOut();
    }

    public void Warp(Vector3 target, float speed = float.NaN, float warpSpeed = float.NaN, List<Mobile> fleet = null)
    {
        float finalSpeed;
        float finalWarpSpeed;

        if (speed != float.NaN)
        {
            finalSpeed = speed;
        }
        else
        {
            finalSpeed = MaxSpeed;
        }
        
        if (warpSpeed != float.NaN)
        {
            finalWarpSpeed = warpSpeed;
        }
        else
        {
            finalWarpSpeed = MaxSpeed;
        }

        Vector3 targetDir = Target - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
                    
        if (angle < 5 && Vector3.Distance(transform.position,Target) > Owner.ObjectRadius * 10)
        {
            if (fleet == null)
            {
                Warping = true;
                return;
            }
        
            bool allReady = true;
        
            foreach (Mobile obj in fleet)
            {
                if (Vector3.Distance(obj.transform.position,obj.moveComponent.Target) < obj.ObjectRadius * 10) continue;
                Vector3 objTargetDir = obj.moveComponent.Target - obj.transform.position;
                float objAngle = Vector3.Angle(objTargetDir, obj.transform.forward);

                if(objAngle > 4.5f) allReady = false;
            }

            if (allReady)
            {
                if (fleetWarpIn > 0)
                {
                    fleetWarpIn -= Time.deltaTime;
                }
                else
                {
                    Warping = true;
                }
            }
        }
        
        if (maxAvailableSpeed() > 0)
        {
            if (maxAvailableSpeed() < finalSpeed)
            {
                CurMaxSpeed = maxAvailableSpeed();
            }
            else
            {
                CurMaxSpeed = finalSpeed;
            }
        }
        
        if (maxAvailableWarpSpeed() > 0)
        {
            CanWarp = true;
            if (maxAvailableWarpSpeed() < finalWarpSpeed)
            {
                CurMaxSpeed = maxAvailableWarpSpeed();
            }
            else
            {
                CurMaxSpeed = finalWarpSpeed;
            }
        }
        else
        {
            CanWarp = false;
        }

        Target = target;
        Moving = true;
    }
    
    /// <summary> Остановка. </summary>
    public void Stop()
    {
        if (Moving)
        {
            Target = gameObject.transform.position + (gameObject.transform.forward * DistanceToFullStop (Acceleration));
        }
        else
        {
            Pilot.Status = EngineController.State.Stopped;
        }
    }

    public void RotateShip(Vector3 target, bool toZero = false, bool revert = true)
    {
        Vector3 relativePos = transform.position - target;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos),Acceleration/5 * Time.deltaTime);
        
        RollShip(toZero, revert);
    }
    public void RotateShip(Quaternion target, bool toZero = false, bool revert = true)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, target,Acceleration/5 * Time.deltaTime);
        
        RollShip(toZero, revert);
    }
    
    /// <summary> Ограничение максимальной скорости </summary>
    private void ClipVelocity(float maxspeed)
    {
        _rigidbody.velocity = _rigidbody.velocity.Clip(maxspeed);
    }
    
    #endregion
}
