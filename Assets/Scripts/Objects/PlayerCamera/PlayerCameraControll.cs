using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
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

    [HideInInspector]
    public bool SettingPatrolWay;

    public FleetControllingFields Fleets;
    private int NumClickCount;
    private float NumClickDelay;
    
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<GameManager>();

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
                if (Input.mousePosition.y < Y * 25 && Input.mousePosition.y > Y * 0 && Input.mousePosition.x < X * 14 && Input.mousePosition.x > X * 0)
                {
                    Debug.Log ("Interface");
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
            }
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
                                if (LeftMClickCount == 2)
                                {
                                    FindShipType(seltar);
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
                            if (SelectionList.Count > 1)
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
                                return;
                            }
                            else if (SelectionList.Count > 1)
                            {
                                List<Mobile> ml = SetTimelyFleet(SelectionList.ToList());
                                foreach (Mobile obj in ml)
                                {
                                    obj.captain.EnterCommand(_ac);
                                }
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
                            LockOnOrderSet = false;
                            break;
                    }
                }
            }
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                SelectionList.Clear();
            }
        }
    }

    private void OrderSet()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 9;

        if (Physics.Raycast(_ray, out _hit, 10000.0f, layerMask))
        {
            if (_hit.transform.GetComponent<SelectableObject>())
            {
                SelectableObject tar = _hit.transform.GetComponent<SelectableObject>();
                if (manager.Players[tar.PlayerNum - 1].TeamNum != manager.Players[PlayerNum - 1].TeamNum)
                {
                    AttackCommand com = new AttackCommand();
                    com.command = "Attack";
                    com.attackTarget = tar;
                    if (SelectionList.Count == 1)
                    {
                        if (SelectionList[0] is Mobile)
                        {
                            Mobile m = SelectionList[0] as Mobile;
                            m.ResetTimelyFleet(null);
                        }
                        SelectionList[0].captain.EnterCommand(com);
                        return;
                    }
                    if (SelectionList.Count > 1)
                    {
                        List<Mobile> ml = SetTimelyFleet(SelectionList.ToList());
                        foreach (Mobile obj in ml)
                        {
                            obj.captain.EnterCommand(com);
                        }
                        return;
                    }
                }
            }
        }

        _gridRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMaskGrid = 1 << 10;
        if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMaskGrid))
        {   
            if (SelectionList.Count > 0)
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

                    if (mlist.Count > 1)
                    {
                        float maxRad = STMethods.MaxRadiusInFleet(mlist)*(2+(mlist.Count/16));
                        Vector3 relativePos = _gridHit.point - mlist[0].transform.position;
                        Quaternion rotation = Quaternion.LookRotation(relativePos);
                        
                        MoveCommand com = new MoveCommand();
                        com.command = "Move";
                        com.targetVec = new List<Vector3>();
                        for (int i = 0; i < mlist.Count; i++)
                        {
                            Vector3 rotateVector = _gridHit.point + (rotation * new Vector3(pos[i].x * maxRad, pos[i].y * maxRad, pos[i].z * maxRad));
                            com.targetVec.Add(new Vector3(rotateVector.x, _gridHit.point.y, rotateVector.z));
                        }
                        
                        foreach (Mobile _s in mlist)
                        {
                            _s.ResetTimelyFleet(mlist.ToList());
                            _s.captain.EnterCommand(com);
                        }

                        return;
                    }

                    if (mlist.Count == 1)
                    {
                        mlist[0].ResetTimelyFleet(null);
                        MoveCommand com = new MoveCommand();
                        com.command = "Move";
                        com.targetVec = new List<Vector3>{_gridHit.point};
                        mlist[0].captain.EnterCommand(com);
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
            Vector2 objpos = Camera.main.WorldToScreenPoint(obj.transform.position);

            if (((objpos.x > _startPoint.x && objpos.x < _endPoint.x) || (objpos.x < _startPoint.x && objpos.x > _endPoint.x)) &&
                ((objpos.y > _startPoint.y && objpos.y < _endPoint.y) || (objpos.y < _startPoint.y && objpos.y > _endPoint.y)))
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
    }
    
    void FleetSetting(int num)
    {
        NumClickCount++;
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if(SelectionList != null && SelectionList.Count > 0)
            Fleets.fleets[num] = SelectionList.ToList();
        }
        else
        {
            if(Fleets.fleets[num] != null && Fleets.fleets[num].Count > 0)
            SelectionList = Fleets.fleets[num].ToList();
            if (NumClickCount == 2)
            {
                MoveCameraToFleet(Fleets.fleets[num]);
            }
        }
    }
    
    private void MoveCameraToFleet(List<SelectableObject> Fleet)
    {
        transform.position = new Vector3(Fleet[0].transform.position.x, transform.position.y, Fleet[0].transform.position.z);
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