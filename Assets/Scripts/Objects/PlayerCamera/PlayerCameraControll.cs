using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using Debug = UnityEngine.Debug;

public class PlayerCameraControll : MonoBehaviour
{
    public int PlayerNum = 1;

    /// <summary> Работает ли система выделения. </summary>
    public bool SelectionLock;

    public Texture2D SelectionFrame;

    public List<SelectableObject> SelectionList = new List<SelectableObject>();

    private GameManager manager;

    public bool isSelect;

    private float _selX;
    private float _selY;
    private float _selWidth;
    private float _selHeight;

    private float _selXOld;
    private float _selYOld;

    public List<Vector3> pos;
    public List<Vector3> protectionPositions;
    public List<Vector3> staticProtectionPositions;

    private Ray _gridRay;
    private RaycastHit _gridHit;

    private Vector2 _startPoint;
    private Vector2 _endPoint;

    private Ray _ray;
    private RaycastHit _hit;

    public int LeftMClickCount = 0;
    public float LeftMClickTimer = 0.3f;

    public bool LockOnOrderSet;
    public Captain.PlayerCommand SetNewCommand;

    public bool LockSelection;

    //[HideInInspector]
    public bool SettingPatrolWay;

    //[HideInInspector]
    public bool LockOnPatrolSetting;
    [HideInInspector] public PatrolCommand _pc = new PatrolCommand();
    [HideInInspector] public MiningCommand _mc = new MiningCommand();

    public FleetControllingFields Fleets;
    private int NumClickCount;
    private float NumClickDelay;

    private GameObject MoveMark;
    private GameObject AttackMark;
    private GameObject GuardMark;

    public GameObject HighGrid;
    public GameObject Flag;

    public SelectableObject HoveringList;

    // Start is called before the first frame update
    void Start()
    {
        MoveMark = (GameObject) Resources.Load("Effects/PlayerEffects/OrderEffects/MoveMark");
        AttackMark = (GameObject) Resources.Load("Effects/PlayerEffects/OrderEffects/AttackMark");
        GuardMark = (GameObject) Resources.Load("Effects/PlayerEffects/OrderEffects/GuardMark");

        manager = GameObject.FindObjectOfType<GameManager>();
        HighGrid = GameObject.FindObjectOfType<HighGridColorControl>().gameObject;
        Flag = Instantiate((GameObject) Resources.Load("Effects/PlayerEffects/OrderEffects/MoveFlag"));

        manager.Players[PlayerNum - 1].CameraControll = gameObject;

        SelectionFrame = GetSelectionFrame(manager.Players[PlayerNum - 1].race);

        NumClickDelay = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!SelectionLock)
        {
            HoverProcedure();

            float X;
            float Y;
            X = Screen.width / 100;
            Y = Screen.height / 100;

            if (LeftMClickCount > 0)
            {
                if (LeftMClickTimer > 0)
                {
                    LeftMClickTimer -= Time.deltaTime;
                }
                else
                {
                    LeftMClickCount = 0;
                    LeftMClickTimer = 0.3f;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectPro();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isSelect = false;
                if (Input.mousePosition.y < Y * 25 && Input.mousePosition.y > Y * 0 && Input.mousePosition.x < X * 14 &&
                    Input.mousePosition.x > X * 0)
                {
                    Debug.Log("Interface");
                }
                else
                {
                    _endPoint = Input.mousePosition;
                    FindSelect();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                OrderSet();
                if(LockOnOrderSet)LockOnOrderSet = false;
            }

            if (isSelect)
            {
                _selX = Input.mousePosition.x;
                _selY = Screen.height - Input.mousePosition.y;
                _selWidth = _selXOld - Input.mousePosition.x;
                _selHeight = Input.mousePosition.y - _selYOld;
            }
        }

        if (SelectionList.Count > 0)
        {
            foreach (SelectableObject obj in SelectionList)
            {
                obj.isSelected = true;
                obj.ShowSelectionEffect(0.2f);

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    obj.ObjectOrdersHigh += Input.GetAxis("Mouse Y");
                    Mathf.Clamp(obj.ObjectOrdersHigh, -100, 100);
                }
            }

            HighGrid.SetActive(true);
            HighGrid.transform.position = new Vector3(0, SelectionList[0].ObjectOrdersHigh, 0);

            if (SelectionList[0].GetComponent<FlagControll>())
            {
                Flag.transform.position = SelectionList[0].GetComponent<FlagControll>().ExitFlag;
                Flag.SetActive(true);
            }
            else
            {
                Flag.SetActive(false);
            }
        }
        else
        {
            HighGrid.SetActive(false);
            if (_mc != null) _mc = null;
            Flag.SetActive(false);

            LockOnOrderSet = false;
        }

        NumDownDetecting();
        OrderButtonDowns();
        if (NumClickCount > 0)
        {
            if (NumClickDelay > 0)
            {
                NumClickDelay -= Time.deltaTime;
            }
            else
            {
                NumClickDelay = 0.2f;
                NumClickCount = 0;
            }
        }

        if (LockOnPatrolSetting)
        {
            ActivatePatrol();
        }

        if (SettingPatrolWay)
        {
            LockOnPatrolSetting = true;
        }
    }

