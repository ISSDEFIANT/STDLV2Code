using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GlobalInterfaceEventSystem : MonoBehaviour
{
    #region Enums
    public enum globalInterfaceStatus
    {
        Clear,
        Normal,
        Science,
        GlobalMinimap,
        Pause
    }
    /// <summary> Активная панель. </summary>
    public enum interfaceState
    {
        MSD,
        Orders,
        Special,
        Build,
        Trade
    }

    public enum StationLists
    {
        none,
        S1,
        S2,
        S3
    }
    #endregion

    #region Main interface parts
    public globalInterfaceStatus globalStatus = globalInterfaceStatus.Normal;
    public GameObject InterfaceObj;
    public GameObject ScienceObj;
    public GameObject PauseObj;

    public ScienceSubModule scienceSubModule;

    public Button MSDButton;
    public Button OrdersButton;
    public Button SpecialsButton;
    public Button BuildButton;
    public Button TradeButton;

    public interfaceState state;
    public StationLists CurCategory;
    
    public int PlayerNum;

    public PlayerCameraControll player;
    #endregion

    #region Fleets panel
    public GameObject FleetsNamesPanel;
    public Button[] FleetsSelectionButtons;
    public Text[] FleetsSelectionButtonTexts;
    public InputField[] FleetsInputFields;
    public Button[] FleetsClearButtons;
    #endregion

    #region Objects panels
    public GameObject FleetsMSD;
    public GameObject FleetsOrders;
    
    public GameObject OneObjectMSDPanel;
    public GameObject[] OneMSDBars;
    public GameObject OneSubSystemsPanel;
    public GameObject OneResourcesPanel;
    public GameObject OneMinerPanel;
    public GameObject OneBuilderPanel;
    
    public GameObject OneObjectOrdersPanel;
    public GameObject OneObjectSpecialsPanel;
    public GameObject OneObjectBuildingPanel;
    
    public GameObject OneObjectStationPanel;
    #endregion
    
    public GameObject CategorySelectionPlane;
    public bool BuilderControllerChecked;
    public BuilderConstructionController BuilderController;
    public DockingConstructionTypeController DockingBuilderController;
    public GameObject NoneObject;
    public List<ConstructionContract> NoneCategory = new List<ConstructionContract>();
    public GameObject S1Object;
    public List<ConstructionContract> Category1 = new List<ConstructionContract>();
    public GameObject S2Object;
    public List<ConstructionContract> Category2 = new List<ConstructionContract>();
    public GameObject S3Object;
    public List<ConstructionContract> Category3 = new List<ConstructionContract>();

    public List<Skill> ObjectSkills;
    
    public List<ElementInterface> OneObjectInterface;
    public FleetSlot[] FleetSlots;
    public List<ElementInterface> FleetHoverElements;

    private SelectableObject oldSelectedObject;

    public Toggle GridToggle;

    public Transform OneObjectElements;

    private float ClickCooldown = 0.3f;
    private int ClickCound = 0;
    private int CurFleet;

    public Button RecrewButton;
    public Button LevelUpButton;

    public GameObject MinimapPanel;

    private UnitsMarksController _umc;

    // Start is called before the first frame update
    void Start()
    {
        OneObjectInterface = new List<ElementInterface>();
        ElementInterface[] all = Resources.FindObjectsOfTypeAll<ElementInterface>();
        foreach (ElementInterface _a in all)
        {
            _a.mainInterfaceSystem = this;
            if(_a.transform.IsChildOf(OneObjectElements))
            {
                OneObjectInterface.Add(_a);
            }
        }

        for (int i = 0; i < FleetSlots.Length; i++)
        {
            foreach (ElementInterface _a in FleetSlots[i].ShipElement)
            {
                _a.mainInterfaceSystem = this;
            }
        }
        
        FindObjectOfType<GlobalMinimapController>().Init(this);

        _umc = FindObjectOfType<UnitsMarksController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (globalStatus)
        {
            case globalInterfaceStatus.Clear:
                InterfaceObj.SetActive(false);
                ScienceObj.SetActive(false);
                PauseObj.SetActive(false);
                MinimapPanel.SetActive(false);
                break;
            case globalInterfaceStatus.Normal:
                InterfaceObj.SetActive(true);
                ScienceObj.SetActive(false);
                PauseObj.SetActive(false);
                MinimapPanel.SetActive(false);
                break;
            case globalInterfaceStatus.Science:
                InterfaceObj.SetActive(false);
                ScienceObj.SetActive(true);
                PauseObj.SetActive(false);
                MinimapPanel.SetActive(false);
                break;
            case globalInterfaceStatus.Pause:
                InterfaceObj.SetActive(false);
                ScienceObj.SetActive(false);
                PauseObj.SetActive(true);
                MinimapPanel.SetActive(false);
                break;
            case globalInterfaceStatus.GlobalMinimap:
                InterfaceObj.SetActive(true);
                ScienceObj.SetActive(false);
                PauseObj.SetActive(false);
                MinimapPanel.SetActive(true);
                break;
        }
        if (player.SelectionList.Count == 0)
        {
            OneObjectMSDPanel.SetActive(false);
            OneObjectOrdersPanel.SetActive(false);
            OneObjectSpecialsPanel.SetActive(false);
            OneObjectBuildingPanel.SetActive(false);

            state = interfaceState.MSD;
        }
        if (player.SelectionList.Count == 1)
        {
            switch (state)
            {
                case interfaceState.MSD:
                    OneObjectMSDPanel.SetActive(true);
                    FleetsMSD.SetActive(false);
                    FleetsOrders.SetActive(false);
                    
                    OneObjectOrdersPanel.SetActive(false);
                    OneObjectSpecialsPanel.SetActive(false);
                    OneObjectBuildingPanel.SetActive(false);
                    
                    
                    CurCategory = StationLists.none;
                    break;
                case interfaceState.Orders:
                    OneObjectMSDPanel.SetActive(false);
                    FleetsMSD.SetActive(false);
                    FleetsOrders.SetActive(false);
                    OneObjectOrdersPanel.SetActive(true);
                    OneObjectSpecialsPanel.SetActive(false);
                    OneObjectBuildingPanel.SetActive(false);
                    
                    
                    CurCategory = StationLists.none;
                    break;
                case interfaceState.Special:
                    OneObjectMSDPanel.SetActive(false);
                    FleetsMSD.SetActive(false);
                    FleetsOrders.SetActive(false);
                    OneObjectOrdersPanel.SetActive(false);
                    OneObjectSpecialsPanel.SetActive(true);
                    OneObjectBuildingPanel.SetActive(false);
                    
                    
                    CurCategory = StationLists.none;
                    break;
                case interfaceState.Build:
                    OneObjectMSDPanel.SetActive(false);
                    FleetsMSD.SetActive(false);
                    FleetsOrders.SetActive(false);
                    OneObjectOrdersPanel.SetActive(false);
                    OneObjectSpecialsPanel.SetActive(false);
                    OneObjectBuildingPanel.SetActive(true);
                    
                    
                    break;
                case interfaceState.Trade:
                    OneObjectMSDPanel.SetActive(false);
                    FleetsMSD.SetActive(false);
                    FleetsOrders.SetActive(false);
                    OneObjectOrdersPanel.SetActive(false);
                    OneObjectSpecialsPanel.SetActive(false);
                    OneObjectBuildingPanel.SetActive(false);


                    CurCategory = StationLists.none;
                    break;
            }

            switch (CurCategory)
            {
                case StationLists.none:
                    if (DockingBuilderController == null)
                    {
                        CategorySelectionPlane.SetActive(true);
                        NoneObject.SetActive(false);
                    }
                    else
                    {
                        CategorySelectionPlane.SetActive(false);
                        NoneObject.SetActive(true);
                    }

                    S1Object.SetActive(false);
                    S2Object.SetActive(false);
                    S3Object.SetActive(false);
                    break;
                case StationLists.S1:
                    CategorySelectionPlane.SetActive(false);
                    NoneObject.SetActive(false);
                    S1Object.SetActive(true);
                    S2Object.SetActive(false);
                    S3Object.SetActive(false);
                    break;
                case StationLists.S2:
                    CategorySelectionPlane.SetActive(false);
                    NoneObject.SetActive(false);
                    S1Object.SetActive(false);
                    S2Object.SetActive(true);
                    S3Object.SetActive(false);
                    break;
                case StationLists.S3:
                    CategorySelectionPlane.SetActive(false);
                    NoneObject.SetActive(false);
                    S1Object.SetActive(false);
                    S2Object.SetActive(false);
                    S3Object.SetActive(true);
                    break;
            }

            foreach (ElementInterface elements in OneObjectInterface)
            {
                elements.target = player.SelectionList[0];
            }

            if (!BuilderControllerChecked)
            {
                UpdateSkills(player.SelectionList[0]);
                if (player.SelectionList[0].GetComponent<BuilderConstructionController>())
                {
                    BuilderController = player.SelectionList[0].GetComponent<BuilderConstructionController>();
                    UpdateCategories(BuilderController);
                }
                else
                {
                    BuilderController = null;
                }
                if (player.SelectionList[0].GetComponent<DockingConstructionTypeController>())
                {
                    DockingBuilderController = player.SelectionList[0].GetComponent<DockingConstructionTypeController>();
                    NoneCategory = DockingBuilderController.AbleToConstruct.ToList();
                }
                else
                {
                    DockingBuilderController = null;
                }
                BuilderControllerChecked = true;
            }

            if (player.SelectionList[0].captain != null)
            {
                if (player.SelectionList[0].captain.Builder != null && player.SelectionList[0].captain.curCommandInfo is BuildingCommand &&
                         (player.SelectionList[0].captain.curCommandInfo as BuildingCommand).proTarget != null)
                {
                    OneMinerPanel.SetActive(false);
                    OneBuilderPanel.SetActive(true);
                }
                else if (player.SelectionList[0].captain.ShipBuilder != null &&
                         player.SelectionList[0].captain.ShipBuilder.ShipsInList != null &&
                         player.SelectionList[0].captain.ShipBuilder.ShipsInList.Count > 0)
                {
                    OneMinerPanel.SetActive(false);
                    OneBuilderPanel.SetActive(true);
                }
                else if (player.SelectionList[0].captain.LevelUpdater != null &&
                         player.SelectionList[0].captain.LevelUpdater.levels != null &&
                         player.SelectionList[0].captain.LevelUpdater.levels.Count > 0 &&
                         player.SelectionList[0].captain.LevelUpdater.Updating)
                {
                    OneMinerPanel.SetActive(false);
                    OneBuilderPanel.SetActive(true);
                }
                else
                {
                    OneBuilderPanel.SetActive(false);
                    if (player.SelectionList[0].captain != null &&
                        player.SelectionList[0].captain.Miner != null)
                    {
                        OneMinerPanel.SetActive(true);
                    }
                    else
                    {
                        OneMinerPanel.SetActive(false);
                    }
                }
            }
            
            if (player.SelectionList[0] is ObjectUnderConstruction)
            {
                OneMinerPanel.SetActive(false);
                OneBuilderPanel.SetActive(true);
            }

            if (player.SelectionList[0] is Static && player.SelectionList[0].PlayerNum == PlayerNum)
            {
                Static station = player.SelectionList[0] as Static;
                if (station.healthSystem)
                {
                    RecrewButton.gameObject.SetActive(true);
                    RecrewButton.interactable = !station._hs.isAttacked;
                }
                else
                {
                    RecrewButton.gameObject.SetActive(false);
                }

                if (player.SelectionList[0].captain != null && player.SelectionList[0].captain.LevelUpdater != null)
                {
                    LevelUpButton.gameObject.SetActive(true);
                    if (player.SelectionList[0].captain.LevelUpdater.levels.Count > 0)
                    {
                        LevelUpButton.interactable = player.SelectionList[0].captain.LevelUpdater.levels[0].CanBeBuild(PlayerNum);
                        LevelUpButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        LevelUpButton.gameObject.SetActive(false);
                    }
                }
                else
                {
                    LevelUpButton.gameObject.SetActive(false);
                }
                
                OneObjectStationPanel.SetActive(true);
            }
            else
            {
                OneObjectStationPanel.SetActive(false);
            }
        }
        if (player.SelectionList.Count > 1)
        {
            switch (state)
            {
                case interfaceState.MSD:
                    OneObjectMSDPanel.SetActive(false);
                    FleetsMSD.SetActive(true);
                    FleetsOrders.SetActive(false);
                    OneObjectOrdersPanel.SetActive(false);
                    OneObjectSpecialsPanel.SetActive(false);
                    OneObjectBuildingPanel.SetActive(false);

                    CurCategory = StationLists.none;

                    if (player.hoverignObject != null)
                    {
                        foreach (ElementInterface obj in FleetHoverElements)
                        {
                            obj.target = player.hoverignObject;
                        }
                        if(!FleetHoverElements[0].transform.parent.gameObject.activeSelf) FleetHoverElements[0].transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        if(FleetHoverElements[0].transform.parent.gameObject.activeSelf) FleetHoverElements[0].transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case interfaceState.Orders:
                    OneObjectMSDPanel.SetActive(false);
                    FleetsMSD.SetActive(false);
                    FleetsOrders.SetActive(true);
                    OneObjectOrdersPanel.SetActive(false);
                    OneObjectSpecialsPanel.SetActive(false);
                    OneObjectBuildingPanel.SetActive(false);
                    
                    
                    CurCategory = StationLists.none;
                    break;
                case interfaceState.Special:
                    OneObjectMSDPanel.SetActive(false);
                    FleetsMSD.SetActive(false);
                    FleetsOrders.SetActive(false);
                    OneObjectOrdersPanel.SetActive(false);
                    OneObjectSpecialsPanel.SetActive(true);
                    OneObjectBuildingPanel.SetActive(false);
                    
                    
                    CurCategory = StationLists.none;
                    break;
            }

            for (int i = 0; i < player.SelectionList.Count; i++)
            {
                FleetSlots[i].SetVessel(player.SelectionList[i]);
                if(!FleetSlots[i].ShipElement[0].transform.parent.gameObject.activeSelf)FleetSlots[i].ShipElement[0].transform.parent.gameObject.SetActive(true);
            }
            for (int i = player.SelectionList.Count; i < FleetSlots.Length; i++)
            {
                if(FleetSlots[i].ShipElement[0].transform.parent.gameObject.activeSelf)FleetSlots[i].ShipElement[0].transform.parent.gameObject.SetActive(false);
            }

            OneObjectStationPanel.SetActive(false);
        }
        else
        {
            FleetsMSD.SetActive(false);
            FleetsOrders.SetActive(false);
        }

        if (player.SelectionList.Count > 0)
        {
            if (oldSelectedObject != player.SelectionList[0])
            {
                state = interfaceState.MSD;
                oldSelectedObject = player.SelectionList[0];
                BuilderControllerChecked = false;

                if (player.SelectionList[0].healthSystem)
                {
                    foreach (GameObject obj in OneMSDBars)
                    {
                        if (!obj.activeSelf) obj.SetActive(true);
                    }
                    if (player.SelectionList[0]._hs.SubSystems != null && player.SelectionList[0]._hs.SubSystems.Length > 0)
                    {
                        OneSubSystemsPanel.SetActive(true);
                    }
                    else
                    {
                        OneSubSystemsPanel.SetActive(false);
                    }
                }
                else
                {
                    foreach (GameObject obj in OneMSDBars)
                    {
                        if (obj.activeSelf) obj.SetActive(false);
                    }
                    OneSubSystemsPanel.SetActive(false);
                }
                
                if (!(player.SelectionList[0] is ResourceSource) && !(player.SelectionList[0] is SystemCentralObject))
                {
                    MSDButton.gameObject.SetActive(true);
                    OrdersButton.gameObject.SetActive(true);
                    SpecialsButton.gameObject.SetActive(true);
                    OneResourcesPanel.SetActive(false);
                }
                else
                {
                    MSDButton.gameObject.SetActive(false);
                    OrdersButton.gameObject.SetActive(false);
                    SpecialsButton.gameObject.SetActive(false);
                    BuildButton.gameObject.SetActive(false);
                    TradeButton.gameObject.SetActive(false);
                    OneMinerPanel.SetActive(false);
                    OneBuilderPanel.SetActive(false);
                    if (player.SelectionList[0] is ResourceSource)
                    {
                        OneResourcesPanel.SetActive(true);
                    }
                    else
                    {
                        OneResourcesPanel.SetActive(false);
                    }
                }


                if (player.SelectionList[0].PlayerNum != PlayerNum)
                {
                    OrdersButton.interactable = false;
                    SpecialsButton.interactable = false;
                    BuildButton.gameObject.SetActive(false);
                    TradeButton.gameObject.SetActive(false);
                }
                else
                { 
                    OrdersButton.interactable = true;
                    SpecialsButton.interactable = true;
                    if (player.SelectionList.Count > 1)
                    {
                        BuildButton.gameObject.SetActive(false);
                        TradeButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (player.SelectionList[0].GetComponent<BuilderConstructionController>() || player.SelectionList[0].GetComponent<DockingConstructionTypeController>())
                        {
                            BuildButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            BuildButton.gameObject.SetActive(false);
                        }
                        TradeButton.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            oldSelectedObject = null;
            MSDButton.gameObject.SetActive(false);
            OrdersButton.gameObject.SetActive(false);
            SpecialsButton.gameObject.SetActive(false);
            BuildButton.gameObject.SetActive(false);
            TradeButton.gameObject.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            if (player.Fleets.fleets[i] != null)
            {
                if (player.Fleets.fleets[i].Count > 0)
                {
                    FleetsClearButtons[i].interactable = true;
                }
                else
                {
                    FleetsClearButtons[i].interactable = false;
                }
            }
        }

        if (ClickCound > 0)
        {
            if (ClickCooldown > 0)
            {
                ClickCooldown -= Time.deltaTime;
            }
            else
            {
                ClickCound = 0;
                ClickCooldown = 0.3f;
            }
        }
    }

    public void OnFleetButtonDown(int i)
    {
        if (CurFleet != i)
        {
            ClickCound = 1;
            CurFleet = i;
        }
        else
        {
            ClickCound++;
        }

        if (ClickCound == 1)
        {
            if (player.Fleets.fleets[i] != null && player.Fleets.fleets[i].Count > 0)
            {
                player.SelectionList = player.Fleets.fleets[i].ToList();
            }
            foreach (InputField _if in FleetsInputFields)
            {
                _if.gameObject.SetActive(false);
            }
            foreach (Button _b in FleetsSelectionButtons)
            {
                _b.gameObject.SetActive(true);
            }
        }
        if (ClickCound == 2)
        {
            FleetsSelectionButtons[i].gameObject.SetActive(false);
            FleetsInputFields[i].gameObject.SetActive(true);
        }
    }
    public void OnFleetClearButtonDown(int i)
    {
        if (player.Fleets.fleets[i] != null && player.Fleets.fleets[i].Count > 0)
        {
            player.Fleets.fleets[i].Clear();
        }
    }

    public void OnMSDButton()
    {
        state = interfaceState.MSD;
    }
    public void OnOrderButton()
    {
        state = interfaceState.Orders;
    }
    public void OnSpecialButton()
    {
        state = interfaceState.Special;
    }
    public void OnBuildButton()
    {
        state = interfaceState.Build;
    }

    public void OnAttackButton()
    {
        player.CameraState = STMethods.PlayerCameraState.OrderSetting;
        player.SetNewCommand = Captain.PlayerCommand.Attack;
    }
    public void OnGuardButton()
    {
        player.CameraState = STMethods.PlayerCameraState.OrderSetting;
        player.SetNewCommand = Captain.PlayerCommand.Guard;
    }
    public void OnSelfDestructButton()
    {
        foreach (SelectableObject obj in player.SelectionList)
        {
            if (obj.PlayerNum == PlayerNum && obj.healthSystem)
            {
                obj._hs.SelfDestructActive = !obj._hs.SelfDestructActive;
            }
        }
    }
    public void OnPatrolButton()
    {
        player._pc = new PatrolCommand();
        player._pc.command = "Patrol";
        player.CameraState = STMethods.PlayerCameraState.PatrolSetting;
        player.SetNewCommand = Captain.PlayerCommand.Patrol;
    }

    public void OnFullStopButton()
    {
        foreach (SelectableObject obj in player.SelectionList)
        {
            if (obj.PlayerNum == PlayerNum && obj.captain != null)
            {
                PlayerCommands stopCommand = new PlayerCommands();
                stopCommand.command = "FullStop";
                obj.captain.EnterCommand(stopCommand);
            }
        }
    }

    public void OnFixButton()
    {
        if (player.SelectionList[0].PlayerNum == PlayerNum)
        {
            FixingCommand _fc = new FixingCommand();
            _fc.command = "Fixing";
            _fc.FixingPoint = null;

            foreach (SelectableObject obj in player.SelectionList)
            {
                if (obj.healthSystem)
                {
                    obj.captain.EnterCommand(_fc);
                }
            }
        }
    }

    public void OnDeassambleButton()
    {
        if (player.SelectionList[0].PlayerNum == PlayerNum)
        {
            PlayerCommands _dc = new PlayerCommands();
            _dc.command = "Deassembling";

            foreach (SelectableObject obj in player.SelectionList)
            {
                if (obj.canBeDeassembled)
                {
                    obj.captain.EnterCommand(_dc);
                }
            }
        }
    }

    public void OnRedAlertButton()
    {
        foreach (SelectableObject obj in player.SelectionList)
        {
            if (obj.PlayerNum == PlayerNum)
            {
                obj.Alerts = STMethods.Alerts.RedAlert;
            }
        }
    }
    public void OnYellowAlertButton()
    {
        foreach (SelectableObject obj in player.SelectionList)
        {
            if (obj.PlayerNum == PlayerNum)
            {
                obj.Alerts = STMethods.Alerts.YellowAlert;
            }
        }
    }
    public void OnGreenAlertButton()
    {
        foreach (SelectableObject obj in player.SelectionList)
        {
            if (obj.PlayerNum == PlayerNum)
            {
                obj.Alerts = STMethods.Alerts.GreenAlert;
            }
        }
    }

    public void SetAiming(string target)
    {
        STMethods.AttackType aiming = STMethods.AttackType.NormalAttack;
        switch (target)
        {
            case "Normal":
                aiming = STMethods.AttackType.NormalAttack;
                break;
            case "PrimaryWeapon":
                aiming = STMethods.AttackType.PrimaryWeaponSystemAttack;
                break;
            case "SecondaryWeapon":
                aiming = STMethods.AttackType.SecondaryWeaponSystemAttack;
                break;
            case "ImpulseEngine":
                aiming = STMethods.AttackType.ImpulseSystemAttack;
                break;
            case "WarpEngine":
                aiming = STMethods.AttackType.WarpEngingSystemAttack;
                break;
            case "WarpCore":
                aiming = STMethods.AttackType.WarpCoreAttack;
                break;
            case "LifeSupport":
                aiming = STMethods.AttackType.LifeSupportSystemAttack;
                break;
            case "Sensors":
                aiming = STMethods.AttackType.SensorsSystemAttack;
                break;
            case "TractorBeam":
                aiming = STMethods.AttackType.TractorBeamSystemAttack;
                break;
        }
        foreach (SelectableObject obj in player.SelectionList)
        {
            if (obj.PlayerNum == PlayerNum && obj.captain != null && obj.captain.Gunner != null)
            {
                obj.captain.Gunner.Aiming = aiming;
            }
        }
    }
    
    public void OnCaterotyBackButton()
    {
        CurCategory = StationLists.none;
    }
    public void OnCateroty1Button()
    {
        CurCategory = StationLists.S1;
    }
    public void OnCateroty2Button()
    {
        CurCategory = StationLists.S2;
    }
    public void OnCateroty3Button()
    {
        CurCategory = StationLists.S3;
    }

    public void OnFlagPlacingButton()
    {
        player.CameraState = STMethods.PlayerCameraState.FlagPlacing;
    }

    public void ToInterfacePlane()
    {
        globalStatus = globalInterfaceStatus.Normal;
        player.CameraState = STMethods.PlayerCameraState.Normal;
    }
    public void ToSciencePlane()
    {
        globalStatus = globalInterfaceStatus.Science;
        player.CameraState = STMethods.PlayerCameraState.Lock;
        scienceSubModule.UpdateSciList(true);
    }
    public void ToPausePlane()
    {
        globalStatus = globalInterfaceStatus.Pause;
    }

    public void OnGridButton(Toggle value)
    {
        if (player != null)
        {
            player.ShowGrid = value.isOn;
            player.PlayersSetGridVisibility = value.isOn;
        }
    }
    public void On3DVisionButton(Toggle value)
    {
        if (player != null)
        {
            bool playerSet = player.PlayersSetGridVisibility;
            player._3DCameraSettings(value.isOn);
            if (value.isOn)
            {
                player.ShowGrid = !value.isOn;
                GridToggle.isOn = !value.isOn;
                player.PlayersSetGridVisibility = playerSet;
            }
            else
            {
                if (player.PlayersSetGridVisibility)
                {
                    player.ShowGrid = true;
                    GridToggle.isOn = true;
                    player.PlayersSetGridVisibility = playerSet;
                }
                else
                {
                    player.ShowGrid = false;
                    GridToggle.isOn = false;
                    player.PlayersSetGridVisibility = playerSet;
                }
            }
        }
    }

    public void UpdateCategories(BuilderConstructionController tar)
    {
        Category1 = new List<ConstructionContract>();
        Category2 = new List<ConstructionContract>();
        Category3 = new List<ConstructionContract>();
        foreach (ConstructionContract con in tar.AbleToConstruct)
        {
            switch (con.ObjectCategory)
            {
                case 1:
                    Category1.Add(con);
                    break;
                case 2:
                    Category2.Add(con);
                    break;
                case 3:
                    Category3.Add(con);
                    break;
            }
        }
    }
    
    public void UpdateSkills(SelectableObject tar)
    {
        ObjectSkills = new List<Skill>();
        List<Skill> Battle = new List<Skill>();
        List<Skill> Research = new List<Skill>();
        List<Skill> Passive = new List<Skill>();
        foreach (Skill skills in tar.GetComponents<Skill>())
        {
            switch (skills.SkillType)
            {
                case Skill.skillType.Battle:
                    Battle.Add(skills);
                    break;
                case Skill.skillType.Research:
                    Research.Add(skills);
                    break;
                case Skill.skillType.Passive:
                    Passive.Add(skills);
                    break;
            }
        }
        ObjectSkills.AddRange(Battle);
        ObjectSkills.AddRange(Research);
        ObjectSkills.AddRange(Passive);
    }
    
    public void SetDilithiumMining()
    {
        player.OnDilithiumMineButtonDown();
    }
    public void SetTitaniumMining()
    {
        player.OnTitaniumMineButtonDown();
    }

    public void OnFleetsButton()
    {
        FleetsNamesPanel.SetActive(!FleetsNamesPanel.activeSelf);
    }

    public void Fleet1Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[0] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[1].gameObject.SetActive(false);
        FleetsSelectionButtons[1].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[1].text = value;
    }
    public void Fleet2Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[1] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[2].gameObject.SetActive(false);
        FleetsSelectionButtons[2].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[2].text = value;
    }
    public void Fleet3Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[2] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[3].gameObject.SetActive(false);
        FleetsSelectionButtons[3].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[3].text = value;
    }
    public void Fleet4Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[3] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[4].gameObject.SetActive(false);
        FleetsSelectionButtons[4].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[4].text = value;
    }
    public void Fleet5Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[4] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[5].gameObject.SetActive(false);
        FleetsSelectionButtons[5].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[5].text = value;
    }
    public void Fleet6Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[5] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[6].gameObject.SetActive(false);
        FleetsSelectionButtons[6].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[6].text = value;
    }
    public void Fleet7Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[6] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[7].gameObject.SetActive(false);
        FleetsSelectionButtons[7].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[7].text = value;
    }
    public void Fleet8Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[7] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[8].gameObject.SetActive(false);
        FleetsSelectionButtons[8].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[8].text = value;
    }
    public void Fleet9Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[8] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[9].gameObject.SetActive(false);
        FleetsSelectionButtons[9].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[9].text = value;
    }
    public void Fleet0Rename(string value)
    {
        GameManager.instance.Players[PlayerNum - 1].GroupsNames[9] = value;
        player.KeyboardFastButtonsDeactive = false;
        FleetsInputFields[0].gameObject.SetActive(false);
        FleetsSelectionButtons[0].gameObject.SetActive(true);
        FleetsSelectionButtonTexts[0].text = value;
    }

    public void BlockFastButtons()
    {
        player.KeyboardFastButtonsDeactive = true;
    }

    public void RecrewButtonDown()
    {
        if (player.SelectionList[0].healthSystem) (player.SelectionList[0] as Static)._hs.isRecrewing = !(player.SelectionList[0] as Static)._hs.isRecrewing;
    }

    public void StationUpdateButtonDown()
    {
        player.SelectionList[0].captain.LevelUpdater.UpdateObject();
    }

    public void OnGlobalMinimapButton()
    {
        if(globalStatus == globalInterfaceStatus.Normal) globalStatus = globalInterfaceStatus.GlobalMinimap;
    }
    public void OnLocalMinimapButton()
    {
        if(globalStatus == globalInterfaceStatus.GlobalMinimap) globalStatus = globalInterfaceStatus.Normal;
    }
    [Serializable]
    public class FleetSlot
    {
        public ElementInterface[] ShipElement;

        public void SetVessel(SelectableObject tar)
        {
            foreach (ElementInterface el in ShipElement)
            {
                el.target = tar;
            }
        }
    }

    #region Global minimap filters

    public void FilterNamesClick(Toggle value)
    {
        if (player != null)
        {
            _umc.NamesVisibile = value.isOn;
        }
    }
    
    
    
    public void FilterShipsClick(Toggle value)
    {
        if (player != null)
        {
            _umc.ShipsVisible = value.isOn;
        }
    }
    
    
    
    public void FilterStationsClick(Toggle value)
    {
        if (player != null)
        {
            _umc.StationsVisible = value.isOn;
        }
    }
    public void FilterOtherStationsClick(Toggle value)
    {
        if (player != null)
        {
            _umc.OtherStationsVisible = value.isOn;
        }
    }
    public void FilterObjectUnderConstructionClick(Toggle value)
    {
        if (player != null)
        {
            _umc.ObjectsInProgressVisible = value.isOn;
        }
    }
    public void FilterStarbaseClick(Toggle value)
    {
        if (player != null)
        {
            _umc.StarbaseVisible = value.isOn;
        }
    }
    public void FilterConstructionStationsClick(Toggle value)
    {
        if (player != null)
        {
            _umc.ConstructionStationsVisible = value.isOn;
        }
    }
    public void FilterDefenceStationClick(Toggle value)
    {
        if (player != null)
        {
            _umc.DefenceStationsVisible = value.isOn;
        }
    }
    public void FilterMiningStationsClick(Toggle value)
    {
        if (player != null)
        {
            _umc.MiningStationsVisible = value.isOn;
        }
    }
    public void FilterSciStationClick(Toggle value)
    {
        if (player != null)
        {
            _umc.ScienceStationsVisible = value.isOn;
        }
    }
    
    
    
    public void FilterUnknownClick(Toggle value)
    {
        if (player != null)
        {
            _umc.OtherStationsVisible = value.isOn;
        }
    }
    
    
    
    public void FilterStarClick(Toggle value)
    {
        if (player != null)
        {
            _umc.StarsVisible = value.isOn;
        }
    }
    public void FilterPlanetClick(Toggle value)
    {
        if (player != null)
        {
            _umc.PlanetsVisible = value.isOn;
        }
    }
    public void FilterAsteroidClick(Toggle value)
    {
        if (player != null)
        {
            _umc.AsteroidsVisible = value.isOn;
        }
    }

    #endregion
}