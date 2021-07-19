using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetElement : MonoBehaviour
{
	public enum ElementType
	{
		None,
		
		SystemAttackHull,
		SystemAttackPrimaryWeapon,
		SystemAttackSecondaryWeapon,
		SystemAttackImpulseEngine,
		SystemAttackWarpEngine,
		SystemAttackWarpCore,
		SystemAttackLifeSupport,
		SystemAttackSensores,
		SystemAttackTractorBeam,
		
		RedAlertButton,
		YellowAlertButton,
		GreenAlertButton,
		SelfDestructionButton,
		GuardOrder,
		FixButton,

		AttackOrderButton,
		PatrolOrderButton,
		SearchOrder,
		FaDOrder,
	}
	
    public ElementType Element;

    public Sprite OrderActive;
    public Sprite OrderDeActive;
    
    private Image TargetImage;
    private Text TargetText;
    private Toggle TargetToggle;
    private Button TargetButton;
    
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
        if (mainInterfaceSystem.player.SelectionList != null && mainInterfaceSystem.player.SelectionList.Count > 1)
        {
            UpdateInfo();
        }
    }
    
    void UpdateInfo()
	{
		bool isActive = false;
		bool isAvaible = false;
		switch (Element)
		{
			//OrdersMenu

			case ElementType.SystemAttackHull:
				SystemAttackVoid(STMethods.AttackType.NormalAttack);
				break;
			case ElementType.SystemAttackPrimaryWeapon:
				SystemAttackVoid(STMethods.AttackType.PrimaryWeaponSystemAttack);
				break;
			case ElementType.SystemAttackSecondaryWeapon:
				SystemAttackVoid(STMethods.AttackType.SecondaryWeaponSystemAttack);
				break;
			case ElementType.SystemAttackImpulseEngine:
				SystemAttackVoid(STMethods.AttackType.ImpulseSystemAttack);
				break;
			case ElementType.SystemAttackWarpEngine:
				SystemAttackVoid(STMethods.AttackType.WarpEngingSystemAttack);
				break;
			case ElementType.SystemAttackWarpCore:
				SystemAttackVoid(STMethods.AttackType.WarpCoreAttack);
				break;
			case ElementType.SystemAttackLifeSupport:
				SystemAttackVoid(STMethods.AttackType.LifeSupportSystemAttack);
				break;
			case ElementType.SystemAttackSensores:
				SystemAttackVoid(STMethods.AttackType.SensorsSystemAttack);
				break;
			case ElementType.SystemAttackTractorBeam:
				SystemAttackVoid(STMethods.AttackType.TractorBeamSystemAttack);
				break;

			case ElementType.RedAlertButton:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.Alerts == STMethods.Alerts.RedAlert)
					{
						isActive = true;
						break;
					}
				}

				if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			case ElementType.YellowAlertButton:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.Alerts == STMethods.Alerts.YellowAlert)
					{
						isActive = true;
						break;
					}
				}

				if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			case ElementType.GreenAlertButton:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.Alerts == STMethods.Alerts.GreenAlert)
					{
						isActive = true;
						break;
					}
				}

				if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;

			case ElementType.SelfDestructionButton:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target._hs.SelfDestructActive)
					{
						isActive = true;
						break;
					}
				}

				if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			case ElementType.GuardOrder:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.captain.Gunner != null)
					{
						isAvaible = true;
						if (target.captain.curCommandInfo is GuardCommand)
						{
							isActive = true;
							break;
						}
					}
				}
				TargetButton.interactable = isAvaible;
				if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			case ElementType.FixButton:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.captain.curCommandInfo is FixingCommand)
					{
						isActive = true;
						break;
					}
				}
				if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			
			
			
			case ElementType.AttackOrderButton:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.captain.Gunner != null)
					{
						isAvaible = true;
						break;
					}
				}
				TargetButton.interactable = isAvaible;
				break;
			
			case ElementType.PatrolOrderButton:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.captain.Pilot != null)
					{
						isAvaible = true;
						if (target.captain.curCommandInfo is PatrolCommand)
						{
							isActive = true;
							break;
						}
					}
				}
				TargetButton.interactable = isAvaible;
				if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}
				break;
			
			case ElementType.SearchOrder:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.captain.Pilot != null)
					{
						isAvaible = true;
						/*if (target.captain.curCommandInfo is PatrolCommand)
						{
							isActive = true;
							break;
						}*/
					}
				}
				TargetButton.interactable = isAvaible;
				/*if (isActive)
				{
					TargetImage.sprite = OrderActive;
				}
				else
				{
					TargetImage.sprite = OrderDeActive;
				}*/
				break;
			
			case ElementType.FaDOrder:
				foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
				{
					if (target.captain.Pilot != null && target.captain.Gunner != null)
					{
						isAvaible = true;
					}
				}
				TargetButton.interactable = isAvaible;
				break;
		}
	}

    private void SystemAttackVoid(STMethods.AttackType type)
    {
	    bool isActive = false;
	    foreach (SelectableObject target in mainInterfaceSystem.player.SelectionList)
	    {
		    if (target.captain.Gunner.Aiming == type)
		    {
			    isActive = true;
			    break;
		    }
	    }

	    if (isActive)
	    {
		    TargetImage.sprite = OrderActive;
	    }
	    else
	    {
		    TargetImage.sprite = OrderDeActive;
	    }
    }
}
