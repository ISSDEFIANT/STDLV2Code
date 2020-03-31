using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    /// <summary> Модуль отвечающий за перемещение в пространстве. </summary>
    public class EngineController : MonoBehaviour
    {
        /// <summary> Владелец. </summary>
        public Mobile Owner;
        
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

        public Engines engines;

        /// <summary> Состояние модуля. </summary>
        public State Status = State.Stopped;

        /// <summary> Двигается ли корабль.  </summary>        
        public bool IsMoving => Status != State.Disabled && Status != State.Stopped;        

        /// <summary> Координаты цели. </summary>
        public Vector3 TargetPosition { get; protected set; }

        public WaypointCircuit PatternAlpha;
        public WaypointCircuit PatternBeta;
        public WaypointCircuit PatternGamma;

        private WaypointProgressTracker _pt;

        void Awake()
        {
            GameObject alpha = (GameObject)Instantiate(Resources.Load("ManeuverPatterns/Single/RoundWay"), Vector3.zero, Quaternion.identity);
            GameObject beta = (GameObject)Instantiate(Resources.Load("ManeuverPatterns/Single/BType"), Vector3.zero, Quaternion.identity);
            GameObject gamma = (GameObject)Instantiate(Resources.Load("ManeuverPatterns/Single/Flower"), Vector3.zero, Quaternion.identity);
            
            PatternAlpha = alpha.GetComponent<WaypointCircuit>();
            PatternBeta = beta.GetComponent<WaypointCircuit>();
            PatternGamma = gamma.GetComponent<WaypointCircuit>();

            _pt = gameObject.AddComponent<WaypointProgressTracker>();
            GameObject tracker = new GameObject();
            tracker.name = "WayTracker";
            tracker.transform.parent = transform;
            _pt.target = tracker.transform;
        }

        private void OnDestroy()
        {
            Destroy(PatternAlpha);
            Destroy(PatternBeta);
            Destroy(PatternGamma);
        }

        /// <summary> Прибытие в точку. </summary>
        /// <remarks> Точка может перемещаться. </remarks>
        /// <param name="target"> Объект, к которому надо прилететь. </param>
        public void Arrival(Transform target)
        {
            Status = State.Arriving;
        }

        /// <summary> Прибытие в точку. </summary>
        /// <param name="target"> Координаты точки. </param>
        public void Arrival(Vector3 target, float speed = 0f)
        {
            Status = State.Arriving;

            float distance = (target - transform.position).magnitude;
            if (distance > Owner.Threshold)
            {
                if (speed == 0)
                {
                    engines.Move(target, engines.MaxSpeed);
                }
                else
                {
                    engines.Move(target, speed);
                }
            }
            else
            {
                Stop();
            }

        }

        /// <summary> Остановка. </summary>
        public void Stop()
        {
            Status = State.Stopping;
            
            engines.Stop();
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
            _pt.circuit = PatternAlpha;
            PatternAlpha.transform.position = target.transform.position;
            PatternAlpha.transform.localScale = new Vector3(radius,radius,radius);
            
            engines.Move(_pt.target.position, engines.MaxSpeed);
            
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
        public void FleetMovement(Vector3 targetPosition, List<Mobile> fleet)
        {
            Status = State.MovingInFleet;
            
            float minSpeed = fleet.Select(shipGO => shipGO.GetComponent<Engines>()).Min(moveComp => moveComp.MaxSpeed);
            engines.Move(targetPosition, minSpeed);
        }

        /// <summary> Атака по паттерну Альфа </summary>
        /// <param name="target"> Цель. </param>
        public void AttaсkAlpha(SelectableObject target, GunnerController gunner, float speed = 0f)
        {
            float radius = engines.Owner.ObjectRadius+target.ObjectRadius+gunner.WeaponRange*0.75f;
            
            _pt.circuit = PatternAlpha;
            PatternAlpha.transform.position = target.transform.position;
            PatternAlpha.transform.localScale = new Vector3(radius,radius,radius);

            if (speed == 0)
            {
                engines.Move(_pt.target.position, engines.MaxSpeed);
            }
            else
            {
                engines.Move(_pt.target.position, speed);
            }

            Status = State.AttackingAlpha;
        }

        /// <summary> Атака по паттерну Бета. </summary>
        /// <param name="target"> Цель. </param>
        public void AttackBeta(SelectableObject target, GunnerController gunner, float speed = 0f)
        {
            float radius = engines.Owner.ObjectRadius+target.ObjectRadius+gunner.WeaponRange*0.75f;
            
            _pt.circuit = PatternBeta;
            PatternBeta.transform.position = target.transform.position;
            PatternBeta.transform.localScale = new Vector3(radius,radius,radius);
            
            if (speed == 0)
            {
                engines.Move(_pt.target.position, engines.MaxSpeed);
            }
            else
            {
                engines.Move(_pt.target.position, speed);
            }
            
            Status = State.AttackingBeta;
        }

        /// <summary> Атака по паттерну Гамма. </summary>
        /// <param name="target"> Цель. </param>
        public void AttackGamma(SelectableObject target, GunnerController gunner, float speed = 0f)
        {
            float radius = engines.Owner.ObjectRadius+target.ObjectRadius+gunner.WeaponRange*0.75f;
            
            _pt.circuit = PatternGamma;
            PatternGamma.transform.position = target.transform.position;
            PatternGamma.transform.localScale = new Vector3(radius,radius,radius);
            
            if (speed == 0)
            {
                engines.Move(_pt.target.position, engines.MaxSpeed);
            }
            else
            {
                engines.Move(_pt.target.position, speed);
            }
            
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