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

        /// <summary> Состояние Контроллера. </summary>
        public GunnerStatus Status;
    
        /// <summary> Механика системы в рабочем состоянии. </summary>
        public void Update()
        {
            STMethods.RemoveAllNullsFromList(Targets);
            STMethods.RemoveAllNullsFromList(TargetsUnderAttack);
            
            if (MainTarget != null)
            {
                if (!MainTarget.healthSystem || Vector3.Distance(transform.position, MainTarget.transform.position) > WeaponRange || MainTarget.destroyed)
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

            if (Owner.Alerts == STMethods.Alerts.RedAlert)
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


            if (Targets.Count > 0)
            {
                foreach (SelectableObject _targets in Targets)
                {
                    if (!_targets.healthSystem || Vector3.Distance(transform.position, _targets.transform.position) > WeaponRange || _targets.destroyed)
                    {
                        Targets.Remove(_targets);
                    }
                }
            }
        }
        /// <summary> Инициализация листа целей. </summary>
        void Start()
        {
            Targets = new List<SelectableObject>();

            WeaponRange = Owner.WeaponRange;
        }
        
        public enum GunnerStatus
        {
            Idle,
            Firing
        }
    }
}
