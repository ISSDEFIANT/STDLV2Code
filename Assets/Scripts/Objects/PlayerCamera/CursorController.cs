using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour
{
	public CursorInfo Abilitis;

    public CursorInfo Attack;

    //
    public CursorInfo Colony;
    //Пока не создано

    public CursorInfo Guard;

    public CursorInfo Repair;

    public CursorInfo Move;

    public CursorInfo Hover;

    public CursorInfo Mine;

    //
    public CursorInfo Error;
    //Пока не создано

    public CursorInfo Idle;

    //
    public CursorInfo Transport;
	//Пока не создано


	private PlayerCameraControll _pcc;

	public int numDepth = 1;
	public List<Texture2D> Cursors;
	public Texture2D Static_cursor;
	private int i;
	public float TimerChange;
	private Texture2D cur;
	public bool UseStaticCursor;
	private float TimerDown;

	public float MinusX;
	public float MinusY;

	public float SizeX = 25;
	public float SizeY = 25;

	private Ray _rayShipHover;
	private RaycastHit _hitShipHover;

	int layerMaskForShipHover = 1 << 9;
	int layerMaskForGlobalMinimap = 1 << 14;
	// Use this for initialization
	void Awake()
	{
		_pcc = gameObject.GetComponent<PlayerCameraControll>();

		TimerDown = 0;
	}

	// Update is called once per frame
	void Update()
	{
		UnityEngine.Cursor.visible = false;
		if (Cursors.Count > 0)
		{
			if (i >= Cursors.Count)
			{
				i = 0;
			}
		}

		List<RaycastResult> results = new List<RaycastResult>();
		PointerEventData interfaceData = new PointerEventData(_pcc.interfaceEventSystem);
		interfaceData.position = Input.mousePosition;
		_pcc.interface_Raycaster.Raycast(interfaceData, results);

		if (results.Count > 0)
		{
			SetCursor(Idle);
			return;
		}

		if (_pcc.globalInterface.globalStatus == GlobalInterfaceEventSystem.globalInterfaceStatus.GlobalMinimap)
		{
			_rayShipHover = _pcc.PlayerCameras.GlobalMinimapCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_rayShipHover, out _hitShipHover, 10000.0f, layerMaskForGlobalMinimap))
			{
				GlobalMinimapMark Target = _hitShipHover.transform.GetComponent<GlobalMinimapMark>();

				if (_pcc.SelectionList.Count > 0)
				{
					SelectableObject _tst = Target.curClaster[0];

					if (_tst.PlayerNum > 0)
					{
						if (GameManager.instance.Players[_tst.PlayerNum - 1].TeamNum ==
						    GameManager.instance.Players[_pcc.PlayerNum - 1].TeamNum)
						{
							foreach (SelectableObject ships in _pcc.SelectionList)
							{
								if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
								    _pcc.SetNewCommand == Captain.PlayerCommand.SettingAbilityTarget &&
								    _tst != ships)
								{
									if (_pcc.curSelectedAbility.SkillTarget == Skill.skillTarget.Object ||
									    _pcc.curSelectedAbility.SkillTarget ==
									    Skill.skillTarget.PlayerThroughObject)
									{
										SetCursor(Abilitis);
										return;
									}
								}

								if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
								    _pcc.SetNewCommand == Captain.PlayerCommand.Guard && _tst != ships)
								{
									SetCursor(Guard);
									return;
								}

								if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
								    _pcc.SetNewCommand == Captain.PlayerCommand.Attack && _tst != ships)
								{
									SetCursor(Attack);
									return;
								}

								if (_tst.PlayerNum == _pcc.PlayerNum)
								{
									if (ships._hs != null && _tst != ships)
									{
										if (!(ships is Static) && _tst.GetComponent<FixingPointController>())
										{
											SetCursor(Repair);
											return;
										}
									}

									if (ships.captain != null && ships.captain.Miner != null && _tst != ships)
									{
										if (_tst.GetComponent<ResourceUnloadingController>())
										{
											SetCursor(Mine);
											return;
										}
									}

									SetCursor(Hover);
									return;
								}

								SetCursor(Hover);
								return;
							}
						}
						else
						{
							if (_pcc.SelectionList[0].PlayerNum == _pcc.PlayerNum)
							{
								SetCursor(Attack);
								return;
							}

							SetCursor(Hover);
							return;
						}
					}
					else
					{
						foreach (SelectableObject ships in _pcc.SelectionList)
						{
							if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
							    _pcc.SetNewCommand == Captain.PlayerCommand.SettingAbilityTarget && _tst != ships)
							{
								if (_pcc.curSelectedAbility.SkillTarget == Skill.skillTarget.Object ||
								    _pcc.curSelectedAbility.SkillTarget == Skill.skillTarget.PlayerThroughObject)
								{
									SetCursor(Abilitis);
									return;
								}
							}

							if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
							    _pcc.SetNewCommand == Captain.PlayerCommand.Guard && _tst != ships)
							{
								SetCursor(Guard);
								return;
							}

							if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
							    _pcc.SetNewCommand == Captain.PlayerCommand.Attack && _tst != ships)
							{
								SetCursor(Attack);
								return;
							}

							if (_tst is ResourceSource && ships.captain != null && ships.captain.Miner != null &&
							    _tst != ships)
							{
								if (_tst.GetComponent<ResourceSource>())
								{
									SetCursor(Mine);
									return;
								}
							}

							SetCursor(Hover);
							return;
						}
					}
				}

				SetCursor(Hover);
				return;
			}
		}
		else
		{
			_rayShipHover = _pcc.PlayerCameras.MainCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_rayShipHover, out _hitShipHover, 10000.0f, layerMaskForShipHover))
			{
				GameObject Target = _hitShipHover.transform.gameObject;

				if (_pcc.SelectionList.Count > 0)
				{
					if (Target.GetComponent<SelectableObject>())
					{
						SelectableObject _tst = Target.GetComponent<SelectableObject>();

						if (_tst.PlayerNum > 0)
						{
							if (GameManager.instance.Players[_tst.PlayerNum - 1].TeamNum ==
							    GameManager.instance.Players[_pcc.PlayerNum - 1].TeamNum)
							{
								foreach (SelectableObject ships in _pcc.SelectionList)
								{
									if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
									    _pcc.SetNewCommand == Captain.PlayerCommand.SettingAbilityTarget &&
									    _tst != ships)
									{
										if (_pcc.curSelectedAbility.SkillTarget == Skill.skillTarget.Object ||
										    _pcc.curSelectedAbility.SkillTarget ==
										    Skill.skillTarget.PlayerThroughObject)
										{
											SetCursor(Abilitis);
											return;
										}
									}

									if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
									    _pcc.SetNewCommand == Captain.PlayerCommand.Guard && _tst != ships)
									{
										SetCursor(Guard);
										return;
									}

									if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
									    _pcc.SetNewCommand == Captain.PlayerCommand.Attack && _tst != ships)
									{
										SetCursor(Attack);
										return;
									}

									if (_tst.PlayerNum == _pcc.PlayerNum)
									{
										if (ships._hs != null && _tst != ships)
										{
											if (!(ships is Static) && _tst.GetComponent<FixingPointController>())
											{
												SetCursor(Repair);
												return;
											}
										}

										if (ships.captain != null && ships.captain.Miner != null && _tst != ships)
										{
											if (_tst.GetComponent<ResourceUnloadingController>())
											{
												SetCursor(Mine);
												return;
											}
										}

										SetCursor(Hover);
										return;
									}

									SetCursor(Hover);
									return;
								}
							}
							else
							{
								if (_pcc.SelectionList[0].PlayerNum == _pcc.PlayerNum)
								{
									SetCursor(Attack);
									return;
								}

								SetCursor(Hover);
								return;
							}
						}
						else
						{
							foreach (SelectableObject ships in _pcc.SelectionList)
							{
								if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
								    _pcc.SetNewCommand == Captain.PlayerCommand.SettingAbilityTarget && _tst != ships)
								{
									if (_pcc.curSelectedAbility.SkillTarget == Skill.skillTarget.Object ||
									    _pcc.curSelectedAbility.SkillTarget == Skill.skillTarget.PlayerThroughObject)
									{
										SetCursor(Abilitis);
										return;
									}
								}

								if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
								    _pcc.SetNewCommand == Captain.PlayerCommand.Guard && _tst != ships)
								{
									SetCursor(Guard);
									return;
								}

								if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting &&
								    _pcc.SetNewCommand == Captain.PlayerCommand.Attack && _tst != ships)
								{
									SetCursor(Attack);
									return;
								}

								if (_tst is ResourceSource && ships.captain != null && ships.captain.Miner != null &&
								    _tst != ships)
								{
									if (_tst.GetComponent<ResourceSource>())
									{
										SetCursor(Mine);
										return;
									}
								}

								SetCursor(Hover);
								return;
							}
						}
					}
				}

				SetCursor(Hover);
				return;
			}
		}

		if (_pcc.SelectionList.Count > 0)
		{
			if (_pcc.CameraState == STMethods.PlayerCameraState.OrderSetting && _pcc.SetNewCommand == Captain.PlayerCommand.SettingAbilityTarget)
			{
				if (_pcc.curSelectedAbility.SkillTarget == Skill.skillTarget.PositionInSpace)
				{
					SetCursor(Abilitis);
					return;
				}
			}
			if (_pcc.SelectionList[0] is Mobile)
			{
				if (_pcc.SelectionList[0].PlayerNum == _pcc.PlayerNum)
				{
					SetCursor(Move);
					return;
				}
			}
		}

		SetCursor(Idle);
	}

	void SetCursor(CursorInfo ci)
	{
		if(i >= ci.Cursor.Length) i = 0;
		if (ci.Cursor.Length == 1)
        {
            UseStaticCursor = true;
            Static_cursor = ci.Cursor[0];
        }
        else
        {
            UseStaticCursor = false;
        }
      
        Cursors = ci.Cursor.ToList();
        TimerChange = ci.Speed;
        if (!ci.InCenter)
        {
            MinusX = ci.Size.x / 1000;
            MinusY = ci.Size.y / 1000;
        }
        else
        {
            MinusX = ci.Size.x / 2;
            MinusY = ci.Size.y / 2;
        }
        SizeX = ci.Size.x;
        SizeY = ci.Size.y;
    }

    void OnGUI()
	{
		GUI.depth = numDepth;
		Vector2 MP = Input.mousePosition;
		MP.y = Screen.height - MP.y;
		MP.x -= MinusX;
		MP.y -= MinusY;
		if (UseStaticCursor)
		{
			if (Static_cursor != null) GUI.DrawTexture(new Rect(MP.x, MP.y, SizeX, SizeY), Static_cursor);
		}
		else
		{
			if (Cursors.Count > 0)
			{
				GUI.DrawTexture(new Rect(MP.x, MP.y, SizeX, SizeY), cur);
				TimerDown -= Time.deltaTime;
				if (TimerDown <= 0)
				{
					if (Cursors[i] != null)
					{
						cur = Cursors[i];
					}
					i++;
					TimerDown = TimerChange;
				}
			}
		}
	}
}

[System.Serializable]
public class CursorInfo
{
    public bool InCenter;
    public Texture2D[] Cursor;
    public float Speed;
    public Vector2 Size;
}