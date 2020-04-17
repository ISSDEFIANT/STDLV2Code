using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Controllers;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        Warp,
        Patrol,
        SeekAndDestroy,
        Mine,
        Trade,
        FixIn,
        DeassambleIn,
        Build,
        Undocking
    }

    public PlayerCommands curCommandInfo;

    public SelectableObject tarSelObj;
    public Vector3 tarVec;

    /// <summary> Команда, указанная игроком. </summary>
    public PlayerCommand Command = PlayerCommand.None;

    public SelectableObject Owner;
    public SensorSS Sensors;

    public GunnerController Gunner;
    public EngineController Pilot;
    public MiningController Miner;

    private float fleetStabTimer = 3;
    private int PatrolPositionNum = 0;

    public bool ToEnterPoint;
    public bool ToStayPoint;
    public bool ToExitPoint;
    public SelectableObject dockingStation;
    public DockingHub dockingHub;

    // Start is called before the first frame update
    void Start()
    {
        Owner.captain = this;

        if (gameObject.GetComponent<GunnerController>())
        {
            Gunner = gameObject.GetComponent<GunnerController>();
        }

        if (gameObject.GetComponent<EngineController>())
        {
            Pilot = gameObject.GetComponent<EngineController>();
        }
        
        if (gameObject.GetComponent<MiningController>())
        {
            Miner = gameObject.GetComponent<MiningController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Command == PlayerCommand.None)
        {
            if (Owner.Alerts == STMethods.Alerts.GreenAlert)
            {
                if (Sensors.EnemysInSensorRange().Count > 0)
                {

                }
            }
            else if (Owner.Alerts == STMethods.Alerts.YellowAlert)
            {
                if (Sensors.EnemysInSensorRange().Count > 0)
                {

                }
            }
            else if (Owner.Alerts == STMethods.Alerts.RedAlert)
            {
                if (Gunner != null)
                {
                    if (Sensors.EnemysInSensorRange().Count > 0)
                    {
                        if (Pilot != null)
                        {
                            Mobile ow = Owner as Mobile;
                            if (ow.TimelyFleet != null && ow.TimelyFleet.Count > 0 && ow.TimelyFleet[0] == Owner)
                            {
                                AdmiralControlling(ow.TimelyFleet);
                            }
                            else
                            {
                                if (Pilot.Status != EngineController.State.Disabled)
                                {
                                    if (Gunner.GetNearestTarget() != null)
                                    {
                                        if (Pilot.Status != EngineController.State.AttackingAlpha ||
                                            Pilot.Status != EngineController.State.AttackingBeta ||
                                            Pilot.Status != EngineController.State.AttackingGamma)
                                        {
                                            float random = UnityEngine.Random.Range(0f, 1f);
                                            switch (ChangeAttackPattern(random))
                                            {
                                                case "Alpha":
                                                    Pilot.AttaсkAlpha(Gunner.GetNearestTarget(), Gunner);
                                                    break;
                                                case "Beta":
                                                    Pilot.AttackBeta(Gunner.GetNearestTarget(), Gunner);
                                                    break;
                                                case "Gamma":
                                                    Pilot.AttackGamma(Gunner.GetNearestTarget(), Gunner);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        Gunner.OpenFireAtNearestEnemy();
                    }
                }
            }

            if (Owner is Mobile)
            {                
                Mobile ow = Owner as Mobile;
                if (ow.TimelyFleet != null && ow.TimelyFleet.Count > 0)
                {
                    if (!Pilot.engines.Moving)
                    {
                        float angleToTarget = Vector3.Angle(ow.TimelyFleet[0].transform.forward, transform.forward);
                        if (angleToTarget > 2)
                        {
                            if (fleetStabTimer > 0)
                            {
                                fleetStabTimer -= Time.deltaTime;
                            }
                            else
                            {
                                Pilot.engines.RotateShip(ow.TimelyFleet[0].transform.rotation);
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

    public string ChangeAttackPattern(float probability)
    {
        Mobile ow = Owner as Mobile;
        if (probability <= ow.AttackProbability.AlphaProbability)
        {
            return "Alpha";
        }

        if (probability > ow.AttackProbability.AlphaProbability && probability <= ow.AttackProbability.BetaProbability)
        {
            return "Beta";
        }

        if (probability > ow.AttackProbability.BetaProbability && probability <= ow.AttackProbability.GammaProbability)
        {
            return "Gamma";
        }

        return "Null";
    }

    public void AdmiralControlling(List<Mobile> Fleet, PlayerCommands command = null)
    {
        if (Owner.destroyed)
        {
            foreach (Mobile obj in Fleet)
            {
                if(obj.TimelyFleet.Any(x => x == Owner))obj.TimelyFleet.Remove(Owner as Mobile);
            }
        }
        List<Mobile> ShipsCanotFight = new List<Mobile>();
        List<Mobile> ShipsCanFight = new List<Mobile>();
        List<Mobile> ShipsWithoutShield = new List<Mobile>();
        List<Mobile> DamagedShips = new List<Mobile>();

        List<SelectableObject> FleetAttackingList = new List<SelectableObject>();

        foreach (Mobile _s in Fleet)
        {
            PrimaryWeaponSS _pw = null;
            SecondaryWeaponSS _sw = null;

            if (_s._hs.SubSystems[0].gameObject.GetComponent<PrimaryWeaponSS>())
            {
                _pw = _s._hs.SubSystems[0].gameObject.GetComponent<PrimaryWeaponSS>();
            }

            if (_s._hs.SubSystems[0].gameObject.GetComponent<SecondaryWeaponSS>())
            {
                _sw = _s._hs.SubSystems[0].gameObject.GetComponent<SecondaryWeaponSS>();
            }

            if (_pw == null && _sw == null)
            {
                if (!ShipsCanotFight.Any(x => x == _s))
                {
                    ShipsCanotFight.Add(_s);
                }
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
                    if (!ShipsCanotFight.Any(x => x == _s))
                    {
                        ShipsCanotFight.Add(_s);
                    }

                    if (ShipsCanFight.Any(x => x == _s))
                    {
                        ShipsCanFight.Remove(_s);
                    }
                }
                else
                {
                    if (ShipsCanotFight.Any(x => x == _s))
                    {
                        ShipsCanotFight.Remove(_s);
                    }

                    if (!ShipsCanFight.Any(x => x == _s))
                    {
                        ShipsCanFight.Add(_s);
                    }
                }
            }

            if (_s._hs.Shilds.Length > 0)
            {
                if (_s._hs.Shilds[0].SubSystemCurHealth <= 0)
                {
                    if (!ShipsWithoutShield.Any(x => x == _s))
                    {
                        ShipsWithoutShield.Add(_s);
                    }
                }
                else
                {
                    if (ShipsWithoutShield.Any(x => x == _s))
                    {
                        ShipsWithoutShield.Remove(_s);
                    }
                }
            }
            else
            {
                if (!ShipsWithoutShield.Any(x => x == _s))
                {
                    ShipsWithoutShield.Add(_s);
                }
            }

            if (_s._hs.curHull <= _s._hs.MaxHull / 2)
            {
                if (!DamagedShips.Any(x => x == _s))
                {
                    DamagedShips.Add(_s);
                }
            }
            else
            {
                if (DamagedShips.Any(x => x == _s))
                {
                    DamagedShips.Remove(_s);
                }
            }

            if (_s._hs.MaxCrew > 0)
            {
                if (_s._hs.curCrew <= _s._hs.MaxHull / 3)
                {
                    if (!DamagedShips.Any(x => x == _s))
                    {
                        DamagedShips.Add(_s);
                    }
                }
                else
                {
                    if (DamagedShips.Any(x => x == _s))
                    {
                        DamagedShips.Remove(_s);
                    }
                }
            }

            if (_s.captain.Command != PlayerCommand.None)
            {
                if (_s.captain.Command == PlayerCommand.Attack)
                {
                    if ((_s.captain.curCommandInfo as AttackCommand).attackTarget != null && (_s.captain.curCommandInfo as AttackCommand).attackTarget.destroyed)
                    {
                        _s.captain.Command = PlayerCommand.None;
                        _s.captain.curCommandInfo = null;
                    }
                }
            }
        }

        foreach (Mobile _scf in ShipsCanFight)
        {
            foreach (SelectableObject tar in _scf.captain.Gunner.TargetsUnderAttack)
            {
                if (!FleetAttackingList.Any(x => x == tar))
                {
                    FleetAttackingList.Add(tar);
                }
            }
        }

        STMethods.RemoveAllNullsFromList(FleetAttackingList);

        STMethods.RemoveAllNullsFromList(ShipsCanotFight);
        STMethods.RemoveAllNullsFromList(ShipsCanFight);
        STMethods.RemoveAllNullsFromList(ShipsWithoutShield);
        STMethods.RemoveAllNullsFromList(DamagedShips);

        if (ShipsCanotFight.Count > 0)
        {
            foreach (Mobile _scnf in ShipsCanotFight)
            {
                if (_scnf.ThreateningEnemyObjects.Count > 0)
                {
                    List<SelectableObject> convertedList =
                        ShipsCanotFight.ConvertAll(
                            new Converter<Mobile, SelectableObject>(STMethods.MobileToSelectableObject));
                    Mobile coverignShip = STMethods.NearestSelObj(Owner, convertedList) as Mobile;

                    HideCoverCommand com = new HideCoverCommand();
                    com.command = "Hide";
                    com.coverignShip = coverignShip;
                    com.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects[0];
                    com.MainCommand = command;
                    _scnf.captain.EnterCommand(com);

                    if (command == null)
                    {
                        HideCoverCommand com2 = new HideCoverCommand();
                        com2.command = "Cover";
                        com2.coverignShip = _scnf;
                        com2.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects[0];
                        com2.MainCommand = command;
                        coverignShip.captain.EnterCommand(com2);
                    }
                }
            }
        }

        if (DamagedShips.Count > 0)
        {
            foreach (Mobile _scnf in DamagedShips)
            {
                if (_scnf.ThreateningEnemyObjects.Count > 0)
                {
                    List<SelectableObject> convertedList =
                        ShipsCanotFight.ConvertAll(
                            new Converter<Mobile, SelectableObject>(STMethods.MobileToSelectableObject));
                    Mobile coverignShip = STMethods.NearestSelObj(Owner, convertedList) as Mobile;

                    HideCoverCommand com = new HideCoverCommand();
                    com.command = "Hide";
                    com.coverignShip = coverignShip;
                    com.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects[0];
                    com.MainCommand = command;
                    _scnf.captain.EnterCommand(com);

                    HideCoverCommand com2 = new HideCoverCommand();
                    com2.command = "Cover";
                    com2.coverignShip = _scnf;
                    com2.ThreateningEnemyObjects = _scnf.ThreateningEnemyObjects[0];
                    com2.MainCommand = command;
                    coverignShip.captain.EnterCommand(com2);
                }
            }
        }

        if (command == null)
        {
            if (FleetAttackingList.Count > 0)
            {
                foreach (SelectableObject tar in FleetAttackingList)
                {
                    if (tar.healthSystem)
                    {
                        if (tar._hs.Shilds.Length > 0)
                        {
                            if (tar._hs.Shilds[0].SubSystemCurHealth <= 0)
                            {
                                foreach (Mobile _s in ShipsCanFight)
                                {
                                    if (_s.captain.Gunner.MainTarget._hs.Shilds != null && _s.captain.Gunner.MainTarget._hs.Shilds.Length > 0)
                                    {
                                        if (_s.captain.Gunner.MainTarget._hs.Shilds[0].SubSystemCurHealth <= 0)
                                        {
                                            _s.captain.Gunner.MainTarget = tar;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            switch (command.command)
            {
                case "Attack":
                    AttackCommand _ac = curCommandInfo as AttackCommand;
                    if (ShipsCanFight.Count > 0)
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
                        for (int i = Fleet.Count-1; i >= 0; i--)
                        {
                            if (Fleet[i] == all)
                            {
                                all.captain.tarVec = _mc.targetVec[i];
                                all.captain.Pilot.FleetMovement(_mc.targetVec[i], Fleet, _mc.Warp);
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void EnterCommand(PlayerCommands newCommand)
    {
        UndockingCommand _uc = new UndockingCommand();
        switch (newCommand.command)
        {
            case "Move":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.Move;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "Attack":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.Attack;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "Cover":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.Cover;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "Hide":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.Hide;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "FullStop":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.FullStop;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "Guard":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.Guard;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "Patrol":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.Patrol;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "Mine":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Miner.ToBase = (curCommandInfo as MiningCommand).ToBase;
                    Command = PlayerCommand.Mine;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
            case "Fixing":
                if (!ToStayPoint && !ToExitPoint)
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    curCommandInfo = newCommand;
                    Command = PlayerCommand.FixIn;
                }
                else
                {
                    if (Gunner != null) Gunner.StopFiring();
                    PatrolPositionNum = 0;
                    _uc.commandAfterUndocking = newCommand;
                    _uc.DocingStation = dockingStation;
                    _uc.Hub = dockingHub;
                    curCommandInfo = _uc;
                    Command = PlayerCommand.Undocking;
                }
                break;
        }
    }

    public void PerformCommand()
    {
        Mobile ow = null;
        if (Owner is Mobile) ow = Owner as Mobile;
        GameManager _gm = FindObjectOfType<GameManager>();
        switch (Command)
        {
            case PlayerCommand.Move:
                if (Pilot != null)
                {
                    MoveCommand _mc = curCommandInfo as MoveCommand;
                    if (ow.TimelyFleet != null && ow.TimelyFleet.Count > 0)
                    {
                        if (ow.TimelyFleet[0] == Owner)
                        {
                            AdmiralControlling(ow.TimelyFleet, _mc);
                        }
                    }
                    else
                    {
                        Pilot.Arrival(_mc.targetVec[0], 0, _mc.Warp);
                        tarVec = _mc.targetVec[0];

                        float distance = (_mc.targetVec[0] - transform.position).magnitude;
                        if (distance < 2)
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
                    return;
                }

                if (Pilot != null)
                {
                    if (ow.TimelyFleet != null && ow.TimelyFleet.Count > 0 && ow.TimelyFleet[0] == Owner)
                    {
                        AdmiralControlling(ow.TimelyFleet, _ac);
                    }

                    if (Pilot.Status != EngineController.State.Disabled)
                    {
                        if (_ac.attackTarget != null)
                        {
                            if (Pilot.Status != EngineController.State.AttackingAlpha ||
                                Pilot.Status != EngineController.State.AttackingBeta ||
                                Pilot.Status != EngineController.State.AttackingGamma)
                            {
                                float random = UnityEngine.Random.Range(0f, 1f);
                                switch (ChangeAttackPattern(random))
                                {
                                    case "Alpha":
                                        Pilot.AttaсkAlpha(_ac.attackTarget, Gunner);
                                        break;
                                    case "Beta":
                                        Pilot.AttackBeta(_ac.attackTarget, Gunner);
                                        break;
                                    case "Gamma":
                                        Pilot.AttackGamma(_ac.attackTarget, Gunner);
                                        break;
                                }
                            }
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
                    return;
                }

                if (ow.TimelyFleet != null && ow.TimelyFleet.Count > 0 && ow.TimelyFleet[0] == Owner)
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
                                    if (Pilot.Status != EngineController.State.AttackingAlpha ||
                                        Pilot.Status != EngineController.State.AttackingBeta ||
                                        Pilot.Status != EngineController.State.AttackingGamma)
                                    {
                                        float random = UnityEngine.Random.Range(0f, 1f);
                                        switch (ChangeAttackPattern(random))
                                        {
                                            case "Alpha":
                                                Pilot.AttaсkAlpha(_cc.ThreateningEnemyObjects, Gunner,
                                                    _cc.coverignShip.rigitBody.velocity.magnitude);
                                                break;
                                            case "Beta":
                                                Pilot.AttackBeta(_cc.ThreateningEnemyObjects, Gunner,
                                                    _cc.coverignShip.rigitBody.velocity.magnitude);
                                                break;
                                            case "Gamma":
                                                Pilot.AttackGamma(_cc.ThreateningEnemyObjects, Gunner,
                                                    _cc.coverignShip.rigitBody.velocity.magnitude);
                                                break;
                                        }
                                    }
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
                if (_hc.coverignShip.destroyed || _hc.ThreateningEnemyObjects.destroyed)
                {
                    Command = PlayerCommand.None;
                    curCommandInfo = null;
                    return;
                }

                if (ow.TimelyFleet != null && ow.TimelyFleet.Count > 0 && ow.TimelyFleet[0] == Owner)
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
                if (Pilot != null) Pilot.Stop();
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
                        _gc.guardTarget.ThreateningEnemyObjects.Count > 0)
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
                        _gc.guardTarget.ProtectionFleet.Add(Owner as Mobile);
                    if (Owner.destroyed)
                        if (_gc.guardTarget.ProtectionFleet.Any(x => x == Owner))
                            _gc.guardTarget.ProtectionFleet.Remove(Owner as Mobile);
                    foreach (Mobile all in _gc.guardTarget.ProtectionFleet)
                    {
                        if (all.captain.Pilot.Status != EngineController.State.Disabled)
                        {
                            for (int i = _gc.guardTarget.ProtectionFleet.Count - 1; i >= 0; i--)
                            {
                                if (_gc.guardTarget.ProtectionFleet[i] == all)
                                {
                                    all.captain.tarVec = _gc.fleetPattern[i];
                                    float maxRad = STMethods.MaxRadiusInFleet(_gc.guardTarget.ProtectionFleet) *
                                                   (2 + (_gc.guardTarget.ProtectionFleet.Count / 16));
                                    Vector3 point = _gc.guardTarget.transform.position +
                                                    (_gc.guardTarget.transform.rotation *
                                                     new Vector3(_gc.fleetPattern[i].x * maxRad,
                                                         _gc.fleetPattern[i].y * maxRad,
                                                         _gc.fleetPattern[i].z * maxRad));
                                    all.captain.Pilot.FleetMovement(point, _gc.guardTarget.ProtectionFleet);
                                }
                            }
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
                if (_pc.targetVec.Count > 0)
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
                            if (_mic.ResTar.curResources > 0)
                            {
                                MinerMining(_mic.ResTar);
                            }
                            else
                            {
                                _mic.ResTar = null;
                            }
                        }
                        else
                        {
                            List<SelectableObject> ResourceSourceList = new List<SelectableObject>();
                            for (int i = 0; i < _gm.SelectableObjects.Count; i++)
                            {
                                if (_gm.SelectableObjects[i].GetComponent<ResourceSource>() &&
                                    _gm.SelectableObjects[i].GetComponent<ResourceSource>().type ==
                                    Miner.curResourcesType && _gm.SelectableObjects[i].GetComponent<ResourceSource>()
                                        .curResources > 0)
                                {
                                    ResourceSourceList.Add(_gm.SelectableObjects[i]);
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
                            Pilot.Arrival(stayPoint);
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
                            Pilot.Arrival(exitPoint);
                        }
                        else
                        {
                            Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                        }
                    }
                    else
                    {
                        ToExitPoint = false;

                        EnterCommand(_uc.commandAfterUndocking);
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
        }
    }

    public void MinerDocking(ResourceUnloadingController _tarUC)
    {
        GameManager _gm = FindObjectOfType<GameManager>();

        _tarUC.DockingCall(Owner as Mobile);

        if (_tarUC.AwaitingShipsOnHub.Any(x => x == Owner))
        {
            for (int i = 0; i < _tarUC.AwaitingShipsOnHub.Count; i++)
            {
                if (_tarUC.AwaitingShipsOnHub[i] == Owner)
                {
                    Vector3 awaitingPoint = _tarUC.WaitingPoints()[i];
                    if (Vector3.Distance(transform.position, awaitingPoint) > Pilot.engines.Threshold)
                    {
                        Pilot.Arrival(awaitingPoint);
                    }
                    else
                    {
                        Vector3 targetDir = _tarUC.AwaitingPoint - transform.position;
                        Pilot.engines.RotateShip(targetDir);
                    }
                }
            }
        }
        else
        {
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
                        Pilot.Arrival(enterPoint);
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
                        if(dockingStation != _tarUC.HubSS.Owner) dockingStation = _tarUC.HubSS.Owner;
                        if(dockingHub != _tarUC.HubSS.Hubs[i]) dockingHub = _tarUC.HubSS.Hubs[i];
                        
                        if (Vector3.Distance(transform.position, stayPoint) > Pilot.engines.Threshold)
                        {
                            Vector3 targetDir = stayPoint - transform.position;
                            float angle = Vector3.Angle(targetDir, transform.forward);

                            if (angle < 2)
                            {
                                Pilot.Arrival(stayPoint);
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
                        if(dockingStation != _tarUC.HubSS.Owner) dockingStation = _tarUC.HubSS.Owner;
                        if(dockingHub != _tarUC.HubSS.Hubs[i]) dockingHub = _tarUC.HubSS.Hubs[i];
                        
                        if (Vector3.Distance(transform.position, exitPoint) > Pilot.engines.Threshold)
                        {
                            Vector3 targetDir = exitPoint - transform.position;
                            float angle = Vector3.Angle(targetDir, transform.forward);

                            if (angle < 2)
                            {
                                Pilot.Arrival(exitPoint);
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
                }
            }
        }
    }

    public void MinerMining(ResourceSource ResTar)
    {
        if (Miner.curResourcesType != ResTar.type)
        {
            Miner.curResources = 0;
            Miner.curResourcesType = ResTar.type;
        }
        float radius = Owner.ObjectRadius+ResTar.ObjectRadius+Owner.WeaponRange*0.75f;
        Pilot.Orbiting(ResTar, radius);

        if (Vector3.Distance(transform.position, ResTar.transform.position) <= (Owner.ObjectRadius+ResTar.ObjectRadius+Owner.WeaponRange))
        {
            Miner.Mine(ResTar);
        }
    }
    
    public void FixingDocking(FixingPointController _tarFC)
    {
        GameManager _gm = FindObjectOfType<GameManager>();

        _tarFC.DockingCall(Owner as Mobile);

        if (_tarFC.AwaitingShipsOnHub.Any(x => x == Owner))
        {
            for (int i = 0; i < _tarFC.AwaitingShipsOnHub.Count; i++)
            {
                if (_tarFC.AwaitingShipsOnHub[i] == Owner)
                {
                    Vector3 awaitingPoint = _tarFC.WaitingPoints()[i];
                    if (Vector3.Distance(transform.position, awaitingPoint) > Pilot.engines.Threshold)
                    {
                        Pilot.Arrival(awaitingPoint);
                    }
                    else
                    {
                        Vector3 targetDir = _tarFC.AwaitingPoint - transform.position;
                        Pilot.engines.RotateShip(targetDir);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _tarFC.HubCount; i++)
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
                        Pilot.Arrival(enterPoint);
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
                        if(dockingStation != _tarFC.HubSS.Owner) dockingStation = _tarFC.HubSS.Owner;
                        if(dockingHub != _tarFC.HubSS.Hubs[i]) dockingHub = _tarFC.HubSS.Hubs[i];
                        
                        if (Vector3.Distance(transform.position, stayPoint) > Pilot.engines.Threshold)
                        {
                            Vector3 targetDir = stayPoint - transform.position;
                            float angle = Vector3.Angle(targetDir, transform.forward);

                            if (angle < 2)
                            {
                                Pilot.Arrival(stayPoint);
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
                        if(dockingStation != _tarFC.HubSS.Owner) dockingStation = _tarFC.HubSS.Owner;
                        if(dockingHub != _tarFC.HubSS.Hubs[i]) dockingHub = _tarFC.HubSS.Hubs[i];
                        
                        if (Vector3.Distance(transform.position, exitPoint) > Pilot.engines.Threshold)
                        {
                            Vector3 targetDir = exitPoint - transform.position;
                            float angle = Vector3.Angle(targetDir, transform.forward);

                            if (angle < 2)
                            {
                                Pilot.Arrival(exitPoint);
                            }
                            else
                            {
                                Pilot.engines.RotateShip(Quaternion.LookRotation(targetDir));
                            }
                        }
                        else
                        {
                            ToExitPoint = false;
                            
                            MoveCommand _nmc = new MoveCommand();
                            _nmc.command = "Move";
                            _nmc.targetVec = new List<Vector3>(); 
                            _nmc.targetVec.Add(dockingStation.GetComponent<FixingPointController>().ExitFlag.ExitFlag);
                            _nmc.Warp = false;
                            
                            EnterCommand(_nmc);
                            
                            dockingStation = null;
                            dockingHub = null;
                        }
                    }
                }
            }
        }
    }
}