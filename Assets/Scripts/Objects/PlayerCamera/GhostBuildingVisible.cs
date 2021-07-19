using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBuildingVisible : MonoBehaviour
{
	private Mobile OldShip;

	public PlayerCameraControll player;

	private BuildingCommand _buildingOrder;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (player.SelectionList.Count == 1 && (player.SelectionList[0] is Mobile))
		{
			if (OldShip != null && OldShip != player.SelectionList[0])
			{
				if ((OldShip.captain.curCommandInfo as BuildingCommand).ghost != null)
				{
					(OldShip.captain.curCommandInfo as BuildingCommand).ghost.SetActive(false);
				}
			}
			if (player.SelectionList[0].captain.curCommandInfo is BuildingCommand)
			{
				_buildingOrder = player.SelectionList[0].captain.curCommandInfo as BuildingCommand;
				OldShip = player.SelectionList[0] as Mobile;

				if (player.SelectionList[0].PlayerNum == player.PlayerNum)
				{
					if (_buildingOrder.ghost != null)
					{
						_buildingOrder.ghost.SetActive(true);
					}
				}
				else
				{
					if (_buildingOrder.ghost != null)
					{
						_buildingOrder.ghost.SetActive(false);
					}
				}
			}
			else
			{
				_buildingOrder = null;
			}
		}
		else
		{
			_buildingOrder = null;
			if (OldShip != null)
			{
				if (OldShip.captain.curCommandInfo is BuildingCommand)
				{
					if ((OldShip.captain.curCommandInfo as BuildingCommand).ghost != null)
					{
						(OldShip.captain.curCommandInfo as BuildingCommand).ghost.SetActive(false);
					}
				}
				else
				{
					OldShip = null;
				}
			}
			OldShip = null;
		}
	}
}