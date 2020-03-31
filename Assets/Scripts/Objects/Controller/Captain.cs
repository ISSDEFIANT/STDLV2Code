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

    private float fleetStabTimer = 3;

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
                                        switch (ChangeAttackPattern())
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

    public string ChangeAttackPattern()
    {
        float random = UnityEngine.Random.Range(0, 1);
        Mobile ow = Owner as Mobile;
        if (random <= ow.AttackProbability.AlphaProbability)
        {
            return "Alpha";
        }

        if (random > ow.AttackProbability.AlphaProbability && random <= ow.AttackProbability.BetaProbability)
        {
            return "Beta";
        }

        if (random > ow.AttackProbability.BetaProbability && random <= ow.AttackProbability.GammaProbability)
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

                if (_pw != null)
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
                                    if (_s.captain.Gunner.MainTarget._hs.Shilds.Length > 0)
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
                                all.captain.Pilot.FleetMovement(_mc.targetVec[i], Fleet);
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void EnterCommand(PlayerCommands newCommand)
    {
        switch (newCommand.command)
        {
            case "Move":
                if (Gunner != null) Gunner.MainTarget = null;
                curCommandInfo = newCommand;
                Command = PlayerCommand.Move;
                break;
            case "Attack":
                if (Gunner != null) Gunner.MainTarget = null;
                curCommandInfo = newCommand;
                Command = PlayerCommand.Attack;
                break;
            case "Cover":
                if (Gunner != null) Gunner.MainTarget = null;
                curCommandInfo = newCommand;
                Command = PlayerCommand.Cover;
                break;
            case "Hide":
                if (Gunner != null) Gunner.MainTarget = null;
                curCommandInfo = newCommand;
                Command = PlayerCommand.Hide;
                break;
            case "FullStop":
                if (Gunner != null) Gunner.MainTarget = null;
                curCommandInfo = newCommand;
                Command = PlayerCommand.FullStop;
                break;
            case "Guard":
                if (Gunner != null) Gunner.MainTarget = null;
                curCommandInfo = newCommand;
                Command = PlayerCommand.Guard;
                break;
        }
    }

    public void PerformCommand()
    {
        Mobile ow = null;
        if (Owner is Mobile) ow = Owner as Mobile;
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
                        Pilot.Arrival(_mc.targetVec[0]);
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
                            switch (ChangeAttackPattern())
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
                                    switch (ChangeAttackPattern())
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
                if (Gunner != null) Gunner.MainTarget = null;
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
                    if (_gc.guardTarget.ThreateningEnemyObjects != null && _gc.guardTarget.ThreateningEnemyObjects.Count > 0)
                    {
                        if (Gunner != null) Gunner.MainTarget = STMethods.NearestSelObj(_gc.guardTarget, _gc.guardTarget.ThreateningEnemyObjects);
                        tarSelObj = Gunner.MainTarget;
                    }
                }

                if (Pilot != null)
                {
                    if(!_gc.guardTarget.ProtectionFleet.Any(x => x == Owner))_gc.guardTarget.ProtectionFleet.Add(Owner as Mobile);
                    if(Owner.destroyed)if(_gc.guardTarget.ProtectionFleet.Any(x => x == Owner))_gc.guardTarget.ProtectionFleet.Remove(Owner as Mobile);
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
                                    Vector3 point = _gc.guardTarget.transform.position + (_gc.guardTarget.transform.rotation * new Vector3(_gc.fleetPattern[i].x * maxRad,_gc.fleetPattern[i].y * maxRad, _gc.fleetPattern[i].z * maxRad));
                                    all.captain.Pilot.FleetMovement(point, _gc.guardTarget.ProtectionFleet);
                                }
                            }
                        }
                    }
                }
                break;
        }
    }
}