    private void SelectPro()
    {
        isSelect = true;
        _selXOld = Input.mousePosition.x;
        _selYOld = Input.mousePosition.y;

        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 9;

        _startPoint = Input.mousePosition;

        if (Physics.Raycast(_ray, out _hit, 10000.0f, layerMask))
        {
            if (_hit.transform.GetComponent<SelectableObject>())
            {
                SelectableObject seltar = _hit.transform.GetComponent<SelectableObject>();
                if (!LockOnOrderSet)
                {
                    if (!seltar.selectionLock)
                    {
                        isSelect = false;
                        LeftMClickCount += 1;

                        if (SelectionList.Count == 0)
                        {
                            SelectionList.Add(seltar);
                        }

                        if (!seltar.stationSelectionType)
                        {
                            if (SelectionList.Count == 1)
                            {
                                if (!SelectionList[0].stationSelectionType)
                                {
                                    if (LeftMClickCount == 2)
                                    {
                                        if (seltar.PlayerNum == PlayerNum)
                                        {
                                            FindShipType(seltar);
                                        }
                                    }
                                    else
                                    {
                                        if (!Input.GetKey(KeyCode.LeftShift))
                                        {
                                            SelectionList[0] = seltar;
                                        }
                                        else
                                        {
                                            if (!SelectionList.Any(x => x == seltar))
                                            {
                                                SelectionList.Add(seltar);
                                            }
                                            else
                                            {
                                                SelectionList.Remove(seltar);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SelectionList.Clear();
                                    SelectionList.Add(seltar);
                                }
                            }
                            else if (SelectionList.Count > 1)
                            {
                                if (!Input.GetKey(KeyCode.LeftShift))
                                {
                                    SelectionList.Clear();
                                }

                                if (!SelectionList.Any(x => x == seltar) && SelectionList.Count < 64)
                                {
                                    SelectionList.Add(seltar);
                                }
                            }
                        }
                        else
                        {
                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
                                SelectionList.Clear();
                                SelectionList.Add(seltar);
                            }
                        }
                    }
                }
                else
                {
                    switch (SetNewCommand)
                    {
                        case Captain.PlayerCommand.Attack:
                            AttackCommand _ac = new AttackCommand();
                            _ac.command = "Attack";
                            _ac.attackTarget = seltar;
                            if (SelectionList.Count == 1)
                            {
                                if (SelectionList[0] is Mobile)
                                {
                                    Mobile m = SelectionList[0] as Mobile;
                                    m.ResetTimelyFleet(null);
                                }

                                SelectionList[0].captain.EnterCommand(_ac);
                                Instantiate(AttackMark, seltar.transform.position, Quaternion.Euler(Vector3.zero));
                                return;
                            }
                            else if (SelectionList.Count > 1)
                            {
                                List<Mobile> ml = SetTimelyFleet(SelectionList.ToList());
                                foreach (Mobile obj in ml)
                                {
                                    obj.captain.EnterCommand(_ac);
                                }

                                Instantiate(AttackMark, seltar.transform.position, Quaternion.Euler(Vector3.zero));
                            }

                            LockOnOrderSet = false;
                            break;
                        case Captain.PlayerCommand.Guard:
                            GuardCommand _gc = new GuardCommand();
                            if (seltar is Mobile)
                            {
                                _gc.fleetPattern = protectionPositions;
                            }
                            else if (seltar is Static)
                            {
                                _gc.fleetPattern = staticProtectionPositions;
                            }

                            _gc.command = "Guard";
                            _gc.guardTarget = seltar;
                            List<Mobile> ml2 = SetTimelyFleet(SelectionList.ToList());
                            foreach (Mobile obj in ml2)
                            {
                                obj.captain.EnterCommand(_gc);
                            }

                            Instantiate(GuardMark, seltar.transform.position, Quaternion.Euler(Vector3.zero));
                            LockOnOrderSet = false;
                            break;
                    }
                }
            }
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift) && !SettingPatrolWay)
            {
                SelectionList.Clear();
            }
        }

        if (SettingPatrolWay)
        {
            _gridRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMaskGrid = 1 << 10;
            if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMaskGrid))
            {
                _pc.targetVec.Add(_gridHit.point + new Vector3(0, SelectionList[0].ObjectOrdersHigh, 0));
            }
        }
    }

    void ActivatePatrol()
    {
        if (!SettingPatrolWay)
        {
            if (SelectionList.Count > 0)
            {
                if (SelectionList[0].PlayerNum == PlayerNum)
                {
                    foreach (SelectableObject obj in SelectionList)
                    {
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            if (_pc.targetVec != null && _pc.targetVec.Count > 0)
                            {
                                obj.captain.EnterCommand(_pc);
                            }

                            LockOnPatrolSetting = false;
                            SetNewCommand = Captain.PlayerCommand.None;

                        }
                    }
                }
                else
                {
                    LockOnPatrolSetting = false;
                    SetNewCommand = Captain.PlayerCommand.None;
                }
            }
            else
            {
                LockOnPatrolSetting = false;
                SetNewCommand = Captain.PlayerCommand.None;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (SelectionList.Count > 0)
                {
                    if (SelectionList[0].PlayerNum == PlayerNum)
                    {
                        foreach (SelectableObject obj in SelectionList)
                        {
                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
                                if (_pc.targetVec != null && _pc.targetVec.Count > 0)
                                {
                                    obj.captain.EnterCommand(_pc);
                                }

                                LockOnPatrolSetting = false;
                                SettingPatrolWay = false;
                                SetNewCommand = Captain.PlayerCommand.None;
                            }
                        }
                    }
                    else
                    {
                        LockOnPatrolSetting = false;
                        SettingPatrolWay = false;
                        SetNewCommand = Captain.PlayerCommand.None;
                    }
                }
                else
                {
                    LockOnPatrolSetting = false;
                    SettingPatrolWay = false;
                    SetNewCommand = Captain.PlayerCommand.None;
                }
            }
        }
    }

    private void OrderSet()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 9;

        if (Physics.Raycast(_ray, out _hit, 10000.0f, layerMask))
        {
            if (!LockOnPatrolSetting)
            {
                if (_hit.transform.GetComponent<SelectableObject>())
                {
                    SelectableObject tar = _hit.transform.GetComponent<SelectableObject>();
                    if (tar.PlayerNum != 0)
                    {
                        if (manager.Players[tar.PlayerNum - 1].TeamNum != manager.Players[PlayerNum - 1].TeamNum)
                        {
                            AttackCommand com = new AttackCommand();
                            com.command = "Attack";
                            com.attackTarget = tar;
                            if (SelectionList.Count == 1)
                            {
                                if (SelectionList[0].PlayerNum == PlayerNum)
                                {
                                    if (SelectionList[0] is Mobile)
                                    {
                                        Mobile m = SelectionList[0] as Mobile;
                                        m.ResetTimelyFleet(null);
                                    }

                                    SelectionList[0].captain.EnterCommand(com);
                                    Instantiate(AttackMark, tar.transform.position, Quaternion.Euler(Vector3.zero));
                                    return;
                                }
                            }

                            if (SelectionList.Count > 1)
                            {
                                if (SelectionList[0].PlayerNum == PlayerNum)
                                {
                                    List<Mobile> ml = SetTimelyFleet(SelectionList.ToList());
                                    foreach (Mobile obj in ml)
                                    {
                                        obj.captain.EnterCommand(com);
                                    }

                                    Instantiate(AttackMark, tar.transform.position, Quaternion.Euler(Vector3.zero));

                                    return;
                                }
                            }
                        }

                        if (tar.GetComponent<ResourceUnloadingController>())
                        {
                            if (tar.PlayerNum == PlayerNum)
                            {
                                if (SelectionList.Count > 0)
                                {
                                    if (SelectionList[0].PlayerNum == PlayerNum)
                                    {
                                        if (_mc == null) _mc = new MiningCommand();
                                        _mc.command = "Mine";
                                        _mc.UnloadPoint = tar;
                                        _mc.ToBase = true;
                                    
                                        foreach (SelectableObject obj in SelectionList)
                                        {
                                            if (obj.GetComponent<MiningController>())
                                            {
                                                obj.captain.EnterCommand(_mc);
                                            }   
                                        }

                                        if (_mc.ResTar != null) _mc = null;
                                    }
                                }
                            }
                        }
                        if (tar.GetComponent<FixingPointController>())
                        {
                            if (tar.PlayerNum == PlayerNum)
                            {
                                if (SelectionList.Count > 0)
                                {
                                    if (SelectionList[0].PlayerNum == PlayerNum)
                                    {
                                        FixingCommand _fc = new FixingCommand();
                                        _fc.command = "Fixing";
                                        _fc.FixingPoint = tar;
                                    
                                        foreach (SelectableObject obj in SelectionList)
                                        {
                                            if (obj.healthSystem && obj._hs.NeedFix())
                                            {
                                                obj.captain.EnterCommand(_fc);
                                            }   
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (tar.PlayerNum == 0)
                    {
                        if (tar is ResourceSource)
                        {
                            if (SelectionList.Count > 0)
                            {
                                if (SelectionList[0].PlayerNum == PlayerNum)
                                {
                                    if (_mc == null) _mc = new MiningCommand();
                                    _mc.command = "Mine";
                                    _mc.ResTar = tar as ResourceSource;
                                    _mc.ToBase = false;
                                    
                                    foreach (SelectableObject obj in SelectionList)
                                    {
                                        if (obj.GetComponent<MiningController>())
                                        {
                                            obj.captain.EnterCommand(_mc);
                                        }   
                                    }

                                    if (_mc.UnloadPoint != null) _mc = null;
                                }
                            }
                        }
                    }
                }
            }

            return;
        }

        _gridRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMaskGrid = 1 << 10;
        if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMaskGrid))
        {
            if (SelectionList.Count > 0)
            {
                if (SelectionList[0].PlayerNum == PlayerNum)
                {
                    if (SelectionList[0] is Mobile)
                    {
                        List<Mobile> mlist = new List<Mobile>();
                        foreach (SelectableObject obj in SelectionList)
                        {
                            if (obj is Mobile)
                            {
                                mlist.Add(obj as Mobile);
                            }
                        }

                        if (!LockOnPatrolSetting)
                        {
                            if (mlist.Count > 1)
                            {
                                float maxRad = STMethods.MaxRadiusInFleet(mlist) * (2 + (mlist.Count / 16));
                                Vector3 relativePos = _gridHit.point - mlist[0].transform.position;
                                Quaternion rotation = Quaternion.LookRotation(relativePos);

                                MoveCommand com = new MoveCommand();
                                com.command = "Move";
                                com.targetVec = new List<Vector3>();
                                for (int i = 0; i < mlist.Count; i++)
                                {
                                    Vector3 rotateVector =
                                        _gridHit.point + (rotation * new Vector3(pos[i].x * maxRad, pos[i].y * maxRad,
                                                              pos[i].z * maxRad)) + new Vector3(0,
                                            mlist[i].ObjectOrdersHigh, 0);
                                    com.targetVec.Add(new Vector3(rotateVector.x, rotateVector.y, rotateVector.z));
                                    Instantiate(MoveMark,
                                        new Vector3(rotateVector.x, rotateVector.y,
                                            rotateVector.z), Quaternion.Euler(Vector3.zero));
                                }

                                foreach (Mobile _s in mlist)
                                {
                                    _s.ResetTimelyFleet(mlist.ToList());
                                    if (Input.GetKey(KeyCode.LeftControl))
                                    {
                                        com.Warp = true;
                                    }
                                    _s.captain.EnterCommand(com);
                                }

                                return;
                            }

                            if (mlist.Count == 1)
                            {
                                mlist[0].ResetTimelyFleet(null);
                                MoveCommand com = new MoveCommand();
                                com.command = "Move";
                                com.targetVec = new List<Vector3> {_gridHit.point + new Vector3(0, mlist[0].ObjectOrdersHigh,0)};
                                if (Input.GetKey(KeyCode.LeftControl))
                                {
                                    com.Warp = true;
                                }
                                mlist[0].captain.EnterCommand(com);
                                Instantiate(MoveMark, _gridHit.point + new Vector3(0, mlist[0].ObjectOrdersHigh, 0),
                                    Quaternion.Euler(Vector3.zero));
                            }
                        }
                        else
                        {
                            if (!SettingPatrolWay)
                            {
                                _pc.targetVec.Add(_gridHit.point +
                                                  new Vector3(0, SelectionList[0].ObjectOrdersHigh, 0));
                            }
                        }
                    }
                }
            }
        }
    }

    Texture2D GetSelectionFrame(STMethods.Races race)
    {
        switch (race)
        {
            case STMethods.Races.Federation:
                return (Texture2D) Resources.Load("Textures/RacesInterface/Fadaration/FedSelFrame");
            case STMethods.Races.Borg:
                return (Texture2D) Resources.Load("Textures/RacesInterface/Borg/BorgSelFrame");
            case STMethods.Races.Klingon:
                return (Texture2D) Resources.Load("Textures/RacesInterface/Klingon/KliSelFrame");
        }

        return null;
    }

    void FindShipType(SelectableObject Type)
    {
        if(Type.stationSelectionType) return;
        foreach (SelectableObject obj in manager.SelectableObjects)
        {
            SelectableObject _ost = obj;

            Vector2 objpos = Camera.main.WorldToScreenPoint(obj.transform.position);

            if ((objpos.x > 0 && objpos.x < Screen.width) || (objpos.x < 0 && objpos.x > Screen.width))
            {
                if ((objpos.y > 0 && objpos.y < Screen.height) || (objpos.y < 0 && objpos.y > Screen.height))
                {
                    if (!SelectionList.Any(x => x == obj) && SelectionList.Count < 12)
                    {
                        if (_ost.GetType() == Type.GetType())
                        {
                            if (_ost.PlayerNum == PlayerNum)
                            {
                                if (!obj.selectionLock)
                                {
                                    SelectionList.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void FindSelect()
    {
        foreach (SelectableObject obj in manager.SelectableObjects)
        {
            if (!obj.stationSelectionType)
            {
                Vector2 objpos = Camera.main.WorldToScreenPoint(obj.transform.position);

                if (((objpos.x > _startPoint.x && objpos.x < _endPoint.x) ||
                     (objpos.x < _startPoint.x && objpos.x > _endPoint.x)) &&
                    ((objpos.y > _startPoint.y && objpos.y < _endPoint.y) ||
                     (objpos.y < _startPoint.y && objpos.y > _endPoint.y)))
                {
                    if (!SelectionList.Any(x => x == obj) && SelectionList.Count < 12)
                    {
                        if (obj.PlayerNum == PlayerNum)
                        {
                            if (!obj.selectionLock)
                            {
                                SelectionList.Add(obj);
                            }
                        }
                    }
                }
            }
        }
    }

    void HoverProcedure()
    {
        foreach (SelectableObject obj in manager.SelectableObjects)
        {
            MaterialEffectController modelEffect = obj.modelEffects;


            RendererBoundsInScreenSpaceInfo _mpSpaceInfo = STMethods.RendererBoundsInScreenSpace(modelEffect._or);

            Vector2 mousePos = Input.mousePosition;

            if (_mpSpaceInfo.MinX < mousePos.x && _mpSpaceInfo.MaxX > mousePos.x &&
                _mpSpaceInfo.MinY < mousePos.y && _mpSpaceInfo.MaxY > mousePos.y)
            {
                obj.isHovering = true;
                obj.ShowSelectionEffect(0.1f);
            }
            else
            {
                obj.isHovering = false;
                obj.ShowSelectionEffect(0);
            }
        }
    }

    void NumDownDetecting()
    {
        if (Input.GetKeyDown("0"))
        {
            FleetSetting(0);
        }

        if (Input.GetKeyDown("1"))
        {
            FleetSetting(1);
        }

        if (Input.GetKeyDown("2"))
        {
            FleetSetting(2);
        }

        if (Input.GetKeyDown("3"))
        {
            FleetSetting(3);
        }

        if (Input.GetKeyDown("4"))
        {
            FleetSetting(4);
        }

        if (Input.GetKeyDown("5"))
        {
            FleetSetting(5);
        }

        if (Input.GetKeyDown("6"))
        {
            FleetSetting(6);
        }

        if (Input.GetKeyDown("7"))
        {
            FleetSetting(7);
        }

        if (Input.GetKeyDown("8"))
        {
            FleetSetting(8);
        }

        if (Input.GetKeyDown("9"))
        {
            FleetSetting(9);
        }
    }

    void OrderButtonDowns()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LockOnOrderSet = true;
            SetNewCommand = Captain.PlayerCommand.Attack;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            LockOnOrderSet = true;
            SetNewCommand = Captain.PlayerCommand.Guard;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _pc = new PatrolCommand();
            _pc.command = "Patrol";
            SettingPatrolWay = true;
            SetNewCommand = Captain.PlayerCommand.Patrol;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SelectionList.Count > 0)
            {
                if (SelectionList[0].PlayerNum == PlayerNum)
                {
                    FixingCommand _fc = new FixingCommand();
                    _fc.command = "Fixing";
                    _fc.FixingPoint = null;
                                    
                    foreach (SelectableObject obj in SelectionList)
                    {
                        if (obj.healthSystem && obj._hs.NeedFix())
                        {
                            obj.captain.EnterCommand(_fc);
                        }   
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!SettingPatrolWay)
            {
                _pc = new PatrolCommand();
                _pc.command = "Patrol";
                LockOnPatrolSetting = true;
                SetNewCommand = Captain.PlayerCommand.Patrol;
            }

        }
    }

    void FleetSetting(int num)
    {
        NumClickCount++;
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (SelectionList != null && SelectionList.Count > 0)
            {
                if (SelectionList[0].PlayerNum == PlayerNum)
                {
                    Fleets.fleets[num] = SelectionList.ToList();
                }
            }
        }
        else
        {
            if (Fleets.fleets[num] != null && Fleets.fleets[num].Count > 0)
                SelectionList = Fleets.fleets[num].ToList();
            if (NumClickCount == 2)
            {
                MoveCameraToFleet(Fleets.fleets[num]);
            }
        }
    }

    private void MoveCameraToFleet(List<SelectableObject> Fleet)
    {
        transform.position = new Vector3(Fleet[0].transform.position.x, transform.position.y,
            Fleet[0].transform.position.z);
    }

    void OnGUI()
    {
        if (isSelect)
        {
            GUI.DrawTexture(new Rect(_selX, _selY, _selWidth, _selHeight), SelectionFrame);
        }
    }

    public List<Mobile> SetTimelyFleet(List<SelectableObject> selList)
    {
        List<Mobile> mlist = new List<Mobile>();
        foreach (SelectableObject obj in selList)
        {
            if (obj is Mobile)
            {
                mlist.Add(obj as Mobile);
            }
        }

        foreach (Mobile obj in mlist)
        {
            obj.ResetTimelyFleet(mlist.ToList());
        }

        return mlist;
    }
}