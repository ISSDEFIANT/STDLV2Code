using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Controllers;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class Captain : MonoBehaviour
{
    public enum PlayerCommand
    {
        None,
        FullStop,
        Attack,
        Guard,
        Move,
        Cover,
        Hide,
        Patrol,
        SeekAndDestroy,
        Mine,
        Trade,
        FixIn,
        Deassamble,
        Build,
        Undocking,
        SettingAbilityTarget
    }

    public PlayerCommands curCommandInfo;

    public SelectableObject tarSelObj;
    public Vector3 tarVec;

    /// <summary> Команда, указанная игроком. </summary>
    public PlayerCommand Command = PlayerCommand.None;

    public SelectableObject Owner;
    public Mobile MobileOwner;
    public SensorSS Sensors;

    public GunnerController Gunner = null;
    public EngineController Pilot = null;
    public MiningController Miner = null;
    public BuilderConstructionController Builder;
    public DockingConstructionTypeController ShipBuilder;
    public LevelUpController LevelUpdater;

    private float fleetStabTimer = 3;
    private int PatrolPositionNum = 0;

    public bool ToEnterPoint;
    // Движется ли к доку
    public bool ToStayPoint;
    // Выходит ли из дока
    public bool ToExitPoint;
    public SelectableObject dockingStation;
    public DockingHub dockingHub;

    public float DeassebleTime = -1;
    public bool BeginDeassebling;
    public GameObject deassambleAnim;

    public AttackingPattern AttackingPattern = AttackingPattern.None;

    private Vector3 curFlagPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Owner.captain = this;

        // дупликация контроллеров - так делать не стоит. Если такого контроллера нет - всё равно вернётся null
        Gunner = gameObject.GetComponent<GunnerController>();
        Pilot = gameObject.GetComponent<EngineController>();
        Miner = gameObject.GetComponent<MiningController>();
        Builder = gameObject.GetComponent<BuilderConstructionController>();
        ShipBuilder = gameObject.GetComponent<DockingConstructionTypeController>();
        LevelUpdater = gameObject.GetComponent<LevelUpController>();

        // Запоминаем Owner'a как Mobile, чтобы постоянно не делать as, и не триггерить проверки типов.
        MobileOwner = (Owner is Mobile) ? (Owner as Mobile) : null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Command == PlayerCommand.None)
        {
            // STMethods наверно не лучшее название для структур и enum'ов. Может объекты стоит выделить в STTypes? Чтобы не путать где методы и хэлперы, а где типы данных.
            if (Owner.Alerts == STMethods.Alerts.GreenAlert)
            {
                // Enemies, а не Enemys
                if (Sensors.EnemysInSensorRange().Any())
                {
                    // пустые куски надо бы выкинуть
                }
            }
            else if (Owner.Alerts == STMethods.Alerts.YellowAlert)
            {
                if (Sensors.EnemysInSensorRange().Any())
                {
                    // пустые куски надо бы выкинуть
                }

                // Если EnemysInSensorRange() и ThreateningEnemyObjects это списки, то лучше использовать метод Any() а не Count > 0 - так списку не придёться перевычислять количество объектов, 
                //      а выдаст сразу true если есть хотя бы один
                if (Owner.ThreateningEnemyObjects.Any())
                {
                    // Если позволяет Unity использовать современные конструкции C#, то можно сократить вот так. Если нет, то пускай будет как есть
                    Gunner?.OpenFireAt(STMethods.NearestSelObj(Owner, Owner.ThreateningEnemyObjects));

                    // if (Gunner != null)
                    // {
                    //     Gunner.OpenFireAt(STMethods.NearestSelObj(Owner, Owner.ThreateningEnemyObjects));
                    // }
                }
            }
            else if (Owner.Alerts == STMethods.Alerts.RedAlert)
            {
                if (Sensors.EnemysInSensorRange().Any())
                {
                    if (Gunner != null)
                    {
                        if (Pilot != null)
                        {
                            // Периодически есть проверки на то, что `Owner is Mobile`, а тут нет. Что странно.

                            // Вот это поменять бы для большей читаемости.
                            // ow.TimelyFleet.Count -> ow.TimelyFleet.Any()
                            // ow.TimelyFleet[0] ->ow.TimelyFleet.First()

                            // И почему первый элемент в ow.TimelyFleet должен быть обяязательно Owner? А вдруг это не так? Как это валидируется (проверяется на правильность)?

                            // Также есть вот такая проверка в нескольких кусках кода
                            //          ow.TimelyFleet != null && ow.TimelyFleet.Count > 0 && ow.TimelyFleet[0] == Owner
                            //      стоит это выделить в метод. Если я правильно понимаю, то это проверка на то, является ли корабль "Адмиралом флота", 
                            //          так что выделить метод IsAdmiral() например можно. И туда же проверку на `Owner is Mobile`

                            if (IsAdmiralOfFleet())
                            {
                                AdmiralControlling(MobileOwner.TimelyFleet);
                            }
                            else
                            {
                                if (Pilot.Status != EngineController.State.Disabled)
                                {
                                    if (Gunner.GetNearestTarget() != null)
                                    {
                                        AttackTarget(Gunner.GetNearestTarget(), Gunner);
                                    }
                                    else
                                    {
                                        if (Gunner.MainTarget != null && Gunner.Targets.Any())
                                        {
                                            AttackingPattern = AttackingPattern.None;
                                        }
                                    }
                                }
                            }
                        }

                        Gunner.OpenFireAtNearestEnemy();
                    }
                }

                if (Owner.ThreateningEnemyObjects.Any())
                {
                    SelectableObject tar = STMethods.NearestSelObj(Owner, Owner.ThreateningEnemyObjects);
                    if (Pilot != null)
                    {
                        if (IsAdmiralOfFleet())
                        {
                            AdmiralControlling(MobileOwner.TimelyFleet);
                        }
                        else
                        {
                            if (Gunner != null)
                            {
                                if (tar != null)
                                {
                                    AttackTarget(tar, Gunner);
                                }
                                else
                                {
                                    if (Gunner.MainTarget != null && Gunner.Targets.Any())
                                    {
                                        AttackingPattern = AttackingPattern.None;
                                    }
                                }

                                Gunner.OpenFireAt(STMethods.NearestSelObj(Owner, Owner.ThreateningEnemyObjects));
                            }
                        }
                    }
                    else
                    {
                        Gunner.OpenFireAt(tar);
                    }
                }
            }

            if (OwnerIsMobile())
            {
                if (IsPartOfFleet())
                {
                    if (!Pilot.engines.Moving)
                    {
                        float angleToTarget = Vector3.Angle(MobileOwner.TimelyFleet[0].transform.forward, transform.forward);
                        if (angleToTarget > 2)
                        {
                            if (fleetStabTimer > 0)
                            {
                                fleetStabTimer -= Time.deltaTime;
                            }
                            else
                            {
                                Pilot.engines.RotateShip(MobileOwner.TimelyFleet[0].transform.rotation);
                            }
                        }
                        else
                        {
                            fleetStabTimer = 3;
                        }
                    }
                }
            }
        }
        else
        {
            PerformCommand();
        }
    }

    public AttackingPattern ChangeAttackPattern(float probability)
    {
        if (probability <= MobileOwner.AttackProbability.AlphaProbability)
        {
            return AttackingPattern.Alpha;
        }

        if (probability > MobileOwner.AttackProbability.AlphaProbability && probability <= MobileOwner.AttackProbability.BetaProbability)
        {
            return AttackingPattern.Beta;
        }

        if (probability > MobileOwner.AttackProbability.BetaProbability && probability <= MobileOwner.AttackProbability.GammaProbability)
        {
            return AttackingPattern.Gamma;
        }

        return AttackingPattern.None;
    }

    // Отдача приказов флоту, если корабль является адмиралом флота
    // Выглядит так что Fleet
    public void AdmiralControlling(List<Mobile> Fleet, PlayerCommands command = null)
    {
        if (Owner.destroyed)
        {
            foreach (Mobile obj in Fleet)
            {
                if (obj.TimelyFleet.Any(x => x == Owner)) obj.TimelyFleet.Remove(MobileOwner);
            }
        }

        // Если корабль адмиральский, то этот метод будет вызываться постоянно, и здесь надо подумать над оптимизацией
        // Постоянно создание List'ов вредит, Garbage Collector'у придётся постоянно вычищать память. 
        //      А также если размер List'а недостаточно большой (у него есть внутренний размер), то надо увеличиваться, что тоже вредно.
        //      Как вариант, можно при создании указывать их изначальный размер, если есть понимание сколько там может быть элемементов.
        //      Если, в теории, все элементы могут попасть туда (например все корабли не работают и попадают в ShipsCannotFight), и этот размер не особо большой (пускай пару десятков элементов),
        //          то можно указать этот размер, так этому списку не придёться увеличиваться и тормозить из-за этого

        // ShipsCanotFight -> ShipsCannotFight (ну или ShipsCantFight)
        List<Mobile> ShipsCannotFight = new List<Mobile>();
        List<Mobile> ShipsCanFight = new List<Mobile>();
        List<Mobile> ShipsWithoutShield = new List<Mobile>();
        List<Mobile> DamagedShips = new List<Mobile>();

        List<SelectableObject> FleetAttackingList = new List<SelectableObject>();
        List<SelectableObject> ThreateningList = new List<SelectableObject>();

        AdmiralFleetAnalize(Fleet, ShipsCannotFight, ShipsCanFight, ShipsWithoutShield, DamagedShips, ThreateningList);
        AdmiralEnemyAnalyze(ShipsCanFight, FleetAttackingList);

        /*STMethods.RemoveAllNullsFromList(FleetAttackingList);

        STMethods.RemoveAllNullsFromList(ShipsCannotFight);
        STMethods.RemoveAllNullsFromList(ShipsCanFight);
        STMethods.RemoveAllNullsFromList(ShipsWithoutShield);
        STMethods.RemoveAllNullsFromList(DamagedShips);*/
        
        CoverShipsThatCannotFight(command, ShipsCannotFight, ShipsCanFight, DamagedShips);

        if (command == null)
        {
            AttackShipsWithoutShields(FleetAttackingList, ShipsCanFight);
            ReturnFire(ThreateningList, ShipsCanFight);
        }
        else
        {
            // Не стоит использовать строковые варианты. Надо выделить строку в enum какой-нибудь
            switch (command.command)
            {
                case "Attack":
                    AttackCommand _ac = curCommandInfo as AttackCommand;
                    if (ShipsCanFight.Any())
                    {
                        foreach (Mobile _s in ShipsCanFight)
                        {
                            if (_s.captain.Command == PlayerCommand.None)
                            {
                                AttackCommand com = new AttackCommand();
                                com.command = "Attack";
                                com.attackTarget = _ac.attackTarget;
                                _s.captain.EnterCommand(com);
                            }
                        }
                    }

                    break;
                case "Move":
                    MoveCommand _mc = curCommandInfo as MoveCommand;
                    foreach (Mobile all in Fleet)
                    {
                        for (int i = Fleet.Count - 1; i >= 0; i--)
                        {
                            if (Fleet[i] == all)
                            {
                                all.captain.tarVec = _mc.targetVec[i];
                                all.captain.Pilot.Arrival(_mc.targetVec[i], _mc.Warp, true, true, Fleet);
                            }
                        }
                    }
                    break;
            }
        }
    }

    private static void ReturnFire(List<SelectableObject> ThreateningList, List<Mobile> ShipsCanFight)
    {
        if (ThreateningList.Any())
        {
            foreach (Mobile _s in ShipsCanFight)
            {
                if (_s.captain.Gunner.MainTarget._hs.Shilds != null && _s.captain.Gunner.MainTarget._hs.Shilds.Length > 0 &&
                    _s.captain.Gunner.MainTarget._hs.Shilds[0].SubSystemCurHealth > 0 &&
                    _s.captain.Gunner.MainTarget != ThreateningList[0])
                {
                    _s.captain.Gunner.MainTarget = ThreateningList[0];
                }
            }
        }
    }

    private static void AttackShipsWithoutShields(List<SelectableObject> FleetAttackingList, List<Mobile> ShipsCanFight)
    {
        foreach (SelectableObject tar in FleetAttackingList)
        {
            if (tar.healthSystem && tar._hs.Shilds.Length > 0 && tar._hs.Shilds[0].SubSystemCurHealth <= 0)
            {
                foreach (Mobile _s in ShipsCanFight)
                {
                    if (_s.captain.Gunner.MainTarget._hs.Shilds != null &&
                        _s.captain.Gunner.MainTarget._hs.Shilds.Length > 0 &&
                        _s.captain.Gunner.MainTarget._hs.Shilds[0].SubSystemCurHealth > 0)
                    {
                        _s.captain.Gunner.MainTarget = tar;
                    }
                }
            }
        }
    }

    private static void CoverShipsThatCannotFight(PlayerCommands command, List<Mobile> ShipsCannotFight, List<Mobile> ShipsCanFight,
        List<Mobile> DamagedShips)
    {
        foreach (Mobile _scnf in ShipsCannotFight)
        {
            if (_scnf.destroyed) continue;
            if (_scnf.ThreateningEnemyObjects.Any())
            {
                List<SelectableObject> convertedList =
                    ShipsCanFight.ConvertAll(new Converter<Mobile, SelectableObject>(STMethods.MobileToSelectableObject));
                Mobile coverignShip = STMethods.NearestSelObj(_scnf, convertedList) as Mobile;

                HideCoverCommand com = new HideCoverCommand();
                // Не стоит использовать строковые варианты. Надо выделить строку 
                com.command = "Hide";
                com.coverignShip = coverignShip;
                com.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects.First();
                com.MainCommand = command;
                _scnf.captain.EnterCommand(com);

                if (command == null)
                {
                    HideCoverCommand com2 = new HideCoverCommand();
                    com2.command = "Cover";
                    com2.coverignShip = _scnf;
                    com2.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects.First();
                    com2.MainCommand = command;
                    coverignShip.captain.EnterCommand(com2);
                }
            }
        }

        foreach (Mobile _scnf in DamagedShips)
        {
            if (_scnf.destroyed) continue;
            if (_scnf.ThreateningEnemyObjects.Any())
            {
                List<SelectableObject> convertedList =
                    ShipsCanFight.ConvertAll(new Converter<Mobile, SelectableObject>(STMethods.MobileToSelectableObject));
                Mobile coverignShip = STMethods.NearestSelObj(_scnf, convertedList) as Mobile;

                HideCoverCommand com = new HideCoverCommand();
                com.command = "Hide";
                com.coverignShip = coverignShip;
                com.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects.First();
                com.MainCommand = command;
                _scnf.captain.EnterCommand(com);

                HideCoverCommand com2 = new HideCoverCommand();
                com2.command = "Cover";
                com2.coverignShip = _scnf;
                com2.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects.First();
                com2.MainCommand = command;
                coverignShip.captain.EnterCommand(com2);
            }
        }
    }

    private static void AdmiralEnemyAnalyze(List<Mobile> ShipsCanFight, List<SelectableObject> FleetAttackingList)
    {
        foreach (Mobile _scf in ShipsCanFight)
        {
            if (_scf.destroyed) continue;
            foreach (SelectableObject tar in _scf.captain.Gunner.TargetsUnderAttack)
            {
                if (tar != null && tar.destroyed)
                {
                    if (FleetAttackingList.Any(x => x == tar))
                    {
                        FleetAttackingList.Remove(tar);
                    }

                    continue;
                }

                if (tar != null && !FleetAttackingList.Any(x => x == tar))
                {
                    FleetAttackingList.Add(tar);
                }
            }
        }
    }

    private static void AdmiralFleetAnalize(List<Mobile> Fleet, List<Mobile> ShipsCannotFight, List<Mobile> ShipsCanFight, List<Mobile> ShipsWithoutShield,
        List<Mobile> DamagedShips, List<SelectableObject> ThreateningList)
    {
        foreach (Mobile _s in Fleet)
        {
            // может не `return`, а `continue` ? чтобы если корабль уничтожен, то перейти к следующему, а не покидать весь метод
            if (_s.destroyed) continue;
            PrimaryWeaponSS _pw = _s._hs.SubSystems.First().gameObject.GetComponent<PrimaryWeaponSS>();
            ;
            SecondaryWeaponSS _sw = _s._hs.SubSystems.First().gameObject.GetComponent<SecondaryWeaponSS>();

            // постоянные проверки на if(!...Any()) ...Add() могут быть проблемой если в коллекции очень много элементов.
            // к тому же, стоит уменьшить количество проверок и добавлений если это возможно
            // Предлагаю логику по добавлению и проверки вынести в конец всей этой логики, а необходимость в добавлении/удалении запоминать в bool переменных
            // TODO: Вынести логику с добавлением кораблей в список как можно дальше, вынести флаги о необходимости добавления/удаления в конец

            if (_pw == null && _sw == null)
            {
                ShipsCannotFight.Add(_s);
            }
            else
            {
                float pwe = 0;
                float swe = 0;
                if (_pw != null)
                {
                    pwe = _pw.efficiency;
                }

                if (_sw != null)
                {
                    swe = _sw.efficiency;
                }

                if (pwe <= 0 && swe <= 0)
                {
                    ShipsCannotFight.Add(_s);
                }
                else
                {
                    ShipsCanFight.Add(_s);
                }
            }

            if (_s._hs.Shilds.Length > 0)
            {
                if (_s._hs.Shilds[0].SubSystemCurHealth <= 0)
                {
                    ShipsWithoutShield.Add(_s);
                }
            }
            else
            {
                ShipsWithoutShield.Add(_s);
            }

            if (_s._hs.curHull <= _s._hs.MaxHull / 2)
            {
                DamagedShips.Add(_s);
            }

            if (_s._hs.MaxCrew > 0)
            {
                if (_s._hs.curCrew <= _s._hs.MaxHull / 3)
                {
                    DamagedShips.Add(_s);
                }
            }

            if (_s.ThreateningEnemyObjects.Any())
            {
                foreach (SelectableObject obj in _s.ThreateningEnemyObjects)
                {
                    ThreateningList.Add(obj);
                }
            }
        }
    }

    public void EnterCommand(PlayerCommands newCommand)
    {
        curFlagPos = Vector3.zero;
        AttackingPattern = AttackingPattern.None;

        if (curCommandInfo is GuardCommand)
        {
            // 1. Такую длинную строку, да ещё и с приведениями типов читать весьма сложно и не понятно, так не стоит делать, самого себя "из будущего" запутаете раньше :)
            // 2. Если ProtectionFleet это List<>, то для метода Remove не важно, есть такой элемент или нет в списке. Так что дополнительная проверка на наличие элемента тут не нужна
            (curCommandInfo as GuardCommand).guardTarget.ProtectionFleet.Remove(MobileOwner);
        }
        if (curCommandInfo is BuildingCommand)
        {
            if ((curCommandInfo as BuildingCommand).ghost != null)
                Destroy((curCommandInfo as BuildingCommand).ghost);
        }

        // Надо выделить команду в enum вместо строк
        if (IsInDock())
        {
            UndockingCommand _uc = new UndockingCommand();
            if (Gunner != null) Gunner.StopFiring();
            PatrolPositionNum = 0;
            _uc.commandAfterUndocking = newCommand;
            _uc.DocingStation = dockingStation;
            _uc.Hub = dockingHub;
            curCommandInfo = _uc;
            Command = PlayerCommand.Undocking;
            return;
        }

        if (newCommand.command == "SettingAbilityTargetCommand")
        {
            curCommandInfo = newCommand;
            Command = PlayerCommand.SettingAbilityTarget;
            return;
        }

        if (Gunner != null) Gunner.StopFiring();
        PatrolPositionNum = 0;
        curCommandInfo = newCommand;

        // Стоит наверно привести `newCommand.command` к PlayerCommand, а не как строка
        switch (newCommand.command)
        {
            case "Move":
                Command = PlayerCommand.Move;
                break;
            case "Attack":
                Command = PlayerCommand.Attack;
                break;
            case "Cover":
                Command = PlayerCommand.Cover;
                break;
            case "Hide":
                Command = PlayerCommand.Hide;
                break;
            case "FullStop":
                Command = PlayerCommand.FullStop;
                break;
            case "Guard":
                Command = PlayerCommand.Guard;
                break;
            case "Patrol":
                Command = PlayerCommand.Patrol;
                break;
            case "Mine":
                Miner.ToBase = (curCommandInfo as MiningCommand).ToBase;
                Command = PlayerCommand.Mine;
                break;
            case "Fixing":
                Command = PlayerCommand.FixIn;
                break;
            case "Deassembling":
                Command = PlayerCommand.Deassamble;
                break;
            case "Build":
                if ((newCommand as BuildingCommand).ghost != null)
                    (newCommand as BuildingCommand).ghost.SetActive(false);
                Command = PlayerCommand.Build;
                break;
        }
    }

    public void PerformCommand()
    {
        Mobile ow = MobileOwner;
        GameManager _gm = GameManager.instance;
        switch (Command)
        {
            case PlayerCommand.Move:
                if (Pilot != null)
                {
                    MoveCommand _mc = curCommandInfo as MoveCommand;
                    if (IsPartOfFleet())
                    {
                        if (ow.TimelyFleet.First() == Owner)
                        {
                            AdmiralControlling(ow.TimelyFleet, _mc);
                        }
                    }
                    else
                    {
                        Pilot.Arrival(_mc.targetVec[0], _mc.Warp);
                        tarVec = _mc.targetVec[0];

                        if ((_mc.targetVec[0] - transform.position).magnitude < Pilot.engines.Threshold && Owner.rigitBody.velocity.magnitude < Pilot.engines.Threshold * Pilot.engines.Acceleration)
                        {
                            Command = PlayerCommand.None;
                        }
                    }
                }
                else
                {
                    Command = PlayerCommand.None;
                }

                break;
            case PlayerCommand.Attack:
                if (Gunner == null) return;
                AttackCommand _ac = curCommandInfo as AttackCommand;
                if (_ac.attackTarget.destroyed)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    AttackingPattern = AttackingPattern.None;
                    return;
                }

                if (Pilot != null)
                {
                    if (IsAdmiralOfFleet())
                    {
                        AdmiralControlling(ow.TimelyFleet, _ac);
                    }

                    if (Pilot.Status != EngineController.State.Disabled)
                    {
                        if (_ac.attackTarget != null)
                        {
                            AttackTarget(_ac.attackTarget, Gunner);
                        }
                    }
                }

                Gunner.MainTarget = _ac.attackTarget;
                tarSelObj = _ac.attackTarget;
                break;
            case PlayerCommand.Cover:
                if (Gunner == null) return;
                HideCoverCommand _cc = curCommandInfo as HideCoverCommand;
                if (_cc.coverignShip.destroyed || _cc.ThreateningEnemyObjects.destroyed)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    AttackingPattern = AttackingPattern.None;
                    return;
                }

                if (IsAdmiralOfFleet())
                {
                    if (_cc.MainCommand != null)
                    {
                        AdmiralControlling(ow.TimelyFleet, _cc.MainCommand);
                    }
                    else
                    {
                        AdmiralControlling(ow.TimelyFleet);
                    }
                }

                if (Pilot != null)
                {
                    if (Pilot.Status != EngineController.State.Disabled)
                    {
                        if (_cc.ThreateningEnemyObjects != null)
                        {
                            if (_cc.coverignShip is Mobile)
                            {
                                if (_cc.coverignShip.captain.Pilot.Status != EngineController.State.Disabled)
                                {
                                    AttackTarget(_cc.ThreateningEnemyObjects, Gunner, _cc.coverignShip.rigitBody.velocity.magnitude);
                                }
                                else
                                {
                                    Vector3 relativePos =
                                        _cc.ThreateningEnemyObjects.transform.position -
                                        _cc.coverignShip.transform.position;
                                    Vector3.Normalize(relativePos);
                                    relativePos =
                                        relativePos * (Owner.ObjectRadius + 5 + _cc.coverignShip.ObjectRadius);

                                    Pilot.Arrival(relativePos);
                                }
                            }
                        }
                    }
                }

                Gunner.MainTarget = _cc.ThreateningEnemyObjects;
                tarSelObj = _cc.ThreateningEnemyObjects;
                break;
            case PlayerCommand.Hide:
                HideCoverCommand _hc = curCommandInfo as HideCoverCommand;
                if (_hc.coverignShip == null || _hc.ThreateningEnemyObjects == null || _hc.coverignShip.destroyed || _hc.ThreateningEnemyObjects.destroyed)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                if (IsAdmiralOfFleet())
                {
                    if (_hc.MainCommand != null)
                    {
                        AdmiralControlling(ow.TimelyFleet, _hc.MainCommand);
                    }
                    else
                    {
                        AdmiralControlling(ow.TimelyFleet);
                    }
                }

                if (Pilot != null)
                {
                    if (Pilot.Status != EngineController.State.Disabled)
                    {
                        if (_hc.ThreateningEnemyObjects != null)
                        {
                            Vector3 relativePos = _hc.ThreateningEnemyObjects.transform.position -
                                                  _hc.coverignShip.transform.position;
                            Vector3.Normalize(relativePos);
                            relativePos = relativePos * (_hc.coverignShip.ObjectRadius + 5 + Owner.ObjectRadius * -1);

                            Pilot.Arrival(relativePos);
                            tarVec = relativePos;
                        }
                    }
                }

                if (Gunner != null) Gunner.OpenFireAtNearestEnemy();
                break;
            case PlayerCommand.FullStop:
                if (Pilot != null && Pilot.Status != EngineController.State.Stopping) Pilot.Stop();
                if (Gunner != null) Gunner.StopFiring();
                break;

            case PlayerCommand.Guard:
                GuardCommand _gc = curCommandInfo as GuardCommand;
                if (_gc.guardTarget.destroyed)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                if (Gunner != null)
                {
                    if (_gc.guardTarget.ThreateningEnemyObjects != null &&
                        _gc.guardTarget.ThreateningEnemyObjects.Any())
                    {
                        if (Gunner != null)
                            Gunner.MainTarget = STMethods.NearestSelObj(_gc.guardTarget,
                                _gc.guardTarget.ThreateningEnemyObjects);
                        tarSelObj = Gunner.MainTarget;
                    }
                }

                if (Pilot != null)
                {
                    if (!_gc.guardTarget.ProtectionFleet.Any(x => x == Owner))
                        _gc.guardTarget.ProtectionFleet.Add(MobileOwner);
                    if (Owner.destroyed)
                        // опять же, проверять Remove не стоит, он просто ничего не сделает если элемента нет
                        _gc.guardTarget.ProtectionFleet.Remove(MobileOwner);

                    for (int i = _gc.guardTarget.ProtectionFleet.Count - 1; i >= 0; i--)
                    {
                        if (_gc.guardTarget.ProtectionFleet[i] == MobileOwner)
                        {
                            tarVec = _gc.fleetPattern[i];
                            float maxRad = STMethods.MaxRadiusInFleet(_gc.guardTarget.ProtectionFleet) *
                                           (2 + (_gc.guardTarget.ProtectionFleet.Count / 16));
                            Vector3 point = _gc.guardTarget.transform.position +
                                            (_gc.guardTarget.transform.rotation *
                                             new Vector3(_gc.fleetPattern[i].x * maxRad,
                                                 _gc.fleetPattern[i].y * maxRad,
                                                 _gc.fleetPattern[i].z * maxRad));
                            Pilot.Arrival(point, false, true, true, _gc.guardTarget.ProtectionFleet);
                        }
                    }
                }

                break;
            case PlayerCommand.Patrol:
                if (Pilot == null)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                PatrolCommand _pc = curCommandInfo as PatrolCommand;
                if (_pc.targetVec.Any())
                {
                    if (Vector3.Distance(_pc.targetVec[PatrolPositionNum], transform.position) >
                        Pilot.engines.DistanceToFullStop(Pilot.engines.Acceleration) + Pilot.engines.Threshold)
                    {
                        Pilot.Arrival(_pc.targetVec[PatrolPositionNum]);
                    }
                    else
                    {
                        if (PatrolPositionNum < _pc.targetVec.Count - 1)
                        {
                            PatrolPositionNum++;
                        }
                        else
                        {
                            PatrolPositionNum = 0;
                        }
                    }
                }

                if (Gunner != null) Gunner.OpenFireAtNearestEnemy();
                break;
            case PlayerCommand.Mine:
                if (Pilot == null)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                MiningCommand _mic = curCommandInfo as MiningCommand;

                if (_mic.ResTar != null)
                {
                    Miner.curResourcesType = _mic.Type;
                }
                if (Miner.ToBase)
                {
                    if (_mic.UnloadPoint != null)
                    {
                        if (_mic.UnloadPoint.destroyed) _mic.UnloadPoint = null;
                        MinerDocking(_mic.UnloadPoint.GetComponent<ResourceUnloadingController>());
                    }
                    else
                    {
                        List<SelectableObject> UnloadPointsList = new List<SelectableObject>();
                        for (int i = 0; i < _gm.SelectableObjects.Count; i++)
                        {
                            if (_gm.SelectableObjects[i].GetComponent<ResourceUnloadingController>() &&
                                _gm.SelectableObjects[i].PlayerNum == Owner.PlayerNum)
                            {
                                UnloadPointsList.Add(_gm.SelectableObjects[i]);
                            }
                        }

                        if (UnloadPointsList.Count == 0) return;
                        SelectableObject Nearest = STMethods.NearestSelObj(Owner, UnloadPointsList);

                        MinerDocking(Nearest.GetComponent<ResourceUnloadingController>());
                    }
                }
                else
                {
                    if (Miner.curResources <= Miner.MaxResources)
                    {
                        if (_mic.ResTar != null)
                        {
                            if (_mic.Type == STMethods.ResourcesType.Dilithium)
                            {
                                if (_mic.ResTar.curDilithium > 0)
                                {
                                    MinerMining(_mic.ResTar);
                                }
                                else
                                {
                                    _mic.ResTar = null;
                                }
                            }
                            if (_mic.Type == STMethods.ResourcesType.Titanium)
                            {
                                if (_mic.ResTar.curTitanium > 0)
                                {
                                    MinerMining(_mic.ResTar);
                                }
                                else
                                {
                                    _mic.ResTar = null;
                                }
                            }
                        }
                        else
                        {
                            List<SelectableObject> ResourceSourceList = new List<SelectableObject>();
                            for (int i = 0; i < _gm.SelectableObjects.Count; i++)
                            {
                                if (_mic.Type == STMethods.ResourcesType.Dilithium)
                                {
                                    if (_gm.SelectableObjects[i].GetComponent<ResourceSource>() && _gm.SelectableObjects[i].GetComponent<ResourceSource>().curDilithium > 0)
                                    {
                                        ResourceSourceList.Add(_gm.SelectableObjects[i]);
                                    }
                                }
                                if (_mic.Type == STMethods.ResourcesType.Titanium)
                                {
                                    if (_gm.SelectableObjects[i].GetComponent<ResourceSource>() && _gm.SelectableObjects[i].GetComponent<ResourceSource>().curTitanium > 0)
                                    {
                                        ResourceSourceList.Add(_gm.SelectableObjects[i]);
                                    }
                                }
                            }

                            if (ResourceSourceList.Count == 0) return;
                            SelectableObject Nearest = STMethods.NearestSelObj(Owner, ResourceSourceList);

                            MinerMining(Nearest.GetComponent<ResourceSource>());
                        }
                    }
                    else
                    {
                        Miner.curResources = Miner.MaxResources;
                        Miner.ToBase = true;
                    }
                }

                break;
            case PlayerCommand.Undocking:
                UndockingCommand _uc = curCommandInfo as UndockingCommand;

                if (_uc.DocingStation.destroyed)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                Vector3 stayPoint = _uc.DocingStation.transform.position +
                                    (_uc.DocingStation.transform.rotation * _uc.Hub.StayPoint);
                Vector3 exitPoint = _uc.DocingStation.transform.position +
                                    (_uc.DocingStation.transform.rotation * _uc.Hub.ExitPoint);
                if (ToStayPoint)
                {
                    if (Vector3.Distance(transform.position, stayPoint) > Pilot.engines.Threshold)
                    {
                        Vector3 targetDir = stayPoint - transform.position;
                        float angle = Vector3.Angle(targetDir, transform.forward);

                        if (angle < 2)
                        {
                            Pilot.Arrival(stayPoint, false, false);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        ToExitPoint = true;
                        ToStayPoint = false;
                    }
                }

                if (ToExitPoint)
                {
                    if (Vector3.Distance(transform.position, exitPoint) > Pilot.engines.Threshold)
                    {
                        Vector3 targetDir = exitPoint - transform.position;
                        float angle = Vector3.Angle(targetDir, transform.forward);

                        if (angle < 2)
                        {
                            Pilot.Arrival(exitPoint, false, false);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        _uc.Hub.EnteringShip = null;
                        ToExitPoint = false;
                    }
                }

                if (!ToStayPoint && !ToExitPoint)
                {
                    if (_uc.commandAfterUndocking != null)
                    {
                        EnterCommand(_uc.commandAfterUndocking);
                    }
                    else
                    {
                        if (_uc.AwaitingPoint != Vector3.zero)
                        {
                            if (curFlagPos == Vector3.zero) curFlagPos = _uc.AwaitingPoint;
                            int mask = 1 << 9;
                            Collider[] scanColls = Physics.OverlapSphere(curFlagPos, Owner.ObjectRadius * 1.5f, mask);
                            if (scanColls.Length == 1 && scanColls[0].transform.root != transform)
                            {
                                curFlagPos += new Vector3(
                                    UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f),
                                    UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f),
                                    UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f));
                                return;
                            }

                            if (scanColls.Length > 1)
                            {
                                curFlagPos += new Vector3(
                                    UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f),
                                    UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f),
                                    UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f));
                                return;
                            }

                            MoveCommand _mc = new MoveCommand();
                            _mc.command = "Move";
                            _mc.targetVec = new List<Vector3>();
                            _mc.targetVec.Add(curFlagPos);
                            EnterCommand(_mc);
                            curFlagPos = Vector3.zero;
                        }
                        else
                        {
                            Command = PlayerCommand.None;
                            curCommandInfo = null;
                        }
                    }
                }
                break;
            case PlayerCommand.FixIn:
                if (Pilot == null)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                FixingCommand _fc = curCommandInfo as FixingCommand;

                if (_fc.FixingPoint != null)
                {
                    if (_fc.FixingPoint.destroyed) _fc.FixingPoint = null;
                    FixingDocking(_fc.FixingPoint.GetComponent<FixingPointController>());
                }
                else
                {
                    List<SelectableObject> FixingPointsList = new List<SelectableObject>();
                    for (int i = 0; i < _gm.SelectableObjects.Count; i++)
                    {
                        if (_gm.SelectableObjects[i].GetComponent<FixingPointController>() &&
                            _gm.SelectableObjects[i].PlayerNum == Owner.PlayerNum)
                        {
                            FixingPointsList.Add(_gm.SelectableObjects[i]);
                        }
                    }

                    if (FixingPointsList.Count == 0) return;
                    SelectableObject Nearest = STMethods.NearestSelObj(Owner, FixingPointsList);

                    FixingDocking(Nearest.GetComponent<FixingPointController>());
                }

                break;
            case PlayerCommand.Deassamble:
                if (Pilot == null)
                {
                    StaticDeassembling();
                    return;
                }

                PlayerCommands _dec = curCommandInfo;

                List<SelectableObject> DeassemblingPointsList = new List<SelectableObject>();
                for (int i = 0; i < _gm.SelectableObjects.Count; i++)
                {
                    if (_gm.SelectableObjects[i].GetComponent<DeassemblingPointController>() &&
                        _gm.SelectableObjects[i].PlayerNum == Owner.PlayerNum)
                    {
                        DeassemblingPointsList.Add(_gm.SelectableObjects[i]);
                    }
                }

                if (DeassemblingPointsList.Count == 0) return;
                SelectableObject NearestDe = STMethods.NearestSelObj(Owner, DeassemblingPointsList);

                DeassemblingDocking(NearestDe.GetComponent<DeassemblingPointController>());

                break;
            case PlayerCommand.Build:
                BuildingCommand _buildc = curCommandInfo as BuildingCommand;
                if (Pilot == null || _buildc.target == null)
                {
                    if (_buildc.ghost != null)
                    {
                        Destroy(_buildc.ghost);
                        _buildc.ghost = null;
                    }
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                if (_buildc.proTarget != null)
                {
                    if (Vector3.Distance(gameObject.transform.position, _buildc.proTarget.transform.position) > _buildc.target.ObjectRadius + Owner.ObjectRadius + 5)
                    {
                        Pilot.Arrival(_buildc.proTarget.transform.position, false, true, false);
                    }
                    else
                    {
                        if (Pilot.Status != EngineController.State.Stopping) Pilot.Stop();
                        ObjectUnderConstruction _script = _buildc.proTarget.GetComponent<ObjectUnderConstruction>();
                        if (!_script.Constucters.Any(x => x == MobileOwner))
                        {
                            _script.Constucters.Add(MobileOwner);
                        }

                        if (_script._hs.curHull <= 0)
                        {
                            Command = PlayerCommand.None;
                            curCommandInfo = null;
                        }
                    }
                }
                else
                {
                    if (Vector3.Distance(gameObject.transform.position, _buildc.posVec) > _buildc.target.ObjectRadius + Owner.ObjectRadius + 5)
                    {
                        Pilot.Arrival(_buildc.posVec);
                    }
                    else
                    {
                        if (Pilot.Status != EngineController.State.Stopping) Pilot.Stop();
                        int mask = 1 << 9;
                        Collider[] colls = Physics.OverlapSphere(_buildc.posVec, _buildc.target.ObjectRadius, mask);
                        if (colls.Length <= 0)
                        {
                            if (_buildc.target.CanBeBuild(Owner.PlayerNum))
                            {
                                if (_buildc.ghost != null)
                                {
                                    Destroy(_buildc.ghost);
                                    _buildc.ghost = null;
                                }
                                _buildc.target.RemoveRes(Owner.PlayerNum);
                                GameObject proTarget = new GameObject();
                                proTarget.transform.position = _buildc.posVec;
                                proTarget.transform.rotation = Quaternion.Euler(_buildc.rotVec);
                                proTarget.name = _buildc.target.ObjectUnderConstruction;
                                SelectableObject ns = STMethods.addObjectClass(_buildc.target.ObjectUnderConstruction, proTarget);
                                ns.PlayerNum = Owner.PlayerNum;
                                (ns as ObjectUnderConstruction).Contract = _buildc.target;
                                (ns as ObjectUnderConstruction).BuilderEfficiency = _buildc.target.BuilderEfficiency;

                                _buildc.proTarget = proTarget;
                            }
                            else
                            {
                                if (_buildc.ghost != null)
                                {
                                    Destroy(_buildc.ghost);
                                    _buildc.ghost = null;
                                }
                                Command = PlayerCommand.None;
                                curCommandInfo = null;
                                return;
                            }
                        }
                        else
                        {
                            if (_buildc.ghost != null)
                            {
                                Destroy(_buildc.ghost);
                                _buildc.ghost = null;
                            }
                            Command = PlayerCommand.None;
                            curCommandInfo = null;
                            return;
                        }
                    }
                }
                break;
            case PlayerCommand.SettingAbilityTarget:
                SettingAbilityTargetCommand _satc = curCommandInfo as SettingAbilityTargetCommand;
                _satc.ability.gameTarget = _satc.target;
                _satc.ability.OnApplyTarget();
                Command = PlayerCommand.None;
                curCommandInfo = null;
                break;
        }
    }

    public void MinerDocking(ResourceUnloadingController _tarUC)
    {
        GameManager _gm = GameManager.instance;

        _tarUC.DockingCall(MobileOwner);

        for (int i = 0; i < _tarUC.HubCount; i++)
        {
            if (_tarUC.HubSS.Hubs[i].EnteringShip == Owner)
            {
                Vector3 enterPoint = _tarUC.transform.position +
                                     (_tarUC.transform.rotation * _tarUC.HubSS.Hubs[i].EnterPoint);
                Vector3 stayPoint = _tarUC.transform.position +
                                    (_tarUC.transform.rotation * _tarUC.HubSS.Hubs[i].StayPoint);
                Vector3 exitPoint = _tarUC.transform.position +
                                    (_tarUC.transform.rotation * _tarUC.HubSS.Hubs[i].ExitPoint);
                if (!ToEnterPoint && !ToStayPoint && !ToExitPoint)
                {
                    Pilot.Arrival(enterPoint, true);
                    ToEnterPoint = true;
                }

                if (ToEnterPoint)
                {
                    if (Vector3.Distance(transform.position, enterPoint) > Pilot.engines.Threshold)
                    {
                        Pilot.Arrival(enterPoint);
                    }
                    else
                    {
                        ToStayPoint = true;
                        ToEnterPoint = false;
                    }
                }

                if (ToStayPoint)
                {
                    if (dockingStation != _tarUC.HubSS.Owner) dockingStation = _tarUC.HubSS.Owner;
                    if (dockingHub != _tarUC.HubSS.Hubs[i]) dockingHub = _tarUC.HubSS.Hubs[i];

                    if (Vector3.Distance(transform.position, stayPoint) > Pilot.engines.Threshold)
                    {
                        Vector3 targetDir = stayPoint - transform.position;
                        float angle = Vector3.Angle(targetDir, transform.forward);

                        if (angle < 2)
                        {
                            Pilot.Arrival(stayPoint, false, false);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        if (Miner.curResources > 0)
                        {
                            switch (Miner.curResourcesType)
                            {
                                case STMethods.ResourcesType.Titanium:
                                    _gm.Players[Owner.PlayerNum - 1].Titanium += Time.deltaTime * 10;
                                    break;
                                case STMethods.ResourcesType.Dilithium:
                                    _gm.Players[Owner.PlayerNum - 1].Dilithium += Time.deltaTime * 10;
                                    break;
                                case STMethods.ResourcesType.Biomatter:
                                    _gm.Players[Owner.PlayerNum - 1].Biomatter += Time.deltaTime * 10;
                                    break;
                            }

                            Miner.curResources -= Time.deltaTime * 10;
                            transform.position = Vector3.Lerp(transform.position, stayPoint, Time.deltaTime);
                        }
                        else
                        {
                            Miner.curResources = 0;

                            ToExitPoint = true;
                            ToStayPoint = false;
                        }
                    }
                }

                if (ToExitPoint)
                {
                    if (dockingStation != _tarUC.HubSS.Owner) dockingStation = _tarUC.HubSS.Owner;
                    if (dockingHub != _tarUC.HubSS.Hubs[i]) dockingHub = _tarUC.HubSS.Hubs[i];

                    if (Vector3.Distance(transform.position, exitPoint) > Pilot.engines.Threshold)
                    {
                        Vector3 targetDir = exitPoint - transform.position;
                        float angle = Vector3.Angle(targetDir, transform.forward);

                        if (angle < 2)
                        {
                            Pilot.Arrival(exitPoint, false, false);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        ToExitPoint = false;
                        Miner.ToBase = false;

                        dockingStation = null;
                        dockingHub = null;
                    }
                }
                break;
            }
        }
        if (_tarUC.awaitingPoint.AwaitingShipsOnHub.Any(x => x == Owner))
        {
            for (int y = 0; y < _tarUC.awaitingPoint.AwaitingShipsOnHub.Count; y++)
            {
                if (_tarUC.awaitingPoint.AwaitingShipsOnHub[y] == Owner)
                {
                    Vector3 awaitingPoint = _tarUC.awaitingPoint.WaitingPoints()[y];
                    if (_tarUC.HubSS.Hubs.Length == 1)
                    {
                        awaitingPoint = _tarUC.awaitingPoint.WaitingPoints(_tarUC.HubSS.Hubs[0].EnteringShip)[y];
                    }
                    else
                    {
                        awaitingPoint = _tarUC.awaitingPoint.WaitingPoints()[y];
                    }

                    bool StayInRow = Vector3.Angle(awaitingPoint - transform.position, transform.forward) < 15 &&
                                     Vector3.Angle(
                                         _tarUC.transform.position +
                                         (_tarUC.transform.rotation * _tarUC.awaitingPoint.AwaitingPoint) -
                                         transform.position, transform.forward) < 15 &&
                                     Vector3.Distance(transform.position, awaitingPoint) < Owner.ObjectRadius * 7.5f;

                    if (Vector3.Distance(transform.position, awaitingPoint) > Pilot.engines.Threshold)
                    {
                        if (StayInRow)
                        {
                            Pilot.Arrival(awaitingPoint, false, false, false, _tarUC.awaitingPoint.AwaitingShipsOnHub);
                        }
                        else
                        {
                            Pilot.Arrival(awaitingPoint);
                        }
                    }
                    {
                        Vector3 targetDir = _tarUC.awaitingPoint.AwaitingPoint - transform.position;
                        Pilot.engines.RotateShip(targetDir);
                    }
                }
            }
        }
    }

    public void MinerMining(ResourceSource ResTar)
    {
        float radius = Owner.ObjectRadius + ResTar.ObjectRadius + Owner.WeaponRange * 0.75f;
        if (Vector3.Distance(ResTar.transform.position, transform.position) > Owner.ObjectRadius * 5)
        {
            Pilot.Arrival(ResTar.transform.position, true, true, false);
        }
        else
        {
            Pilot.Orbiting(ResTar, radius);
        }

        if (Vector3.Distance(transform.position, ResTar.transform.position) <=
            (Owner.ObjectRadius + ResTar.ObjectRadius + Owner.WeaponRange))
        {
            Miner.Mine(ResTar);
        }
    }

    public void FixingDocking(FixingPointController _tarFC)
    {
        _tarFC.DockingCall(MobileOwner);
        for (int i = 0; i < _tarFC.HubSS.Hubs.Length; i++)
        {
            if (_tarFC.HubSS.Hubs[i].EnteringShip == Owner)
            {
                Vector3 enterPoint = _tarFC.transform.position +
                                     (_tarFC.transform.rotation * _tarFC.HubSS.Hubs[i].EnterPoint);
                Vector3 stayPoint = _tarFC.transform.position +
                                    (_tarFC.transform.rotation * _tarFC.HubSS.Hubs[i].StayPoint);
                Vector3 exitPoint = _tarFC.transform.position +
                                    (_tarFC.transform.rotation * _tarFC.HubSS.Hubs[i].ExitPoint);
                if (!ToEnterPoint && !ToStayPoint && !ToExitPoint)
                {
                    Pilot.Arrival(enterPoint, true);
                    ToEnterPoint = true;
                }

                if (ToEnterPoint)
                {
                    if (Vector3.Distance(transform.position, enterPoint) > Pilot.engines.Threshold)
                    {
                        Pilot.Arrival(enterPoint);
                        ToEnterPoint = true;
                    }
                    else
                    {
                        ToStayPoint = true;
                        ToEnterPoint = false;
                    }
                }

                if (ToStayPoint)
                {
                    if (dockingStation != _tarFC.HubSS.Owner) dockingStation = _tarFC.HubSS.Owner;
                    if (dockingHub != _tarFC.HubSS.Hubs[i]) dockingHub = _tarFC.HubSS.Hubs[i];

                    if (Vector3.Distance(transform.position, stayPoint) > Pilot.engines.Threshold)
                    {
                        Vector3 targetDir = stayPoint - transform.position;
                        float angle = Vector3.Angle(targetDir, transform.forward);

                        if (angle < 2)
                        {
                            Pilot.Arrival(stayPoint, false, false);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        if (Owner._hs.NeedFix())
                        {
                            Owner._hs.Fixing();
                            transform.position = Vector3.Lerp(transform.position, stayPoint, Time.deltaTime);
                        }
                        else
                        {
                            ToExitPoint = true;
                            ToStayPoint = false;
                        }
                    }
                }

                if (ToExitPoint)
                {
                    if (dockingStation != _tarFC.HubSS.Owner) dockingStation = _tarFC.HubSS.Owner;
                    if (dockingHub != _tarFC.HubSS.Hubs[i]) dockingHub = _tarFC.HubSS.Hubs[i];

                    if (Vector3.Distance(transform.position, exitPoint) > Pilot.engines.Threshold)
                    {
                        Vector3 targetDir = exitPoint - transform.position;
                        float angle = Vector3.Angle(targetDir, transform.forward);

                        if (angle < 2)
                        {
                            Pilot.Arrival(exitPoint, false, false, false);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        if (dockingStation.GetComponent<FixingPointController>())
                        {
                            MoveCommand _nmc = new MoveCommand();
                            _nmc.command = "Move";
                            _nmc.targetVec = new List<Vector3>();

                            if (curFlagPos == Vector3.zero) curFlagPos = dockingStation.GetComponent<FixingPointController>().ExitFlag.ExitFlag;
                            int mask = 1 << 9;
                            Collider[] scanColls = Physics.OverlapSphere(curFlagPos, Owner.ObjectRadius * 1.5f, mask);
                            if (scanColls.Length == 1 && scanColls[0].transform.root != transform)
                            {
                                curFlagPos += new Vector3(UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f), UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f), UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f));
                                return;
                            }
                            if (scanColls.Length > 1)
                            {
                                curFlagPos += new Vector3(UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f), UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f), UnityEngine.Random.Range(-Owner.ObjectRadius * 1.5f, Owner.ObjectRadius * 1.5f));
                                return;
                            }

                            _nmc.targetVec.Add(curFlagPos);
                            _nmc.Warp = false;

                            ToExitPoint = false;
                            dockingStation = null;
                            dockingHub = null;

                            EnterCommand(_nmc);
                        }
                        else
                        {
                            ToExitPoint = false;
                            dockingStation = null;
                            dockingHub = null;
                        }
                    }
                }
                return;
            }
        }
        if (_tarFC.awaitingPoint.AwaitingShipsOnHub.Any(x => x == Owner))
        {
            for (int y = 0; y < _tarFC.awaitingPoint.AwaitingShipsOnHub.Count; y++)
            {
                if (_tarFC.awaitingPoint.AwaitingShipsOnHub[y] == Owner)
                {
                    Vector3 awaitingPoint;
                    if (_tarFC.HubSS.Hubs.Length == 1)
                    {
                        awaitingPoint = _tarFC.awaitingPoint.WaitingPoints(_tarFC.HubSS.Hubs[0].EnteringShip)[y];
                    }
                    else
                    {
                        awaitingPoint = _tarFC.awaitingPoint.WaitingPoints()[y];
                    }

                    bool StayInRow = Vector3.Angle(awaitingPoint - transform.position, transform.forward) < 15 &&
                                     Vector3.Angle(
                                         _tarFC.transform.position +
                                         (_tarFC.transform.rotation * _tarFC.awaitingPoint.AwaitingPoint) -
                                         transform.position, transform.forward) < 15 &&
                                     Vector3.Distance(transform.position, awaitingPoint) < Owner.ObjectRadius * 7.5f;

                    if (Vector3.Distance(transform.position, awaitingPoint) > Pilot.engines.Threshold)
                    {
                        if (StayInRow)
                        {
                            Pilot.Arrival(awaitingPoint, false, false, false, _tarFC.awaitingPoint.AwaitingShipsOnHub);
                        }
                        else
                        {
                            Pilot.Arrival(awaitingPoint);
                        }
                    }
                    else
                    {
                        Vector3 targetDir = _tarFC.awaitingPoint.transform.position + (_tarFC.awaitingPoint.transform.rotation * _tarFC.awaitingPoint.AwaitingPoint) - transform.position;
                        Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir), true, false);
                    }
                }
            }
        }
    }

    public void DeassemblingDocking(DeassemblingPointController _tarDC)
    {
        GameManager _gm = GameManager.instance;

        _tarDC.DockingCall(MobileOwner);


        for (int i = 0; i < _tarDC.HubCount; i++)
        {
            if (_tarDC.HubSS.Hubs[i].EnteringShip == Owner)
            {
                Vector3 enterPoint = _tarDC.transform.position +
                                     (_tarDC.transform.rotation * _tarDC.HubSS.Hubs[i].EnterPoint);
                Vector3 stayPoint = _tarDC.transform.position +
                                    (_tarDC.transform.rotation * _tarDC.HubSS.Hubs[i].StayPoint);

                if (!ToEnterPoint && !ToStayPoint && !ToExitPoint)
                {
                    Pilot.Arrival(enterPoint, true);
                    ToEnterPoint = true;
                }

                if (ToEnterPoint)
                {
                    if (Vector3.Distance(transform.position, enterPoint) > Pilot.engines.Threshold)
                    {
                        Pilot.Arrival(enterPoint);
                        ToEnterPoint = true;
                    }
                    else
                    {
                        ToStayPoint = true;
                        ToEnterPoint = false;
                    }
                }

                if (ToStayPoint)
                {
                    if (dockingStation != _tarDC.HubSS.Owner) dockingStation = _tarDC.HubSS.Owner;
                    if (dockingHub != _tarDC.HubSS.Hubs[i]) dockingHub = _tarDC.HubSS.Hubs[i];

                    Owner.selectionLock = true;
                    if (DeassebleTime == -1)
                    {
                        if (Owner.healthSystem)
                        {
                            DeassebleTime = Owner.DeassebleTime * Owner._hs.curHull / Owner._hs.MaxHull;
                        }
                        else
                        {
                            DeassebleTime = Owner.DeassebleTime;
                        }
                    }

                    if (_tarDC.HubSS.Owner.destroyed)
                    {
                        if (Owner.healthSystem)
                        {
                            Owner._hs.curHull = 0;
                        }
                        else
                        {
                            Destroy(gameObject);
                        }
                    }

                    if (Vector3.Distance(transform.position, stayPoint) > Pilot.engines.Threshold)
                    {
                        Vector3 targetDir = stayPoint - transform.position;
                        float angle = Vector3.Angle(targetDir, transform.forward);

                        if (angle < 2)
                        {
                            Pilot.Arrival(stayPoint, false, false);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, stayPoint, Time.deltaTime);
                        MeshRenderer[] _m = GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer _mr in _m)
                        {
                            _mr.gameObject.SetActive(false);
                        }

                        if (!BeginDeassebling)
                        {
                            deassambleAnim = Instantiate(Owner.DeassembledAnim, transform.position, transform.rotation);
                            BuildAnimationScript[] animSc =
                                deassambleAnim.GetComponentsInChildren<BuildAnimationScript>();
                            foreach (BuildAnimationScript sc in animSc)
                            {
                                sc.BuildTime = DeassebleTime;
                                sc.Revert = true;
                            }

                            BeginDeassebling = true;
                        }
                        else
                        {
                            if (DeassebleTime > 0)
                            {
                                DeassebleTime -= Time.deltaTime;
                            }
                            else
                            {
                                dockingStation = null;
                                dockingHub = null;
                                Destroy(deassambleAnim);
                                if (Owner.healthSystem)
                                {
                                    _gm.Players[Owner.PlayerNum - 1].Titanium +=
                                        Owner.TitaniumCost * Owner._hs.curHull / Owner._hs.MaxHull;
                                    _gm.Players[Owner.PlayerNum - 1].Dilithium +=
                                        Owner.DilithiumCost * Owner._hs.curHull / Owner._hs.MaxHull;
                                    _gm.Players[Owner.PlayerNum - 1].Biomatter +=
                                        Owner.BiomatterCost * Owner._hs.curHull / Owner._hs.MaxHull;
                                    _gm.Players[Owner.PlayerNum - 1].Crew += Owner._hs.curCrew;
                                }
                                else
                                {
                                    _gm.Players[Owner.PlayerNum - 1].Titanium += Owner.TitaniumCost;
                                    _gm.Players[Owner.PlayerNum - 1].Dilithium += Owner.DilithiumCost;
                                    _gm.Players[Owner.PlayerNum - 1].Biomatter += Owner.BiomatterCost;
                                }

                                Destroy(gameObject);
                            }
                        }
                    }
                }
                return;
            }
        }
        if (_tarDC.awaitingPoint.AwaitingShipsOnHub.Any(x => x == Owner))
        {
            for (int y = 0; y < _tarDC.awaitingPoint.AwaitingShipsOnHub.Count; y++)
            {
                if (_tarDC.awaitingPoint.AwaitingShipsOnHub[y] == Owner)
                {
                    Vector3 awaitingPoint = _tarDC.awaitingPoint.WaitingPoints()[y];

                    if (_tarDC.HubSS.Hubs.Length == 1)
                    {
                        awaitingPoint = _tarDC.awaitingPoint.WaitingPoints(_tarDC.HubSS.Hubs[0].EnteringShip)[y];
                    }
                    else
                    {
                        awaitingPoint = _tarDC.awaitingPoint.WaitingPoints()[y];
                    }

                    bool StayInRow = Vector3.Angle(awaitingPoint - transform.position, transform.forward) < 15 &&
                                     Vector3.Angle(
                                         _tarDC.transform.position +
                                         (_tarDC.transform.rotation * _tarDC.awaitingPoint.AwaitingPoint) -
                                         transform.position, transform.forward) < 15 &&
                                     Vector3.Distance(transform.position, awaitingPoint) < Owner.ObjectRadius * 7.5f;

                    if (Vector3.Distance(transform.position, awaitingPoint) > Pilot.engines.Threshold)
                    {
                        if (StayInRow)
                        {
                            
                            Pilot.Arrival(awaitingPoint, false, false, false, _tarDC.awaitingPoint.AwaitingShipsOnHub);
                        }
                        else
                        {
                            Pilot.Arrival(awaitingPoint);
                        }
                    }
                    else
                    {
                        Vector3 targetDir = _tarDC.awaitingPoint.AwaitingPoint - transform.position;
                        Pilot.engines.RotateShip(targetDir);
                    }
                }
            }
        }
    }

    public void StaticDeassembling()
    {
        GameManager _gm = GameManager.instance;
        Owner.selectionLock = true;
        if (DeassebleTime == -1)
        {
            if (Owner.healthSystem)
            {
                DeassebleTime = Owner.DeassebleTime * Owner._hs.curHull / Owner._hs.MaxHull;
            }
            else
            {
                DeassebleTime = Owner.DeassebleTime;
            }
        }

        MeshRenderer[] _m = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer _mr in _m)
        {
            _mr.gameObject.SetActive(false);
        }

        if (!BeginDeassebling)
        {
            deassambleAnim = Instantiate(Owner.DeassembledAnim, transform.position, transform.rotation);
            BuildAnimationScript[] animSc =
                deassambleAnim.GetComponentsInChildren<BuildAnimationScript>();
            foreach (BuildAnimationScript sc in animSc)
            {
                sc.BuildTime = DeassebleTime;
                sc.Revert = true;
            }

            BeginDeassebling = true;
        }
        else
        {
            if (DeassebleTime > 0)
            {
                DeassebleTime -= Time.deltaTime;
            }
            else
            {
                dockingStation = null;
                dockingHub = null;
                Destroy(deassambleAnim);
                if (Owner.healthSystem)
                {
                    _gm.Players[Owner.PlayerNum - 1].Titanium +=
                        Owner.TitaniumCost * Owner._hs.curHull / Owner._hs.MaxHull;
                    _gm.Players[Owner.PlayerNum - 1].Dilithium +=
                        Owner.DilithiumCost * Owner._hs.curHull / Owner._hs.MaxHull;
                    _gm.Players[Owner.PlayerNum - 1].Biomatter +=
                        Owner.BiomatterCost * Owner._hs.curHull / Owner._hs.MaxHull;
                    _gm.Players[Owner.PlayerNum - 1].Crew += Owner._hs.curCrew;
                }
                else
                {
                    _gm.Players[Owner.PlayerNum - 1].Titanium += Owner.TitaniumCost;
                    _gm.Players[Owner.PlayerNum - 1].Dilithium += Owner.DilithiumCost;
                    _gm.Players[Owner.PlayerNum - 1].Biomatter += Owner.BiomatterCost;
                }

                Destroy(gameObject);
            }
        }
    }

    public void OnDestroy()
    {
        if (curCommandInfo is GuardCommand)
        {
            // и снова, проверка не нужна
            (curCommandInfo as GuardCommand).guardTarget.ProtectionFleet.Remove(MobileOwner);
        }
        if (curCommandInfo is BuildingCommand)
        {
            if ((curCommandInfo as BuildingCommand).ghost != null) Destroy((curCommandInfo as BuildingCommand).ghost);
        }
    }

    // Является ли Owner Mobile'ом - является ли движущимся кораблём
    public bool OwnerIsMobile()
    {
        return MobileOwner != null;
    }

    // Является ли текущий корабль адмиралом флота
    public bool IsAdmiralOfFleet()
    {
        return IsPartOfFleet() && MobileOwner.TimelyFleet.First() == MobileOwner;
    }

    // Является ли текущий корабль частью флота
    public bool IsPartOfFleet()
    {
        if (MobileOwner == null)
            return false;
        return MobileOwner.TimelyFleet != null && MobileOwner.TimelyFleet.Any();
    }

    // Приказ пилоту атаковать цель
    public void AttackTarget(SelectableObject target, GunnerController gunner, float speed = 0f)
    {
        if (AttackingPattern == AttackingPattern.None)
        {
            float random = UnityEngine.Random.Range(0f, 1f);
            this.AttackingPattern = ChangeAttackPattern(random);
        }

        Pilot.AttaсkWithPattern(this.AttackingPattern, target, gunner, speed);
    }

    public bool IsInDock()
    {
        return ToStayPoint || ToExitPoint;
    }
}