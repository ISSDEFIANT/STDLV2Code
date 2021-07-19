using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PatrolVisual : MonoBehaviour {
	public List<GameObject> Points;
	public GameObject PointPref;

	public LineRenderer PatrolLine;

	private Mobile OldShip;

	public PlayerCameraControll player;

	private PatrolCommand _patrolOrder;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update()
	{
		if (!player.ShiftPatrolSetting && player.CameraState != STMethods.PlayerCameraState.PatrolSetting)
		{
			if (player.SelectionList.Count == 1 && (player.SelectionList[0] is Mobile))
			{
				if (player.SelectionList[0].captain.curCommandInfo is PatrolCommand)
				{
					_patrolOrder = player.SelectionList[0].captain.curCommandInfo as PatrolCommand;
				}
				else
				{
					_patrolOrder = null;
				}
			}
			else
			{
				_patrolOrder = null;
			}
		}
		else
		{
			_patrolOrder = player._pc;
		}

		if (_patrolOrder != null)
		{
			if (_patrolOrder.targetVec.Count > 0)
			{
				if (Points.Count < _patrolOrder.targetVec.Count)
				{
					GameObject inst = Instantiate(PointPref, Vector3.zero, Quaternion.Euler(Vector3.zero));
					Points.Add(inst);
				}

				PatrolLine.gameObject.SetActive(true);
				PatrolLine.positionCount = _patrolOrder.targetVec.Count;
				PatrolLine.SetPositions(_patrolOrder.targetVec.ToArray());

				if (Points.Count > 0)
				{
					for (int i = 0; i < Points.Count; i++)
					{
						if (i < _patrolOrder.targetVec.Count)
						{
							Points[i].transform.position = _patrolOrder.targetVec[i];
							Points[i].SetActive(true);
						}
						else
						{
							Points[i].SetActive(false);
						}
					}
				}
			}
			else
			{
				foreach (GameObject obj in Points)
				{
					obj.SetActive(false);
				}
				PatrolLine.gameObject.SetActive(false);
			}
		}
		else
		{
			foreach (GameObject obj in Points)
			{
				obj.SetActive(false);
			}
			PatrolLine.gameObject.SetActive(false);
		}
	}
}