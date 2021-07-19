using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class IconControlScript : MonoBehaviour
{ 
	public Image Icon;
	public Image Fraction;

	public Sprite Neutral;
	public Sprite Federation;
	public Sprite Klingon;
	public Sprite Romulan;
	public Sprite Cardassian;
	public Sprite Borg;
	public Sprite S8472;

	public Sprite NullSprite;

	public GlobalInterfaceEventSystem mainInterfaceScript;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (mainInterfaceScript.player.SelectionList.Count > 0)
		{
			switch (mainInterfaceScript.player.SelectionList[0].ControllingFraction)
			{
				case STMethods.Races.Federation:
					if (Fraction.sprite != Federation) Fraction.sprite = Federation;
					break;
				case STMethods.Races.Klingon:
					if (Fraction.sprite != Klingon) Fraction.sprite = Klingon;
					break;
				case STMethods.Races.Romulan:
					if (Fraction.sprite != Romulan) Fraction.sprite = Romulan;
					break;
				case STMethods.Races.Cardassian:
					if (Fraction.sprite != Cardassian) Fraction.sprite = Cardassian;
					break;
				case STMethods.Races.Borg:
					if (Fraction.sprite != Borg) Fraction.sprite = Borg;
					break;
				case STMethods.Races.S8472:
					if (Fraction.sprite != S8472) Fraction.sprite = S8472;
					break;
				case STMethods.Races.None:
					if (Fraction.sprite != Neutral) Fraction.sprite = Neutral;
					break;
			}

			if (mainInterfaceScript.player.SelectionList[0].ObjectIcon != null)
			{
				Icon.sprite = mainInterfaceScript.player.SelectionList[0].ObjectIcon;
				if (Icon.color != Color.white) Icon.color = Color.white;
			}
			else
			{
				if (Icon.color != Color.clear) Icon.color = Color.clear;
			}
		}
		else
		{
			if (Fraction.sprite != NullSprite) Fraction.sprite = NullSprite;
			if (Icon.color != Color.clear) Icon.color = Color.clear;
		}
	}
}