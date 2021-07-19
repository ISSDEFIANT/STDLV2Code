using System;
using System.Collections.Generic;
using System.Linq;
using RTS_Cam;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerCameraControll : MonoBehaviour
{
    [HideInInspector]
    public bool KeyboardFastButtonsDeactive;
    
    public bool ShowGrid = true;
    public bool PlayersSetGridVisibility = true;
    public bool _3DVision;
    public SelectableObject _3DVisionTarget;
    
    public int PlayerNum = 1;

    /// <summary> Состояние камеры. </summary>
    public STMethods.PlayerCameraState CameraState;

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

    public int LeftMClickCount = 0;
    public float LeftMClickTimer = 0.3f;

    public Captain.PlayerCommand SetNewCommand;

    [HideInInspector]
    public bool ShiftPatrolSetting;
    [HideInInspector] public PatrolCommand _pc = new PatrolCommand();
    [HideInInspector] public MiningCommand _mc = new MiningCommand();

    public FleetControllingFields Fleets;
    private int NumClickCount;
    private float NumClickDelay;

    private GameObject MoveMark;
    private GameObject AttackMark;
    private GameObject GuardMark;

    public Renderer GridRenderer;
    public GameObject HighGrid;
    public LineRenderer HighLine;
    public GameObject Flag;

    public SelectableObject hoverignObject;

    public GameObject _ghost;
    [HideInInspector]
    public BuildingCommand _buildingCommand;
    
    public GraphicRaycaster interface_Raycaster;
    public EventSystem interfaceEventSystem;
    public GlobalInterfaceEventSystem globalInterface;

    private CircleRenderer SensorLine;
    private CircleRenderer WeaponLine;
    [HideInInspector]public Skill curSelectedAbility;

    private CameraZoom cameraZoom;
    private RTS_Camera cameraCom;
    protected SgtMouseLook cameraComRot;

    public Cameras PlayerCameras;

    private MiniMap _miniMapComponent;

    public GameObject FleetNameObject;
    public List<FleetObjectScript> FleetObjects;
    [Serializable]
    public class Cameras
    {
        public Camera MainCamera;
        public Camera LocalMinimapCamera;
        public Camera GlobalMinimapCamera;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerCameras.MainCamera = GetComponentInChildren<Camera>();
        _miniMapComponent = GetComponent<MiniMap>();
        PlayerCameras.LocalMinimapCamera = _miniMapComponent.itsMinimapCamera;

        cameraZoom = GetComponentInChildren<CameraZoom>();
        cameraCom = gameObject.GetComponent<RTS_Camera>();
        cameraComRot = gameObject.GetComponent<SgtMouseLook>();

        MoveMark = (GameObject) DataLoader.Instance.ResourcesCache["MoveMark"];
        AttackMark = (GameObject) DataLoader.Instance.ResourcesCache["AttackMark"];
        GuardMark = (GameObject) DataLoader.Instance.ResourcesCache["GuardMark"];

        SensorLine = Instantiate((GameObject) DataLoader.Instance.ResourcesCache["RangeLine"]).GetComponent<CircleRenderer>();
        SensorLine.gameObject.SetActive(false);
        SensorLine.color = new Color32(0,255,0,130);
        WeaponLine = Instantiate((GameObject) DataLoader.Instance.ResourcesCache["RangeLine"]).GetComponent<CircleRenderer>();
        WeaponLine.gameObject.SetActive(false);
        WeaponLine.color = new Color32(255,0,0,130);

        manager = GameManager.instance;
        HighGrid = GameObject.FindObjectOfType<HighGridMarker>().gameObject;
        GridRenderer = GameObject.FindObjectOfType<HighGridMarker>().Grid;
        HighLine = GameObject.FindObjectOfType<HighLineColorControll>().GetComponent<LineRenderer>();
        Flag = Instantiate((GameObject) DataLoader.Instance.ResourcesCache["MoveFlag"]);

        manager.Players[PlayerNum - 1].CameraControll = gameObject;

        SelectionFrame = GetSelectionFrame(manager.Players[PlayerNum - 1].race);

        NumClickDelay = 0.2f;

        switch (manager.Players[PlayerNum - 1].race)
        {
            case STMethods.Races.Federation:
                globalInterface = Instantiate((GameObject) DataLoader.Instance.ResourcesCache["Interface/Federation"]).GetComponent<GlobalInterfaceEventSystem>();
                globalInterface.player = this;
                globalInterface.PlayerNum = PlayerNum;
                interface_Raycaster = globalInterface.GetComponent<GraphicRaycaster>();
                interfaceEventSystem = globalInterface.GetComponent<EventSystem>();
                break;
        }

        for (int i = 0; i < 16; i++)
        {
            FleetObjects.Add(Instantiate(FleetNameObject).GetComponent<FleetObjectScript>());
            FleetObjects[FleetObjects.Count-1].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_3DVision)
        {
            cameraCom.enabled = false;
            if (_3DVisionTarget != null && !_3DVisionTarget.destroyed && _3DVisionTarget.isVisible == STMethods.Visibility.Visible)
            {
                transform.position = _3DVisionTarget.model.transform.position;
                cameraZoom.minHeight = -_3DVisionTarget.ObjectRadius;
            }
            else
            {
                _3DVisionTarget = null;
                cameraZoom.minHeight = -5;
            }

            cameraComRot.isBorderRotation = true;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            if (globalInterface.globalStatus == GlobalInterfaceEventSystem.globalInterfaceStatus.GlobalMinimap)
            {
                cameraCom.enabled = false;    
            }
            else
            {
                cameraCom.enabled = true;
            }

            cameraComRot.isBorderRotation = false;
            cameraZoom.minHeight = -5;
        }
        GridRenderer.enabled = ShowGrid;

        _gridRay = getWorkingCamera().ScreenPointToRay(Input.mousePosition);
        
        switch (CameraState)
        {
            case STMethods.PlayerCameraState.Normal:
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
                    List<RaycastResult> results = new List<RaycastResult>();
                    PointerEventData interfaceData = new PointerEventData(interfaceEventSystem);
                    interfaceData.position = Input.mousePosition;
                    interface_Raycaster.Raycast(interfaceData, results);

                    if (results.Count > 0) return;
                    
                    if (_mc != null) _mc = null;

                    SelectPro();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (isSelect)
                    {
                        isSelect = false;
                        if (Input.mousePosition.y < Y * 25 && Input.mousePosition.y > Y * 0 &&
                            Input.mousePosition.x < X * 14 &&
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

                if (!KeyboardFastButtonsDeactive)
                {
                    NumDownDetecting();
                    OrderButtonDowns();
                }

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

                if (ShiftPatrolSetting)
                {
                    if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        ActivatePatrol();
                    }
                }

                break;
            
            case STMethods.PlayerCameraState.OrderSetting:
                HoverProcedure();
                if (SelectionList.Count == 0)
                {
                    CameraState = STMethods.PlayerCameraState.Normal;
                    curSelectedAbility = null;
                }
                if (SelectionList.Count == 1)
                {
                    if (SetNewCommand == Captain.PlayerCommand.Attack)
                    {
                        if (SelectionList[0].PlayerNum != PlayerNum || SelectionList[0].captain == null || SelectionList[0].captain.Gunner == null)
                        {
                            CameraState = STMethods.PlayerCameraState.Normal;
                        }
                    }
                    if (SetNewCommand == Captain.PlayerCommand.Guard) //Сюда же в будущем команду Найти и уничтожить
                    {
                        if (SelectionList[0].PlayerNum != PlayerNum || SelectionList[0].captain == null || SelectionList[0].captain.Gunner == null || SelectionList[0].captain.Pilot == null)
                        {
                            CameraState = STMethods.PlayerCameraState.Normal;
                        }
                    }
                    if (SetNewCommand == Captain.PlayerCommand.Patrol) // Сюда же в будущем поиск
                    {
                        if (SelectionList[0].PlayerNum != PlayerNum || SelectionList[0].captain == null || SelectionList[0].captain.Pilot == null)
                        {
                            CameraState = STMethods.PlayerCameraState.Normal;
                        }
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (_mc != null) _mc = null;
                    SetLeftMouseButtonOrder();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    CameraState = STMethods.PlayerCameraState.Normal;
                }

                break;
            
            case STMethods.PlayerCameraState.PatrolSetting:
                if (Input.GetMouseButtonDown(0))
                {
                    if (_mc != null) _mc = null;
                    int playerMaskGrid = 1 << 10;
                    if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, playerMaskGrid))
                    {
                        _pc.targetVec.Add(_gridHit.point);
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    ActivatePatrol();
                }
                break;
            
            case STMethods.PlayerCameraState.BuildingPlacement:
                int layerMaskGrid = 1 << 10;
                if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMaskGrid))
                {
                    _ghost.transform.position = new Vector3(_gridHit.point.x, _gridHit.point.y,_gridHit.point.z);
                    
                    if (Input.GetMouseButtonDown(1))
                    {
                        _buildingCommand = new BuildingCommand();
                        Destroy(_ghost);
                        CameraState = STMethods.PlayerCameraState.Normal;
                        return;
                    }
                    int mask = 1 << 9;
                    Collider[] colls = Physics.OverlapSphere(_ghost.transform.position, _buildingCommand.target.ObjectRadius, mask);
                    if (colls.Length > 0)
                    {
                        MeshRenderer[] mats = _ghost.GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer _mr in mats)
                        {
                            _mr.material.SetColor("_InnerColor", new Color(1, 0, 0, 0.5f));
                        }
                    }
                    else
                    {
                        MeshRenderer[] mats = _ghost.GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer _mr in mats)
                        {
                            _mr.material.SetColor("_InnerColor", new Color(0, 1, 0, 0.5f));
                        }
                        
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (_mc != null) _mc = null;
                            _buildingCommand.posVec = _ghost.transform.position;
                            _buildingCommand.rotVec = _ghost.transform.rotation.eulerAngles;
                            _buildingCommand.ghost = _ghost;
                        
                            SelectionList[0].captain.EnterCommand(_buildingCommand);
                            
                            _ghost = null;
                            _startPoint = Vector2.zero;
                            _endPoint = Vector2.zero;
                            CameraState = STMethods.PlayerCameraState.Normal;
                            isSelect = false;
                        }
                    }
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _ghost.transform.Rotate(Vector3.up, Input.GetAxis("Mouse ScrollWheel")*10);
                }
                break;
            
            case STMethods.PlayerCameraState.FlagPlacing:
                HoverProcedure();
                if (SelectionList.Count == 1 && SelectionList[0].GetComponent<FlagControll>())
                {
                    int layerMask = 1 << 10;

                    if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMask))
                    {
                        Flag.transform.position = _gridHit.point;
                        if (hoverignObject == null)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (_mc != null) _mc = null;
                                SelectionList[0].GetComponent<FlagControll>().ExitFlag = _gridHit.point;
                                CameraState = STMethods.PlayerCameraState.Normal;
                            }
                        }
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        CameraState = STMethods.PlayerCameraState.Normal;
                    }
                }
                else
                {
                    CameraState = STMethods.PlayerCameraState.Normal;
                }
                break;
        }

        if (SelectionList.Count > 0)
        {
            for (int i = 0; i < SelectionList.Count;)
            {
                if (SelectionList[i].destroyed)
                {
                    if (SelectionList.Count > 1)
                    {
                        SelectionList.RemoveAt(i);
                    }
                    else if (SelectionList.Count == 1)
                    {
                        SelectionList.Clear();
                        return;
                    }
                    continue;
                }
                if (SelectionList[i] is ObjectUnderConstruction)
                {
                    if (SelectionList[i]._hs.curHull >= SelectionList[i]._hs.MaxHull)
                    {
                        if (SelectionList.Count > 1)
                        {
                            SelectionList.RemoveAt(i);
                        }
                        else if (SelectionList.Count == 1)
                        {
                            SelectionList.Clear();
                            return;
                        }
                    }
                }
                if (SelectionList[i] is Static)
                {
                    if (SelectionList[i].captain.Command == Captain.PlayerCommand.Deassamble)
                    {
                        if (SelectionList.Count > 1)
                        {
                            SelectionList.RemoveAt(i);
                        }
                        else if (SelectionList.Count == 1)
                        {
                            SelectionList.Clear();
                            return;
                        }
                    }
                }
                if (SelectionList[i] is Mobile)
                {
                    if (SelectionList[i].captain.Command == Captain.PlayerCommand.Deassamble && SelectionList[i].captain.ToStayPoint)
                    {
                        if (SelectionList.Count > 1)
                        {
                            SelectionList.RemoveAt(i);
                        }
                        else if (SelectionList.Count == 1)
                        {
                            SelectionList.Clear();
                            return;
                        }
                    }

                    if (SelectionList.Count > 1 && SelectionList[i].PlayerNum != PlayerNum)
                    {
                        SelectionList.RemoveAt(i);
                    }
                }
                SelectionList[i].isSelected = true;
                SelectionList[i].ShowSelectionEffect(0.2f);

                if (Input.GetKey(KeyCode.LeftShift) && !ShiftPatrolSetting)
                {
                    SelectionList[i].ObjectOrdersHigh += Input.GetAxis("Mouse Y")*3;
                    Mathf.Clamp(SelectionList[i].ObjectOrdersHigh, -100, 100);
                }
                i++;
            }

            HighGrid.SetActive(true);
            HighGrid.transform.position = new Vector3(0, SelectionList[0].ObjectOrdersHigh, 0);
            
            int layerMask = 1 << 10;
            
            if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMask))
            {
                HighLine.gameObject.SetActive(true);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    HighLine.transform.position = new Vector3(HighLine.transform.position.x, _gridHit.point.y, HighLine.transform.position.z);
                }
                else
                {
                    HighLine.transform.position = _gridHit.point;
                }
            }

            if (SelectionList[0].GetComponent<FlagControll>())
            {
                if (CameraState != STMethods.PlayerCameraState.FlagPlacing)
                {
                    Flag.transform.position = SelectionList[0].GetComponent<FlagControll>().ExitFlag;
                }

                Flag.SetActive(true);
            }
            else
            {
                Flag.SetActive(false);
            }

            for (int i = 0; i < SelectionList.Count; i++)
            {
                for (int j = 0; j < Fleets.fleets.Length; j++)
                {
                    if (Fleets.fleets[j] != null && Fleets.fleets[j].Count > 0)
                    {
                        if (Fleets.fleets[j].Any(x => x == SelectionList[i]))
                        {
                            FleetObjects[i].Circle.color = GameManager.instance.Players[SelectionList[i].PlayerNum - 1].PlayerColor;
                            if (j == 0)
                            {
                                FleetObjects[i].Text.text = GameManager.instance.Players[SelectionList[i].PlayerNum - 1].GroupsNames[9];
                                FleetObjects[i].FleetNumber.text = 0.ToString();
                            }
                            else
                            {
                                FleetObjects[i].Text.text = GameManager.instance.Players[SelectionList[i].PlayerNum - 1].GroupsNames[j-1];
                                FleetObjects[i].FleetNumber.text = j.ToString();
                            }
                            FleetObjects[i].transform.position = SelectionList[i].transform.position;
                            if (Vector3.Distance(PlayerCameras.MainCamera.transform.position, SelectionList[i].transform.position) <
                                SelectionList[i].ObjectRadius * 6)
                            {
                                FleetObjects[i].transform.localScale = new Vector3(SelectionList[i].ObjectRadius,
                                    SelectionList[i].ObjectRadius, SelectionList[i].ObjectRadius);
                            }
                            else
                            {
                                float distance = Vector3.Distance(PlayerCameras.MainCamera.transform.position,
                                    SelectionList[i].transform.position) / 6;
                                FleetObjects[i].transform.localScale = new Vector3(distance, distance, distance);
                            }

                            Vector3 relativePos = PlayerCameras.MainCamera.gameObject.transform.forward * -1;

                            // the second argument, upwards, defaults to Vector3.up
                            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.down);
                            FleetObjects[i].transform.rotation = rotation;
                            
                            FleetObjects[i].gameObject.SetActive(true);
                            break;
                        }
                    }
                    if(j == 9) FleetObjects[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < FleetObjects.Count; i++)
            {
                if (i > SelectionList.Count-1)
                {
                    FleetObjects[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            HighGrid.SetActive(false);
            HighLine.gameObject.SetActive(false);
            if (_mc != null) _mc = null;
            Flag.SetActive(false);
            
            foreach (FleetObjectScript obj in FleetObjects)
            {
                obj.gameObject.SetActive(false);
            }

            if(CameraState == STMethods.PlayerCameraState.OrderSetting || CameraState == STMethods.PlayerCameraState.PatrolSetting) CameraState = STMethods.PlayerCameraState.Normal;
        }
        for (int j = 0; j < Fleets.fleets.Length; j++)
        {
            if (Fleets.fleets[j] != null && Fleets.fleets[j].Count > 0)
            {
                for (int i = 0; i < Fleets.fleets[j].Count;)
                {
                    if (Fleets.fleets[j][i].destroyed || Fleets.fleets[j][i].PlayerNum != PlayerNum)
                    {
                        Fleets.fleets[j].RemoveAt(i);
                        continue;
                    }
                    i++;
                }
            }
        }
    }

    public Camera getWorkingCamera()
    {
        Camera curMainCamera;

        if (globalInterface.globalStatus == GlobalInterfaceEventSystem.globalInterfaceStatus.GlobalMinimap)
        {
            curMainCamera = PlayerCameras.GlobalMinimapCamera;
        }
        else
        {
            curMainCamera = PlayerCameras.MainCamera;
        }
        
        return curMainCamera;
    } 

    private void SelectPro()
    {
        isSelect = true;
        _selXOld = Input.mousePosition.x;
        _selYOld = Input.mousePosition.y;

        int layerMask = 1 << 9;

        _startPoint = Input.mousePosition;

        if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMask))
        {
            if (_gridHit.transform.root.GetComponent<GlobalInterfaceEventSystem>()) return;
            if (_gridHit.transform.GetComponent<SelectableObject>())
            {
                SelectableObject seltar = _gridHit.transform.GetComponent<SelectableObject>();

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

                            if (!SelectionList.Any(x => x == seltar) && SelectionList.Count < 16)
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

                if (_3DVision && Input.GetKey(KeyCode.LeftAlt))
                {
                    _3DVisionTarget = seltar;
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

    
    
    void ActivatePatrol()
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
                            if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
                            obj.captain.EnterCommand(_pc);
                        }

                        CameraState = STMethods.PlayerCameraState.Normal;
                        SetNewCommand = Captain.PlayerCommand.None;
                    }
                }
            }
            else
            {
                CameraState = STMethods.PlayerCameraState.Normal;
                SetNewCommand = Captain.PlayerCommand.None;
            }
        }
        else
        {
            CameraState = STMethods.PlayerCameraState.Normal;
            SetNewCommand = Captain.PlayerCommand.None;
        }

        ShiftPatrolSetting = false;
    }

    private void OrderSet()
    {
        int layerMask = 1 << 9;

        if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMask))
        {
            if (!ShiftPatrolSetting)
            {
                if (_gridHit.transform.GetComponent<SelectableObject>())
                {
                    SelectableObject tar = _gridHit.transform.GetComponent<SelectableObject>();
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
                                    if(SelectionList[0].captain.Pilot != null) SelectionList[0].captain.Pilot._navigation.positions.Clear();
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
                                        if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
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
                                                if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
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
                                            if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
                                            obj.captain.EnterCommand(_fc);  
                                        }
                                    }
                                }
                            }
                        }
                        if (tar.GetComponent<ObjectUnderConstruction>())
                        {
                            if (tar.PlayerNum == PlayerNum)
                            {
                                if (SelectionList.Count > 0)
                                {
                                    if (SelectionList[0].PlayerNum == PlayerNum)
                                    {
                                        _buildingCommand = new BuildingCommand();
                                        _buildingCommand.command = "Build";
                                        _buildingCommand.proTarget = tar.gameObject;
                                        _buildingCommand.target = tar.GetComponent<ObjectUnderConstruction>().Contract;
                                    
                                        foreach (SelectableObject obj in SelectionList)
                                        {
                                            if (obj.captain.Builder != null) 
                                            {
                                                if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
                                                obj.captain.EnterCommand(_buildingCommand);
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
                                    if (_mc.ResTar.curDilithium <= 0) _mc.Type = STMethods.ResourcesType.Titanium;
                                    if (_mc.ResTar.curTitanium <= 0) _mc.Type = STMethods.ResourcesType.Dilithium;
                                    
                                    foreach (SelectableObject obj in SelectionList)
                                    {
                                        if (obj.GetComponent<MiningController>())
                                        {
                                            if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
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

        float X;
        float Y;
        X = Screen.width / 100;
        Y = Screen.height / 100;
        
        if (Input.mousePosition.y < Y * 25 & Input.mousePosition.y > Y * 0 & Input.mousePosition.x < X * 14 &
            Input.mousePosition.x > X * 0)
        {
            _gridRay = _miniMapComponent.itsMinimapCamera.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            if (globalInterface.globalStatus == GlobalInterfaceEventSystem.globalInterfaceStatus.GlobalMinimap)
            {
                _gridRay = PlayerCameras.GlobalMinimapCamera.ScreenPointToRay(Input.mousePosition);    
            }
            else
            {
                _gridRay = PlayerCameras.MainCamera.ScreenPointToRay(Input.mousePosition);    
            }
        }

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

                        if (!ShiftPatrolSetting)
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
                                    Vector3 rotateVector = _gridHit.point + (rotation * new Vector3(pos[i].x * maxRad, pos[i].y * maxRad, pos[i].z * maxRad));
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
                                    if(_s.captain.Pilot != null) _s.captain.Pilot._navigation.positions.Clear();
                                    _s.captain.EnterCommand(com);
                                }

                                return;
                            }

                            if (mlist.Count == 1)
                            {
                                mlist[0].ResetTimelyFleet(null);
                                MoveCommand com = new MoveCommand();
                                com.command = "Move";
                                com.targetVec = new List<Vector3> {_gridHit.point};
                                if (Input.GetKey(KeyCode.LeftControl))
                                {
                                    com.Warp = true;
                                }
                                if(mlist[0].captain.Pilot != null) mlist[0].captain.Pilot._navigation.positions.Clear();
                                mlist[0].captain.EnterCommand(com);
                                Instantiate(MoveMark, _gridHit.point, Quaternion.Euler(Vector3.zero));
                            }
                        }
                        else
                        {
                            _pc.targetVec.Add(_gridHit.point);
                        }
                    }
                }
            }
        }
    }

    private void SetLeftMouseButtonOrder()
    {
        int layerMask = 1 << 9;

        if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMask))
        {
            if (_gridHit.transform.root.GetComponent<SelectableObject>())
            {
                SelectableObject seltar = _gridHit.transform.root.GetComponent<SelectableObject>();
                switch (SetNewCommand)
                {
                    case Captain.PlayerCommand.Attack:
                        AttackCommand _ac = new AttackCommand();
                        _ac.command = "Attack";
                        _ac.attackTarget = seltar;
                        if (SelectionList.Count == 1)
                        {
                            if (SelectionList[0].PlayerNum != PlayerNum)
                            {
                                CameraState = STMethods.PlayerCameraState.Normal;
                                return;
                            }

                            if (SelectionList[0] is Mobile)
                            {
                                Mobile m = SelectionList[0] as Mobile;
                                m.ResetTimelyFleet(null);
                            }

                            if(SelectionList[0].captain.Pilot != null) SelectionList[0].captain.Pilot._navigation.positions.Clear();
                            SelectionList[0].captain.EnterCommand(_ac);
                            Instantiate(AttackMark, seltar.transform.position, Quaternion.Euler(Vector3.zero));
                            CameraState = STMethods.PlayerCameraState.Normal;
                            return;
                        }
                        else if (SelectionList.Count > 1)
                        {
                            if (SelectionList[0].PlayerNum != PlayerNum)
                            {
                                CameraState = STMethods.PlayerCameraState.Normal;
                                return;
                            }

                            List<Mobile> ml = SetTimelyFleet(SelectionList.ToList());
                            foreach (Mobile obj in ml)
                            {
                                if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
                                obj.captain.EnterCommand(_ac);
                            }

                            Instantiate(AttackMark, seltar.transform.position, Quaternion.Euler(Vector3.zero));
                        }

                        CameraState = STMethods.PlayerCameraState.Normal;
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
                        if (SelectionList.Count == 0 || SelectionList[0].PlayerNum != PlayerNum)
                        {
                            CameraState = STMethods.PlayerCameraState.Normal;
                            return;
                        }

                        List<Mobile> ml2 = SetTimelyFleet(SelectionList.ToList());
                        foreach (Mobile obj in ml2)
                        {
                            if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
                            obj.captain.EnterCommand(_gc);
                        }

                        Instantiate(GuardMark, seltar.transform.position, Quaternion.Euler(Vector3.zero));
                        CameraState = STMethods.PlayerCameraState.Normal;
                        break;
                    case Captain.PlayerCommand.SettingAbilityTarget:
                        SettingAbilityTargetCommand _satc = new SettingAbilityTargetCommand();
                        _satc.command = "SettingAbilityTargetCommand";
                        _satc.target = seltar;
                        _satc.ability = curSelectedAbility;
                        if (SelectionList.Count == 1 && SelectionList[0].PlayerNum == PlayerNum)
                        {
                            if(SelectionList[0].captain.Pilot != null) SelectionList[0].captain.Pilot._navigation.positions.Clear();
                            SelectionList[0].captain.EnterCommand(_satc);
                        }
                        CameraState = STMethods.PlayerCameraState.Normal;
                        break;
                }
            }
        }
    }

    Texture2D GetSelectionFrame(STMethods.Races race)
    {
        switch (race)
        {
            case STMethods.Races.Federation:
                return (Texture2D) DataLoader.Instance.ResourcesCache["SelectionFrames/Federation"];
            case STMethods.Races.Borg:
                return (Texture2D) DataLoader.Instance.ResourcesCache["SelectionFrames/Borg"];
            case STMethods.Races.Klingon:
                return (Texture2D) DataLoader.Instance.ResourcesCache["SelectionFrames/Klingons"];
        }

        return null;
    }

    void FindShipType(SelectableObject Type)
    {
        if(Type.stationSelectionType) return;
        foreach (SelectableObject obj in manager.SelectableObjects)
        {
            SelectableObject _ost = obj;

            Vector2 objpos = getWorkingCamera().WorldToScreenPoint(obj.transform.position);

            if ((objpos.x > 0 && objpos.x < Screen.width) || (objpos.x < 0 && objpos.x > Screen.width))
            {
                if ((objpos.y > 0 && objpos.y < Screen.height) || (objpos.y < 0 && objpos.y > Screen.height))
                {
                    if (!SelectionList.Any(x => x == obj) && SelectionList.Count < 16)
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
                Vector2 objpos = getWorkingCamera().WorldToScreenPoint(obj.transform.position);

                if (((objpos.x > _startPoint.x && objpos.x < _endPoint.x) ||
                     (objpos.x < _startPoint.x && objpos.x > _endPoint.x)) &&
                    ((objpos.y > _startPoint.y && objpos.y < _endPoint.y) ||
                     (objpos.y < _startPoint.y && objpos.y > _endPoint.y)))
                {
                    if (!SelectionList.Any(x => x == obj) && SelectionList.Count < 16)
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
        if (isSelect) return;
        int layerMask = 1 << 9;

        if (Physics.Raycast(_gridRay, out _gridHit, 10000.0f, layerMask))
        {
            if (_gridHit.transform.root.GetComponent<SelectableObject>())
            {
                if (hoverignObject != null && _gridHit.transform.root.GetComponent<SelectableObject>() != hoverignObject)
                {
                    hoverignObject.isHovering = false;
                    hoverignObject.ShowSelectionEffect(0);

                    hoverignObject = null;
                }
                else
                {
                    hoverignObject = _gridHit.transform.root.GetComponent<SelectableObject>();
                    hoverignObject.isHovering = true;
                    hoverignObject.ShowSelectionEffect(0.1f);
                    if (hoverignObject.PlayerNum == PlayerNum)
                    {
                        if (hoverignObject.captain == null)
                        {
                            SensorLine.gameObject.SetActive(false);
                            WeaponLine.gameObject.SetActive(false);
                            return;
                        }
                        if (hoverignObject.captain.Sensors != null)
                        {
                            SensorLine.transform.position = hoverignObject.transform.position;
                            SensorLine.radius = hoverignObject.effectManager.SensorRange();
                            SensorLine.gameObject.SetActive(true);
                        }
                        else
                        {
                            SensorLine.gameObject.SetActive(false);
                        }

                        if (hoverignObject.captain.Gunner != null)
                        {
                            WeaponLine.transform.position = hoverignObject.transform.position;
                            WeaponLine.radius = hoverignObject.WeaponRange;
                            WeaponLine.gameObject.SetActive(true);
                        }
                        else
                        {
                            WeaponLine.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            if (hoverignObject != null)
            {
                hoverignObject.isHovering = false;
                hoverignObject.ShowSelectionEffect(0);

                hoverignObject = null;
            }
            SensorLine.gameObject.SetActive(false);
            WeaponLine.gameObject.SetActive(false);
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
            CameraState = STMethods.PlayerCameraState.OrderSetting;
            SetNewCommand = Captain.PlayerCommand.Attack;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            CameraState = STMethods.PlayerCameraState.OrderSetting;
            SetNewCommand = Captain.PlayerCommand.Guard;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _pc = new PatrolCommand();
            _pc.command = "Patrol";
            CameraState = STMethods.PlayerCameraState.PatrolSetting;
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
                        if (obj.healthSystem)
                        {
                            if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
                            obj.captain.EnterCommand(_fc);
                        }   
                    }
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (SelectionList.Count > 0)
            {
                if (SelectionList[0].PlayerNum == PlayerNum)
                {
                    PlayerCommands _dc = new PlayerCommands();
                    _dc.command = "Deassembling";
                                    
                    foreach (SelectableObject obj in SelectionList)
                    {
                        if (obj.canBeDeassembled)
                        {
                            if(obj.captain.Pilot != null) obj.captain.Pilot._navigation.positions.Clear();
                            obj.captain.EnterCommand(_dc);
                        }   
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            if (!ShiftPatrolSetting)
            {
                _pc = new PatrolCommand();
                _pc.command = "Patrol";
                ShiftPatrolSetting = true;
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
                    foreach (SelectableObject ship in SelectionList)
                    {
                        foreach (List<SelectableObject> obj in Fleets.fleets)
                        {
                            if (obj != null && obj.Count > 0 && obj != Fleets.fleets[num] && obj.Any(x => x == ship)) obj.Remove(ship);
                        }    
                    }
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
        if(Fleet == null || Fleet.Count <= 0) return;
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
    
    public void _3DCameraSettings(bool state)
    {
        if (state)
        {
            if (SelectionList != null && SelectionList.Count > 0)
            {
                _3DVisionTarget = SelectionList[0];
            }
            else
            {
                List<Transform> allObjects = new List<Transform>();

                foreach (SelectableObject obj in GameManager.instance.SelectableObjects)
                {
                    allObjects.Add(obj.transform);
                }

                _3DVisionTarget = STMethods.NearestTransform(allObjects.ToArray(), transform).GetComponent<SelectableObject>();
            }

            _3DVision = true;
        }
        else
        {
            _3DVisionTarget = null;
            _3DVision = false;
        }
    }

    public void OnDilithiumMineButtonDown()
    {
        if (SelectionList != null && SelectionList.Count > 0 && SelectionList[0].PlayerNum == PlayerNum)
        {
            foreach (SelectableObject _selectableObject in SelectionList)
            {
                if (_selectableObject.captain != null && _selectableObject.captain.Miner != null)
                {
                    _selectableObject.captain.Miner.curResourcesType = STMethods.ResourcesType.Dilithium;
                }
            }
        }
    }
    public void OnTitaniumMineButtonDown()
    {
        if (SelectionList != null && SelectionList.Count > 0 && SelectionList[0].PlayerNum == PlayerNum)
        {
            foreach (SelectableObject _selectableObject in SelectionList)
            {
                if (_selectableObject.captain != null && _selectableObject.captain.Miner != null)
                {
                    _selectableObject.captain.Miner.curResourcesType = STMethods.ResourcesType.Titanium;
                }
            }
        }
    }
}