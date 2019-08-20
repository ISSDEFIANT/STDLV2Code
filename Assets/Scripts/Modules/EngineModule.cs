using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

/// <summary> Компонент, отвечающий за перемещение корабля в 3D-пространстве. </summary>
[RequireComponent(typeof(Rigidbody))]
public class EngineModule : MonoBehaviour
{
    /// <summary> Двигается ли корабль. </summary>
    public bool Moving;
    
    /// <summary> Допустимая ошибка. </summary>
    public const float Threshold = 1f;
    
    /// <summary> Коэффициент крена </summary>
    protected const float RollSpeed = 0.05f;
    
    [Tooltip("Объект к которому надо лететь")]
    public Vector3 Target;
    
    [Tooltip("Ускорение корабля")]
    public float Acceleration = 20f;
    
    [Tooltip("Максимальная скорость корабля")]
    public float MaxSpeed = 30f;
    
    [Tooltip(("Может ли корабль двигаться"))]
    public bool CanMove = true;
    
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

    
    #region Unity event functions

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.angularDrag = 10f;
    }

    protected virtual void FixedUpdate()
    {
        if (CanMove)
        {
            if (Moving)
            {
                CalculateTargetParameters();
                MoveToTarget(Target);

                if (Vector3.Distance(transform.position, Target) < 0.5f)
                {
                    Moving = false;
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
            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
        }
        
        if (DistanceToTarget < Threshold
            && _rigidbody.velocity.magnitude < Threshold * Acceleration)
        {
            if (targetPosition.FloatEquals(transform.position)) return;
            
            _rigidbody.velocity = Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
            
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

    /// <summary>
    /// Рассчитывает относительную силу, которую надо приложить, чтобы корабль двигался в нужном направлении.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private Vector3 CalculateForce(Vector3 targetPosition)
    {
        Vector3 force = Vector3.zero;
        Vector3 velocity = _rigidbody.velocity;
        float speed = velocity.magnitude;
        bool accelerationNeeded = DistanceToTarget > DistanceToFullStop;
        
        // Если корабль стоит на месте, то сначала немного разгоняемся, чтобы поворот не выглядел резко
        if (accelerationNeeded && speed < MaxSpeed / 4)
        {
            force += Acceleration * Vector3.forward;
            return force;
        }
        
        // Если корабль направлен не на цель -- поворачиваем
        float errorAngle = Vector3.Angle(DirectionToTarget, velocity);
        float angleThreshold = AngularThreshold;
        if (errorAngle > angleThreshold)
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
            if (Math.Abs(speed - MaxSpeed) > float.Epsilon)
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

    private void Awake()
    {
        Target = transform.position;
    }
    /// <summary> Начало движения в определённую точку. </summary>
    public void Move(Vector3 target)
    {
        Target = target;
        Moving = true;
    }
    /// <summary> Остановка. </summary>
    public void Stop()
    {
        if (Moving)
        {
            Target = gameObject.transform.forward * _rigidbody.velocity.magnitude;
        }
    }
    
    /// <summary> Ограничение максимальной скорости </summary>
    private void ClipVelocity()
    {
        _rigidbody.velocity = _rigidbody.velocity.Clip(MaxSpeed);
    }
    
    #endregion
    }

/// <summary> Расширение класса Vector3 дополнительными матетматическми функциями. </summary>
public static class VectorExtensions
{
    /// <summary> Проецирует вектор на ось. </summary>
    /// <param name="vector"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static float ProjectOn(this Vector3 vector, Vector3 axis)
    {
        return Mathf.Cos(Vector3.Angle(vector, axis) * Mathf.Deg2Rad) * vector.magnitude / axis.magnitude;
    }

    /// <summary> Обрезает вектор, до определённой длины. </summary>
    /// <param name="vector"></param>
    /// <param name="maxMagnitude"> Длина до которой нужно обрезать. </param>
    /// <returns> Обрезанный вектор. </returns>
    public static Vector3 Clip(this Vector3 vector, float maxMagnitude = 1.0f)
    {
        if (vector.magnitude > maxMagnitude)
        {
            return vector.normalized * maxMagnitude;
        }

        return vector;
    }

    /// <summary> Проверяет равенство двух векторов с учетом неточности значений типа float. </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool FloatEquals(this Vector3 vector, Vector3 other)
    {
        return Math.Abs(vector.x - other.x) < float.Epsilon
               && Math.Abs(vector.y - other.y) < float.Epsilon
               && Math.Abs(vector.z - other.z) < float.Epsilon;
    }
}