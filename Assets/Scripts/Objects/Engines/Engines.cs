using System;
using Controllers;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

/// <summary> Компонент, отвечающий за перемещение корабля в 3D-пространстве. </summary>
[RequireComponent(typeof(Rigidbody))]
public class Engines : MonoBehaviour
{
    public EngineController Pilot;
    
    /// <summary> Двигается ли корабль. </summary>
    public bool Moving;
    
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

    public float CurForwardSpeed;

    public SubSystem impulseControlingSubSystem;
    public SubSystem warpControlingSubSystem;
    public SubSystem warpcoreControllingSystem;

    public bool CanMove()
    {
        if (Owner.healthSystem)
        {
            if (!Owner.destroyed)
            {
                return impulseControlingSubSystem.efficiency > 0.1f;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public bool CanWarp()
    {
        if (Owner.healthSystem)
        {
            if (warpcoreControllingSystem.efficiency < 0.1f)
            {
                return false;
            }
            else
            {
                return warpControlingSubSystem.efficiency > 0.1f;   
            }
        }
        return true;
    }
    
    [Tooltip("Дочерний объект с мешем корабля")]
    public Transform Model;

    [Tooltip("Максимальный угол крена")]
    public float MaxRollAngle = 30f;
    
    /// <summary> Нормированный вектор направления на цель </summary>
    public Vector3 DirectionToTarget { get; protected set; }
    
    /// <summary> Расстояние до цели. </summary>
    public float DistanceToTarget { get; protected set; }

    /// <summary> Расстояние которое пройдет корабль до остановки. </summary>
    public float DistanceToFullStop
    {
        get
        {
            float velocity = _rigidbody.velocity.magnitude;
            return velocity * velocity / 2 / Acceleration;
        }
    }

    /// <summary> Допустимая ошибка по углу. </summary>
    /// <remarks> Зависит от расстояния до цели. </remarks>
    public float AngularThreshold => 2 * Mathf.Acos(Threshold / DistanceToTarget);

    public Mobile Owner;
    
    #region Unity event functions

    // Start is called before the first frame update
    protected virtual void Start()
    {
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
    }

    protected virtual void FixedUpdate()
    {
        if (CanMove())
        {
            var localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
            CurForwardSpeed = Mathf.Max(0, localVelocity.z);
            
            if (Moving)
            {
                CalculateTargetParameters();
                MoveToTarget(Target);

                if (DistanceToTarget < Threshold && _rigidbody.velocity.magnitude < Threshold * Acceleration)
                {
                    Moving = false;
                    Pilot.Status = EngineController.State.Stopped;
                }
            }
            else
            {
                if (_rigidbody.velocity != new Vector3(0, 0, 0))
                {
                    stopAllMovment();
                }
            }
        }
        else
        {
            Moving = false;
        }
    }

    #endregion

    #region Private definitions

    private Rigidbody _rigidbody;

    /// <summary> Функция движения к цели </summary>
    /// <remarks> Должна вызываться в FixedUpdate </remarks>
    /// <param name="targetPosition"> Координаты цели. </param>
    private void MoveToTarget(Vector3 targetPosition)
    {
        ClipVelocity();

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
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);

            if (Owner.captain.Command == Captain.PlayerCommand.Move)
                Owner.captain.Command = Captain.PlayerCommand.None;
            
            RollShip(true);
            return;
        }

        RollShip();
        
        Vector3 force = CalculateForce(targetPosition);
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
    private Vector3 CalculateForce(Vector3 targetPosition)
    {
        Vector3 force = Vector3.zero;
        Vector3 velocity = _rigidbody.velocity;
        float speed = CurForwardSpeed;
        bool accelerationNeeded = DistanceToTarget > DistanceToFullStop;
        
        // Если корабль направлен не на цель -- поворачиваем
        float errorAngle = Vector3.Angle(DirectionToTarget, velocity);
        float angleThreshold = AngularThreshold;

        float angleToTarget = Vector3.Angle(DirectionToTarget, transform.forward);
        
        // Если корабль стоит на месте, то сначала немного разгоняемся, чтобы поворот не выглядел резко
        if (accelerationNeeded && speed < CurMaxSpeed / 4)
        {
            if (angleToTarget < 10 || speed > Threshold)
            {
                force += Acceleration * Vector3.forward;
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
            float rotationRadius = speed * speed / Acceleration;
            Vector3 circleCenter = transform.position + forceDir * rotationRadius;
            if ((targetPosition - circleCenter).magnitude < rotationRadius
                || !accelerationNeeded)
            {
                force -= Acceleration * Vector3.forward;
            }
            return force;
        }
        
        if (accelerationNeeded)
        {
            if (Math.Abs(speed - CurMaxSpeed) > float.Epsilon)
            {
                force += Acceleration * Vector3.forward;
            }
        }
        else
        {
            force -= Acceleration * Vector3.forward;
        }
        
        return force;
    }

    /// <summary> Вычисляет и применяет крен корабля во время поворота. </summary>
    /// <param name="toZero"> true -- если надо убрать крен. </param>
    private void RollShip(bool toZero = false)
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
        
        rollAngle *= -Mathf.Sign(directionToTarget.ProjectOn(transform.right));
        
        Model.transform.localRotation = Quaternion.Lerp(Model.transform.localRotation,
                                                        Quaternion.Euler(0, 0, rollAngle),
                                                        RollSpeed);
    }

    private void ShiftShip(Vector3 target)
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
    }

    private void stopAllMovment()
    {
        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime);
        _rigidbody.angularVelocity = Vector3.Lerp(_rigidbody.angularVelocity, Vector3.zero, Time.deltaTime);
    }

    private void Awake()
    {
        Target = transform.position;
    }
    /// <summary> Начало движения в определённую точку. </summary>
    public void Move(Vector3 target, float speed = float.NaN)
    {
        if (speed != float.NaN)
        {
            CurMaxSpeed = speed;
        }
        else
        {
            CurMaxSpeed = MaxSpeed;
        }

        Target = target;
        Moving = true;
    }
    
    /// <summary> Остановка. </summary>
    public void Stop()
    {
        if (Moving)
        {
            Target = gameObject.transform.position + (gameObject.transform.forward * _rigidbody.velocity.magnitude);
        }
    }

    public void RotateShip(Vector3 target)
    {
        Vector3 relativePos = transform.position - target;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos),Acceleration/5 * Time.deltaTime);
        
        RollShip();
    }
    public void RotateShip(Quaternion target)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, target,Acceleration/5 * Time.deltaTime);
        
        RollShip();
    }
    
    /// <summary> Ограничение максимальной скорости </summary>
    private void ClipVelocity()
    {
        _rigidbody.velocity = _rigidbody.velocity.Clip(CurMaxSpeed);
    }
    
    #endregion
}
