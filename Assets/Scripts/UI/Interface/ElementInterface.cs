using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ElementInterface : MonoBehaviour
{
	public enum ElementType
	{
		None,

		ShipIcon,
		ResourcesBar,
		ShildBar,
		HealthBar,
		EnergyBar,
		ShildCount,
		BlueprintPrymaryWeaponSystem,
		BlueprintSecondaryWeaponSystem,
		BlueprintImpulseSystem,
		BlueprintWarpEngineSystem,
		BlueprintWarpCoreSystem,
		BlueprintLifeSupportSystem,
		BlueprintSensoresSystem,
		BlueprintTractorBeamSystem,
		Blueprint,
		ShipName,
		ShipClass,
		Operator,
		ResourcesCount,
		CrewIcon,
		CrewCount,
		PrimaryWeaponIcon,
		PrimaryWeaponBar,
		SecondaryWeaponIcon,
		SecondaryWeaponBar,
		ImpulsIcon,
		ImpulsBar,
		WarpEngIcon,
		WarpEngBar,
		WarpCoreIcon,
		WarpCoreBar,
		LifeSupportIcon,
		LifeSupportBar,
		SensorIcon,
		SensorBar,
		TractorIcon,
		TractorBar,
		SystemAttackIcon,
		RedAlertButton,
		YellowAlertButton,
		GreenAlertButton,
		SelfDestructionButton,
		GuardOrder,
		SelfDestructionBar,
		FixButton,
		
		PlayerDilithium,
		PlayerTitanium,
		PlayerCrew,
		PlayerBiomatter,
		
		AttackOrderButton,
		PatrolOrderButton,
		SearchOrder,
		FaDOrder,
		
		DilithiumBar,
		MetalBar,
		DilithiumSourceText,
		MetalSourceText,
		DilithiumSourcePercent,
		MetalSourcePercent,
		ResourceSourcePercent,
		
		MinerTitaniumButton,
		MinerDilithiumButton,
		
		MinerLoadBar,
		MinerTextColor,
		
		BuilderProgressBar,
		BuilderPercent,
		BuilderTargetName,
		BuilderTargetIcon,
		
		Recrew,
		UpgradeButton
	}

	public ElementType Element;

	public Sprite HullAttack;
	public Sprite PrimaryAttack;
	public Sprite SecondaryAttack;
	public Sprite ImpulsAttack;
	public Sprite WarpEAttack;
	public Sprite WarpCAttack;
	public Sprite LifeAttack;
	public Sprite SensorEAttack;
	public Sprite TractorAttack;

	public Sprite OrderActive;
	public Sprite OrderDeActive;
	
	public Sprite DilithiumSprite;
	public Sprite TitaniumSprite;

	public Color DilithiumColor;
	public Color TitaniumColor;

	private Image TargetImage;
	private Text TargetText;
	private Toggle TargetToggle;
	private Button TargetButton;

	private float ColorChangeTime;
	private Color32 DeactiveSystemColor;

	public SelectableObject target;
	private SelectableObject oldTarget;
	private PrimaryWeaponSS _pw;
	private SecondaryWeaponSS _sw;
	private ImpulsEngineSS _ie;
	private WarpEngineSS _we;
	private WarpCoreSS _wc;
	private LifeSupportSS _ls;
	private SensorSS _s;
	private TractorBeamSS _tb;

	public GlobalInterfaceEventSystem mainInterfaceSystem;


	// Start is called before the first frame update
	void Start()
	{
		if (gameObject.GetComponent<Text>())
		{
			TargetText = gameObject.GetComponent<Text>();
		}

		if (gameObject.GetComponent<Image>())
		{
			TargetImage = gameObject.GetComponent<Image>();
		}

		if (gameObject.GetComponent<Toggle>())
		{
			TargetToggle = gameObject.GetComponent<Toggle>();
		}
		
		if (gameObject.GetComponent<Button>())
		{
			TargetButton = gameObject.GetComponent<Button>();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (target != null)
		{
			if (oldTarget != target)
			{
				_pw = null;
				_sw = null;
				_ie = null;
				_we = null;
				_wc = null;
				_ls = null;
				_s = null;
				_tb = null;
				oldTarget = target;
				
				if (target.healthSystem && target._hs.SubSystems != null && target._hs.SubSystems.Length > 0)
				{
					foreach (SubSystem _ss in target._hs.SubSystems)
					{
						if (_ss is PrimaryWeaponSS)
						{
							_pw = target._hs.SubSystems[0].gameObject.GetComponent<PrimaryWeaponSS>();
						}
						if (_ss is SecondaryWeaponSS)
						{
							_sw = target._hs.SubSystems[0].gameObject.GetComponent<SecondaryWeaponSS>();
						}
						if (_ss is ImpulsEngineSS)
						{
							_ie = target._hs.SubSystems[0].gameObject.GetComponent<ImpulsEngineSS>();
						}
						if (_ss is WarpEngineSS)
						{
							_we = target._hs.SubSystems[0].gameObject.GetComponent<WarpEngineSS>();
						}
						if (_ss is WarpCoreSS)
						{
							_wc = target._hs.SubSystems[0].gameObject.GetComponent<WarpCoreSS>();
						}
						if (_ss is LifeSupportSS)
						{
							_ls = target._hs.SubSystems[0].gameObject.GetComponent<LifeSupportSS>();
						}
						if (_ss is SensorSS)
						{
							_s = target._hs.SubSystems[0].gameObject.GetComponent<SensorSS>();
						}
						if (_ss is TractorBeamSS)
						{
							_tb = target._hs.SubSystems[0].gameObject.GetComponent<TractorBeamSS>();
						}
						if (_ss is PrimaryWeaponSS)
						{
							_pw = target._hs.SubSystems[0].gameObject.GetComponent<PrimaryWeaponSS>();
						}
						if (_ss is PrimaryWeaponSS)
						{
							_pw = target._hs.SubSystems[0].gameObject.GetComponent<PrimaryWeaponSS>();
						}
					}
				}
				else
				{
					return;
				}
			}
			if (ColorChangeTime <= 0)
			{
				if (DeactiveSystemColor.ToString() == new Color32(255, 174, 0, 255).ToString())
				{
					DeactiveSystemColor = new Color32(255, 255, 255, 255);
				}
				else
				{
					DeactiveSystemColor = new Color32(255, 174, 0, 255);
				}

				ColorChangeTime = 1;
			}
			else
			{
				ColorChangeTime -= Time.deltaTime;
			}

			UpdateInfo();
		}
		GlobalData();
	}

	void GlobalData()
	{
		switch (Element)
		{ 
			//Global
			case ElementType.PlayerCrew:
				TargetText.text = "Crew: " + (int)GameManager.instance.Players[mainInterfaceSystem.PlayerNum - 1].Crew;
				break;
			case ElementType.PlayerDilithium:
				TargetText.text = "Dilithium: " + (int)GameManager.instance.Players[mainInterfaceSystem.PlayerNum - 1].Dilithium;
				break;
			case ElementType.PlayerTitanium:
				TargetText.text = "Titanium: " + (int)GameManager.instance.Players[mainInterfaceSystem.PlayerNum - 1].Titanium;
				break;
			case ElementType.PlayerBiomatter:
				TargetText.text = "Biomatter: " + (int)GameManager.instance.Players[mainInterfaceSystem.PlayerNum - 1].Biomatter;
				break;			
		}
	}
	void UpdateInfo()
	{
		switch (Element)
		{
			//MSD Menu

			case ElementType.ShipIcon:
				TargetImage.sprite = target.ObjectIcon;
				break;
			case ElementType.ShipName:
				TargetText.text = target.ObjectName;
				break;
			case ElementType.ShipClass:
				TargetText.text = target.ObjectClass;
				break;

			case ElementType.Operator:
				if (target.PlayerNum == 0)
				{
					TargetText.text = "Operator: Neutral";
				}

				if (target.PlayerNum > 0)
				{
					TargetText.text = "Operator: " + GameManager.instance.Players[target.PlayerNum - 1].PlayerName;
				}

				break;

			case ElementType.BlueprintPrymaryWeaponSystem:
				if (_pw != null && target.ObjectBluePrint[0] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[0];
					SystemIconControl(_pw.SubSystemMaxHealth, _pw.SubSystemCurHealth, target.effectManager.PrimaryWeaponEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.BlueprintSecondaryWeaponSystem:
				if (_sw != null && target.ObjectBluePrint[1] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[1];
					SystemIconControl(_sw.SubSystemMaxHealth, _sw.SubSystemCurHealth, target.effectManager.SecondaryWeaponEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.BlueprintImpulseSystem:
				if (_ie != null && target.ObjectBluePrint[2] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[2];
					SystemIconControl(_ie.SubSystemMaxHealth, _ie.SubSystemCurHealth, target.effectManager.ImpulseEngineEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.BlueprintWarpEngineSystem:
				if (_we != null && target.ObjectBluePrint[3] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[3];
					SystemIconControl(_we.SubSystemMaxHealth, _we.SubSystemCurHealth, target.effectManager.WarpEngineEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.BlueprintWarpCoreSystem:
				if (_wc && target.ObjectBluePrint[4] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[4];
					SystemIconControl(_wc.SubSystemMaxHealth, _wc.SubSystemCurHealth, target.effectManager.WarpCoreEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.BlueprintLifeSupportSystem:
				if (_ls != null && target.ObjectBluePrint[5] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[5];
					SystemIconControl(_ls.SubSystemMaxHealth, _ls.SubSystemCurHealth, target.effectManager.LifeSupportEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.BlueprintSensoresSystem:
				if (_s != null && target.ObjectBluePrint[6] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[6];
					SystemIconControl(_s.SubSystemMaxHealth, _s.SubSystemCurHealth, target.effectManager.SensoresEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.BlueprintTractorBeamSystem:
				if (_tb != null && target.ObjectBluePrint[7] != null)
				{
					TargetImage.enabled = true;
					TargetImage.sprite = target.ObjectBluePrint[7];
					SystemIconControl(_tb.SubSystemMaxHealth, _tb.SubSystemCurHealth, target.effectManager.TractorBeamEffectActivity(), Color.white);
				}
				else
				{
					TargetImage.enabled = false;
				}
				break;
			case ElementType.Blueprint:
				if (target.healthSystem)
				{
					if (target.ObjectBluePrint[8] != null)
					{
						TargetImage.enabled = true;
						TargetImage.sprite = target.ObjectBluePrint[8];
						SystemIconControl(target._hs.MaxHull, target._hs.curHull, true, Color.white);
					}
					else
					{
						TargetImage.sprite = null;
						TargetImage.color = Color.clear;
					}
				}
				else
				{
					if (target.ObjectBluePrint[8] != null)
					{
						TargetImage.enabled = true;
						TargetImage.sprite = target.ObjectBluePrint[8];
						TargetImage.color = Color.white;
					}
					else
					{
						TargetImage.sprite = null;
						TargetImage.color = Color.clear;
					}
				}
				break;
			
			case ElementType.ShildBar:
				SHERBar(target._hs.ActiveShieldCurHealth(), target._hs.ActiveShieldMaxHealth());
				break;
			case ElementType.HealthBar:
				SHERBar(target._hs.curHull, target._hs.MaxHull);
				break;
			case ElementType.EnergyBar:
				SHERBar(target._hs.curEnergy, target._hs.maxEnergy);
				break;
			
			case ElementType.ShildCount:
				int activeShields = 0;
				string countConstruction = "";
				if (target._hs.Shilds != null && target._hs.Shilds.Length > 0)
				{
					foreach (SubSystem _s in target._hs.Shilds)
					{
						if (_s.SubSystemCurHealth > 0) activeShields++;
					}
				}

				if (activeShields > 1)
				{
					for (int i = 0; i < activeShields - 2; i++)
					{
						countConstruction += "|   ";
					}
					countConstruction += "|";
				}
				else
				{
					countConstruction = "";
				}

				TargetText.text = countConstruction;
				break;

			case ElementType.ResourcesBar:
				if (target.captain != null && target.captain.Miner != null)
				{
					SHERBar(target.captain.Miner.curResources, target.captain.Miner.MaxResources);
				}
				else
				{
					TargetImage.fillAmount = 0;
				}

				break;

			case ElementType.ResourcesCount:
				if (target.captain != null && target.captain.Miner != null)
				{
					switch (target.captain.Miner.curResourcesType)
					{
						case STMethods.ResourcesType.Titanium:
							TargetText.text = "Titanium " + (int) target.captain.Miner.curResources;
							break;
						case STMethods.ResourcesType.Dilithium:
							TargetText.text = "Dilithium " + (int) target.captain.Miner.curResources;
							break;
						case STMethods.ResourcesType.Biomatter:
							TargetText.text = "Biomatter " + (int) target.captain.Miner.curResources;
							break;
						case STMethods.ResourcesType.Crew:
							TargetText.text = "Crew " + (int) target.captain.Miner.curResources;
							break;
					}
				}
				else
				{
					TargetText.text = string.Empty;
				}

				break;

			case ElementType.CrewIcon:
				if (target.healthSystem)
				{
					if (target._hs.MaxCrew > 0)
					{
						if (target._hs.curCrew > target._hs.MaxCrew / 2)
						{
							TargetImage.color = Color.green;
						}

						if (target._hs.curCrew < target._hs.MaxCrew / 2 && target._hs.curCrew > target._hs.MaxCrew / 4)
						{
							TargetImage.color = Color.yellow;
						}

						if (target._hs.curCrew < target._hs.MaxCrew / 4 && target._hs.curCrew > 0)
						{
							TargetImage.color = Color.red;
						}

						if (target._hs.curCrew <= 0)
						{
							TargetImage.color = Color.black;
						}
					}
					else
					{
						TargetImage.color = Color.gray;
					}
				}
				break;
			case ElementType.CrewCount:
				if (target._hs.MaxCrew > 0)
				{
					TargetText.text = ": " + (int) target._hs.curCrew;
					TargetText.color = Color.white;
				}
				else
				{
					TargetText.text = ": 0";
					TargetText.color = Color.grey;
				}

				break;

			case ElementType.PrimaryWeaponBar:
				if (_pw != null)
				{
					SystemBarControl(_pw.SubSystemMaxHealth, _pw.SubSystemCurHealth, target.effectManager.PrimaryWeaponEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.PrimaryWeaponIcon:
				if (_pw != null)
				{
					SystemIconControl(_pw.SubSystemMaxHealth, _pw.SubSystemCurHealth, target.effectManager.PrimaryWeaponEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			case ElementType.SecondaryWeaponBar:
				if (_sw != null)
				{
					SystemBarControl(_sw.SubSystemMaxHealth, _sw.SubSystemCurHealth, target.effectManager.SecondaryWeaponEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.SecondaryWeaponIcon:
				if (_sw != null)
				{
					SystemIconControl(_sw.SubSystemMaxHealth, _sw.SubSystemCurHealth, target.effectManager.SecondaryWeaponEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			case ElementType.ImpulsBar:
				if (_ie != null)
				{
					SystemBarControl(_ie.SubSystemMaxHealth, _ie.SubSystemCurHealth, target.effectManager.ImpulseEngineEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.ImpulsIcon:
				if (_ie != null)
				{
					SystemIconControl(_ie.SubSystemMaxHealth, _ie.SubSystemCurHealth, target.effectManager.ImpulseEngineEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			case ElementType.WarpEngBar:
				if (_we != null)
				{
					SystemBarControl(_we.SubSystemMaxHealth, _we.SubSystemCurHealth, target.effectManager.WarpEngineEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.WarpEngIcon:
				if (_we != null)
				{
					SystemIconControl(_we.SubSystemMaxHealth, _we.SubSystemCurHealth, target.effectManager.WarpEngineEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			case ElementType.WarpCoreBar:
				if (_wc != null)
				{
					SystemBarControl(_wc.SubSystemMaxHealth, _wc.SubSystemCurHealth, target.effectManager.WarpCoreEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.WarpCoreIcon:
				if (_wc != null)
				{
					SystemIconControl(_wc.SubSystemMaxHealth, _wc.SubSystemCurHealth, target.effectManager.WarpCoreEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			case ElementType.LifeSupportBar:
				if (_ls != null)
				{
					SystemBarControl(_ls.SubSystemMaxHealth, _ls.SubSystemCurHealth, target.effectManager.LifeSupportEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.LifeSupportIcon:
				if (_ls != null)
				{
					SystemIconControl(_ls.SubSystemMaxHealth, _ls.SubSystemCurHealth, target.effectManager.LifeSupportEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			case ElementType.SensorBar:
				if (_s != null)
				{
					SystemBarControl(_s.SubSystemMaxHealth, _s.SubSystemCurHealth, target.effectManager.SensoresEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.SensorIcon:
				if (_s != null)
				{
					SystemIconControl(_s.SubSystemMaxHealth, _s.SubSystemCurHealth, target.effectManager.SensoresEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			case ElementType.TractorBar:
				if (_tb != null)
				{
					SystemBarControl(_tb.SubSystemMaxHealth, _tb.SubSystemCurHealth, target.effectManager.TractorBeamEffectActivity());
				}
				else
				{
					TargetImage.color = Color.grey;
					TargetImage.fillAmount = 1;
				}
				break;
			case ElementType.TractorIcon:
				if (_tb != null)
				{
					SystemIconControl(_tb.SubSystemMaxHealth, _tb.SubSystemCurHealth, target.effectManager.TractorBeamEffectActivity(), Color.green);
				}
				else
				{
					TargetImage.color = Color.grey;
				}
				break;

			//OrdersMenu

			case ElementType.SystemAttackIcon:
				if (target.captain.Gunner == null) return;
				switch (target.captain.Gunner.Aiming)
				{
					case STMethods.AttackType.NormalAttack:
						TargetImage.sprite = HullAttack;
						break;
					case STMethods.AttackType.PrimaryWeaponSystemAttack:
						TargetImage.sprite = PrimaryAttack;
						break;
					case STMethods.AttackType.SecondaryWeaponSystemAttack:
						TargetImage.sprite = SecondaryAttack;
						break;
					case STMethods.AttackType.ImpulseSystemAttack:
						TargetImage.sprite = ImpulsAttack;
						break;
					case STMethods.AttackType.WarpEngingSystemAttack:
						TargetImage.sprite = WarpEAttack;
						break;
					case STMethods.AttackType.WarpCoreAttack:
						TargetImage.sprite = WarpCAttack;
						break;
					case STMethods.AttackType.LifeSupportSystemAttack:
						TargetImage.sprite = LifeAttack;
						break;
					case STMethods.AttackType.SensorsSystemAttack:
						TargetImage.sprite = SensorEAttack;
						break;
					case STMethods.AttackType.TractorBeamSystemAttack:
						TargetImage.sprite = TractorAttack;
						break;
				}

				break;

			case ElementType.RedAlertButton:
				if (target.Alerts == STMethods.Alerts.RedAlert)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}

				break;
			case ElementType.YellowAlertButton:
				if (target.Alerts == STMethods.Alerts.YellowAlert)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}

				break;
			case ElementType.GreenAlertButton:
				if (target.Alerts == STMethods.Alerts.GreenAlert)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}

				break;

			case ElementType.SelfDestructionButton:
				if (target._hs.SelfDestructActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			case ElementType.SelfDestructionBar:
				if (target._hs.SelfDestructActive)
				{
					TargetImage.enabled = true;
					SHERBar(target._hs.SelfDestructTimer, 5, true);
				}
				else
				{
					TargetImage.enabled = false;
				}

				break;
			case ElementType.GuardOrder:
				if (target.captain.Gunner == null || target.captain.Pilot == null)
				{
					TargetButton.interactable = false;
				}
				else
				{
					TargetButton.interactable = true;
				}
				if (target.captain.curCommandInfo is GuardCommand)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}

				break;
			case ElementType.FixButton:
				if (target.captain.curCommandInfo is FixingCommand)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}

				break;
			
			
			
			case ElementType.AttackOrderButton:
				if (target.captain.Gunner != null)
				{
					TargetButton.interactable = true;
				}
				else
				{
					TargetButton.interactable = false;
				}

				break;
			
			case ElementType.PatrolOrderButton:
				if (target.captain.Pilot == null)
				{
					TargetButton.interactable = false;
				}
				else
				{
					TargetButton.interactable = true;
				}
				if (target.captain.curCommandInfo is PatrolCommand)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}

				break;
			
			case ElementType.SearchOrder:
				if (target.captain.Pilot == null)
				{
					TargetButton.interactable = false;
				}
				else
				{
					TargetButton.interactable = true;
				}
				/*if (target.captain.curCommandInfo is PatrolCommand)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}*/

				break;
			
			case ElementType.FaDOrder:
				if (target.captain.Pilot == null || target.captain.Gunner == null)
				{
					TargetButton.interactable = false;
				}
				else
				{
					TargetButton.interactable = true;
				}
				/*if (target.captain.curCommandInfo is PatrolCommand)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}*/

				break;
			
			case ElementType.DilithiumBar:
				SHERBar((target as ResourceSource).curDilithium, (target as ResourceSource).maxDilithium);
				break;
			case ElementType.MetalBar:
				SHERBar((target as ResourceSource).curTitanium, (target as ResourceSource).maxTitanium);
				break;
			case ElementType.DilithiumSourceText:
				TargetText.text = (int)(target as ResourceSource).curDilithium + " mt left";
				break;
			case ElementType.MetalSourceText:
				TargetText.text = (int)(target as ResourceSource).curTitanium + " mt left";
				break;
			case ElementType.DilithiumSourcePercent:
				if ((target as ResourceSource).maxDilithium <= 0)
				{
					TargetText.text = "Dilithium: none";
					break;
				}
				TargetText.text = "Dilithium: " + (int)((target as ResourceSource).curDilithium / (target as ResourceSource).maxDilithium * 100) + "%";
				break;
			case ElementType.MetalSourcePercent:
				if ((target as ResourceSource).maxTitanium <= 0)
				{
					TargetText.text = "Titanium: none";
					break;
				}
				TargetText.text = "Titanium: " + (int)((target as ResourceSource).curTitanium / (target as ResourceSource).maxTitanium * 100) + "%";
				break;
			case ElementType.ResourceSourcePercent:
				if (target.captain.Miner.curResources <= 0)
				{
					TargetText.text = "0";
					break;
				}
				TargetText.text = (int)(target.captain.Miner.curResources/target.captain.Miner.MaxResources * 100) + "%";
				break;
			case ElementType.MinerTitaniumButton:
				if (target.captain != null && target.captain.Miner != null)
				{
					if (target.captain.Miner.curResourcesType == STMethods.ResourcesType.Titanium)
					{
						TargetImage.sprite = OrderActive;
					}
					else
					{
						TargetImage.sprite = OrderDeActive;
					}
				}
				break;
			case ElementType.MinerDilithiumButton:
				if (target.captain != null && target.captain.Miner != null)
				{
					if (target.captain.Miner.curResourcesType == STMethods.ResourcesType.Dilithium)
					{
						TargetImage.sprite = OrderActive;
					}
					else
					{
						TargetImage.sprite = OrderDeActive;
					}
				}
				break;
			case ElementType.MinerLoadBar:
				if (target.captain != null && target.captain.Miner != null)
				{
					if (target.captain.Miner.curResourcesType == STMethods.ResourcesType.Dilithium)
					{
						TargetImage.sprite = DilithiumSprite;
					}
					if (target.captain.Miner.curResourcesType == STMethods.ResourcesType.Titanium)
					{
						TargetImage.sprite = TitaniumSprite;
					}
				}
				break;
			case ElementType.MinerTextColor:
				if (target.captain != null && target.captain.Miner != null)
				{
					if (target.captain.Miner.curResourcesType == STMethods.ResourcesType.Dilithium)
					{
						TargetText.color = DilithiumColor;
					}
					if (target.captain.Miner.curResourcesType == STMethods.ResourcesType.Titanium)
					{
						TargetText.color = TitaniumColor;
					}
				}
				break;
			
			case ElementType.BuilderProgressBar:
				if (target is ObjectUnderConstruction)
				{
					TargetImage.fillAmount = target._hs.curHull / target._hs.MaxHull;
					break;
				}
				
				if (target.captain != null && target.captain.Builder != null && target.captain.curCommandInfo != null && target.captain.curCommandInfo is BuildingCommand && (target.captain.curCommandInfo as BuildingCommand).proTarget != null)
				{
					SelectableObject obj = (target.captain.curCommandInfo as BuildingCommand).proTarget.GetComponent<SelectableObject>();
					TargetImage.fillAmount = obj._hs.curHull / obj._hs.MaxHull;
					break;
				}

				if (target.captain != null && target.captain.ShipBuilder != null && target.captain.ShipBuilder.ShipsInList != null && target.captain.ShipBuilder.ShipsInList.Count > 0)
				{
					TargetImage.fillAmount = 1 - target.captain.ShipBuilder.ShipsInList[0].ConstructionTime / target.captain.ShipBuilder.ShipsInList[0].MaxConstructionTime;
					break;
				}
				
				if (target.captain != null && target.captain.LevelUpdater != null && target.captain.LevelUpdater.levels != null && target.captain.LevelUpdater.levels.Count > 0)
				{
					TargetImage.fillAmount = 1 - target.captain.LevelUpdater.levels[0].curConstructionTime / target.captain.LevelUpdater.levels[0].ConstructionTime;
				}
				break;
			case ElementType.BuilderPercent:
				if (target is ObjectUnderConstruction)
				{
					TargetText.text = (int) (target._hs.curHull / target._hs.MaxHull * 100) + "%";
					break;
				}
				
				if (target.captain != null && target.captain.Builder != null && target.captain.curCommandInfo != null && target.captain.curCommandInfo is BuildingCommand && (target.captain.curCommandInfo as BuildingCommand).proTarget != null)
				{
					SelectableObject obj = (target.captain.curCommandInfo as BuildingCommand).proTarget.GetComponent<SelectableObject>();
					TargetText.text = (int) (obj._hs.curHull / obj._hs.MaxHull * 100) + "%";
					break;
				}
				
				if (target.captain != null && target.captain.ShipBuilder != null && target.captain.ShipBuilder.ShipsInList != null && target.captain.ShipBuilder.ShipsInList.Count > 0)
				{
					TargetText.text = (int) (100 - (target.captain.ShipBuilder.ShipsInList[0].ConstructionTime / target.captain.ShipBuilder.ShipsInList[0].MaxConstructionTime * 100)) + "%";
					break;
				}
				
				if (target.captain != null && target.captain.LevelUpdater != null && target.captain.LevelUpdater.levels != null && target.captain.LevelUpdater.levels.Count > 0)
				{
					TargetText.text = (int) (100 - (target.captain.LevelUpdater.levels[0].curConstructionTime / target.captain.LevelUpdater.levels[0].ConstructionTime * 100)) + "%";
					break;
				}
				break;
			case ElementType.BuilderTargetName:
				if (target is ObjectUnderConstruction)
				{
					TargetText.text = target.ObjectClass;
					break;
				}
				
				if (target.captain != null && target.captain.Builder != null && target.captain.curCommandInfo != null && target.captain.curCommandInfo is BuildingCommand && (target.captain.curCommandInfo as BuildingCommand).proTarget != null)
				{
					SelectableObject obj = (target.captain.curCommandInfo as BuildingCommand).proTarget.GetComponent<SelectableObject>();
					TargetText.text = obj.ObjectClass;
					break;
				}
				
				if (target.captain != null && target.captain.ShipBuilder != null && target.captain.ShipBuilder.ShipsInList != null && target.captain.ShipBuilder.ShipsInList.Count > 0)
				{
					TargetText.text = target.captain.ShipBuilder.ShipsInList[0].ObjectName;
					break;
				}
				
				if (target.captain != null && target.captain.LevelUpdater != null && target.captain.LevelUpdater.levels != null && target.captain.LevelUpdater.levels.Count > 0)
				{
					TargetText.text = target.captain.LevelUpdater.levels[0].Name;
				}
				break;
			case ElementType.BuilderTargetIcon:
				if (target is ObjectUnderConstruction)
				{
					TargetImage.sprite = target.ObjectIcon;
					break;
				}
				
				if (target.captain != null && target.captain.Builder != null && target.captain.curCommandInfo != null && target.captain.curCommandInfo is BuildingCommand && (target.captain.curCommandInfo as BuildingCommand).proTarget != null)
				{
					SelectableObject obj = (target.captain.curCommandInfo as BuildingCommand).proTarget.GetComponent<SelectableObject>();
					TargetImage.sprite = obj.ObjectIcon;
					break;
				}
				
				if (target.captain != null && target.captain.ShipBuilder != null && target.captain.ShipBuilder.ShipsInList != null && target.captain.ShipBuilder.ShipsInList.Count > 0)
				{
					TargetImage.sprite = target.captain.ShipBuilder.ShipsInList[0].Icon;
					break;
				}
				
				if (target.captain != null && target.captain.LevelUpdater != null && target.captain.LevelUpdater.levels != null && target.captain.LevelUpdater.levels.Count > 0)
				{
					TargetImage.sprite = target.captain.LevelUpdater.levels[0].Icon;
				}
				break;
			case ElementType.Recrew:
				if (target.healthSystem && target._hs.isRecrewing)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			case ElementType.UpgradeButton:
				TargetImage.sprite = target.captain.LevelUpdater.levels[0].ButtonImage;
				break;
		}
	}
	private void SystemBarControl(float maxSystemHealth, float curSystemHealth, bool Activity)
    {
        if (maxSystemHealth > 0)
        {
            TargetImage.fillAmount = curSystemHealth / maxSystemHealth;
            if (Activity)
            {
                if (curSystemHealth > maxSystemHealth - (maxSystemHealth / 3))
                {
                    TargetImage.color = Color.green;
                }
                if (curSystemHealth < maxSystemHealth - (maxSystemHealth / 3) && curSystemHealth > maxSystemHealth / 2)
                {
                    TargetImage.color = Color.yellow;
                }
                if (curSystemHealth < maxSystemHealth / 2 && curSystemHealth > maxSystemHealth / 4)
                {
                    TargetImage.color = new Color32(255, 174, 0, 255);
                }
            }
        }
        else
        {
            TargetImage.color = Color.grey;
            TargetImage.fillAmount = 1;
        }
    }

    private void SystemIconControl(float maxSystemHealth, float curSystemHealth, bool Activity, Color fullHealthColor)
    {
        if (maxSystemHealth > 0)
        {
            if (!Activity)
            {
                TargetImage.color = DeactiveSystemColor;
            }
            else
            {
                if (curSystemHealth > maxSystemHealth - (maxSystemHealth / 3))
                {
                    TargetImage.color = fullHealthColor;
                }
                if (curSystemHealth < maxSystemHealth - (maxSystemHealth / 3) && curSystemHealth > maxSystemHealth / 2)
                {
                    TargetImage.color = Color.yellow;
                }
                if (curSystemHealth < maxSystemHealth / 2 && curSystemHealth > maxSystemHealth / 4)
                {
                    TargetImage.color = new Color32(255, 174, 0, 255);
                }
                if (curSystemHealth < maxSystemHealth / 4)
                {
                    TargetImage.color = Color.red;
                }
                if (curSystemHealth <= 0)
                {
                    TargetImage.color = Color.grey;
                }
            }
        }
        else
        {
            TargetImage.color = Color.grey;
        }
    }
    
    private void SHERBar(float CurVar, float MaxVar, bool Revert = false)
    {
	    if (!Revert)
	    {
		    if (CurVar > 0)
		    {
			    TargetImage.fillAmount = CurVar / MaxVar;
		    }
		    else
		    {
			    TargetImage.fillAmount = 0;
		    }
	    }
	    else
	    {
		    if (CurVar > 0)
		    {
			    float tx = CurVar / MaxVar;
			    TargetImage.fillAmount = 1-tx;
		    }
		    else
		    {
			    TargetImage.fillAmount = 1;
		    }
	    }
    }

    public void OnBlueprintClick()
    {
	    mainInterfaceSystem.player.transform.position = new Vector3(target.transform.position.x, mainInterfaceSystem.player.transform.position.y,
		    target.transform.position.z);
    }
    public void OnIconClick()
    {
	    mainInterfaceSystem.player.SelectionList.Clear();
	    mainInterfaceSystem.player.SelectionList.Add(target);
    }
}
