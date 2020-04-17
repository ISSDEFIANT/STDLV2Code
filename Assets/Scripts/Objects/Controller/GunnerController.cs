using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class GunnerController : MonoBehaviour
    {
        /// <summary> Владелец. </summary>
        public SelectableObject Owner;
        
        /// <summary> Радиус орудий. </summary>
        public float WeaponRange;
        
        /// <summary> Наведение орудий. </summary>
        public STMethods.AttackType Aiming;
        
        /// <summary> Все цели, доступные для стрельбы. </summary>
        public List<SelectableObject> Targets = new List<SelectableObject>();

        /// <summary> Цели под атакой. </summary>
        public List<SelectableObject> TargetsUnderAttack = new List<SelectableObject>();
        
        /// <summary> Главная цель для стрельбы. </summary>
        public SelectableObject MainTarget;

        /// <summary> Первичные орудия. </summary>
        public PrimaryWeaponSS PriWea;
        /// <summary> Вторичные орудия. </summary>
        public SecondaryWeaponSS SecWea;
        /// <summary> Сенсоры. </summary>
        public SensorSS SenSS;

        /// <summary> Состояние Контроллера. </summary>
        public GunnerStatus Status;
    
        /// <summary> Механика системы в рабочем состоянии. </summary>
        public void LateUpdate()
        {
            if (SenSS.EnemysInSensorRange().ToList().Count > 0)
            {
                Targets = SenSS.EnemysInSensorRange().ToList();
            }
            else
            {
                Targets.Clear();
            }

            STMethods.RemoveAllNullsFromList(TargetsUnderAttack);
            
            if (MainTarget != null)
            {
                if (!MainTarget.ThreateningEnemyObjects.Any(x => x == Owner))
                {
                    MainTarget.ThreateningEnemyObjects.Add(Owner);
                }
                if (!MainTarget.healthSystem || Vector3.Distance(transform.position, MainTarget.transform.position) > WeaponRange+MainTarget.ObjectRadius+Owner.ObjectRadius || MainTarget.destroyed)
                {
                    MainTarget = null;
                }
                else
                {
                    if(PriWea != null)PriWea.Attack(MainTarget,Aiming);
                    if(SecWea != null)SecWea.Attack(MainTarget,Aiming);

                    Status = GunnerStatus.Firing;
                }
            }

            if (Targets.Count > 0)
            {
                STMethods.RemoveAllNullsFromList(Targets);
                for(int i = Targets.Count - 1; i >= 0; i--) 
                {
                    if (!Targets[i].healthSystem || Vector3.Distance(transform.position, Targets[i].transform.position) > WeaponRange+Targets[i].ObjectRadius+Owner.ObjectRadius || Targets[i].destroyed)
                    {
                        Targets.Remove(Targets[i]);
                    }
                }
            }

            if (TargetsUnderAttack.Count <= 0)
            {
                Status = GunnerStatus.Idle;
            }
        }
        /// <summary> Инициализация листа целей. </summary>
        void Start()
        {
            Targets = new List<SelectableObject>();

            WeaponRange = Owner.WeaponRange;
        }

        public void OpenFireAtNearestEnemy()
        {
            if (TargetsUnderAttack.Count < Owner.MaxAttackTargetCount)
            {
                if (Targets.Count > 0)
                {
                    int targetNum = Random.Range(0, Targets.Count - 1);
                    if (PriWea != null) PriWea.AttackNotMainTarget(Targets[targetNum], Aiming);
                    if (SecWea != null) SecWea.AttackNotMainTarget(Targets[targetNum], Aiming);

                    Status = GunnerStatus.Firing;
                }
            }
        }
        
        public enum GunnerStatus
        {
            Idle,
            Firing
        }

        public SelectableObject GetNearestTarget()
        {
            float nearest = Mathf.Infinity;
            SelectableObject closest = null;

            foreach (SelectableObject all in Targets)
            {
                float distence = Vector3.Distance(all.transform.position, transform.position);

                if (distence < nearest)
                {
                    nearest = distence;
                    closest = all;
                }
            }

            return closest;
        }

        public void StopFiring()
        {
            if (PriWea != null) PriWea.StopFiring();
            if (SecWea != null) SecWea.StopFiring();
            MainTarget = null;
            Targets.Clear();
        }
    }
}
