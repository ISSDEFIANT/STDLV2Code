using UnityEngine;

namespace Modules
{
    /// <summary> Модуль отвечающий за перемещение в пространстве. </summary>
    public class Engine : Module
    {
        #region Constants

        /// <summary> Допустимая ошибка. </summary>
        public const float Threshold = 1f;
    
        /// <summary> Коэффициент крена </summary>
        protected const float RollSpeed = 0.05f;

        #endregion
        
        public enum State
        {
            Disabled,
            Stopped,
            Stopping,
            Arriving,
            Fleeing,
            Pursuing,
            OffsetPursuing,
            Orbiting,
            LeaderFollowing,
            MovingInFleet,
            AttackingAlpha,
            AttackingBeta,
            AttackingGamma,
            Covering,
            Hiding,
        }

        /// <summary> Состояние модуля. </summary>
        public State Status = State.Stopped;

        /// <summary> Двигается ли корабль.  </summary>
        public bool IsMoving => Status != State.Disabled && Status != State.Stopped;
        
        /// <summary> Ускорение корабля </summary>
        [Tooltip("Ускорение корабля")]
        public float Acceleration = 20f;
    
        /// <summary> Максимальная скорость корабля </summary>
        [Tooltip("Максимальная скорость корабля")]
        public float MaxSpeed = 30f;
    
        /// <summary> Дочерний объект с мешем корабля </summary>
        [Tooltip("Дочерний объект с мешем корабля")]
        public Transform Model;

        /// <summary> Максимальный угол крена </summary>
        [Tooltip("Максимальный угол крена")]
        public float MaxRollAngle = 30f;

        /// <summary> Координаты цели. </summary>
        public Vector3 TargetPosition { get; protected set; }
        
        /// <summary> Прибытие в точку. </summary>
        /// <remarks> Точка может перемещаться. </remarks>
        /// <param name="target"> Объект, к которому надо прилететь. </param>
        public void Arrival(Transform target)
        {
            Status = State.Arriving;
        }

        /// <summary> Прибытие в точку. </summary>
        /// <param name="target"> Координаты точки. </param>
        public void Arrival(Vector3 target)
        {
            Status = State.Arriving;
        }

        /// <summary> Остановка. </summary>
        public void Stop()
        {
            Status = State.Stopping;
        }

        /// <summary> Уклонение от цели. </summary>
        /// <remarks> Цель может перемещаться. </remarks>
        /// <param name="target"> Объект, от которого надо уклоняться. </param>
        public void Flee(Transform target)
        {
            Status = State.Fleeing;
        }

        /// <summary> Уклонение от цели. </summary>
        /// <param name="target"> Координаты цели. </param>
        public void Flee(Vector3 target)
        {
            Status = State.Fleeing;
        }

        /// <summary> Преследование цели. </summary>
        /// <param name="target"> Цель. </param>
        public void Pursuit(SelectableObject target)
        {
            Status = State.Pursuing;
        }

        /// <summary> Преследование цели на расстоянии. </summary>
        /// <param name="target"> Цель. </param>
        /// <param name="offset"> Расстояние до цели. </param>
        public void OffsetPursuit(SelectableObject target, float offset)
        {
            Status = State.OffsetPursuing;
        }

        /// <summary> Полёт вокруг цели. </summary>
        /// <remarks> Цель может перемещаться. </remarks>
        /// <param name="target"> Цель. </param>
        /// <param name="radius"> Радиус орбиты. </param>
        public void Orbiting(SelectableObject target, float radius)
        {
            Status = State.Orbiting;
        }

        /// <summary> Полёт вокруг цели. </summary>
        /// <remarks> Цель может перемещаться. </remarks>
        /// <param name="target"> Цель. </param>
        /// <param name="radius"> Радиус орбиты. </param>
        public void Orbiting(Transform target, float radius)
        {
            Status = State.Orbiting;
        }

        /// <summary> Полёт вокруг цели. </summary>
        /// <param name="target"> Цель. </param>
        /// <param name="radius"> Радиус орбиты. </param>
        public void Orbiting(Vector3 target, float radius)
        {
            Status = State.Orbiting;
        }

        /// <summary> Следование за лидером. </summary>
        /// <param name="leader"> Корабль - цель. </param>
        public void LeaderFollowing(SelectableObject leader)
        {
            Status = State.LeaderFollowing;
        }

        /// <summary> Движение во флоте. </summary>
        public void FleetMovement()
        {
            Status = State.MovingInFleet;
        }

        /// <summary> Атака по паттерну Альфа </summary>
        /// <param name="target"> Цель. </param>
        public void AttaсkAlpha(SelectableObject target)
        {
            Status = State.AttackingAlpha;
        }

        /// <summary> Атака по паттерну Бета. </summary>
        /// <param name="target"> Цель. </param>
        public void AttackBeta(SelectableObject target)
        {
            Status = State.AttackingBeta;
        }

        /// <summary> Атака по паттерну Гамма. </summary>
        /// <param name="target"> Цель. </param>
        public void AttackGamma(SelectableObject target)
        {
            Status = State.AttackingGamma;
        }

        /// <summary> Прикрытие. </summary>
        /// <param name="covering"> Цель, которую нужно прикрывать. </param>
        /// <param name="enemy"> Цель, от которой нужно защищать. </param>
        public void Cover(SelectableObject covering, SelectableObject enemy)
        {
            Status = State.Covering;
        }

        /// <summary> Спрятаться. </summary>
        /// <param name="cover"> Цель, за которой нужно прятаться. </param>
        /// <param name="enemy"> Цель, от которой нужно прятаться. </param>
        public void Hide(SelectableObject cover, SelectableObject enemy)
        {
            Status = State.Hiding;
        }
    }
}