using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorModule : MonoBehaviour
{
	public DoorInformation[] DoorParts;

	public float OpenPS;

	public float MoveSpeed;
	public bool isOpen;

	public bool TriggerOpenSystem;
	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frame

	void OnTriggerEnter(Collider coll)
	{
		if(!coll.transform.root.GetComponent<SelectableObject>() || coll.transform.root == transform.root) return;
		if (TriggerOpenSystem)
		{
			isOpen = true;
		}
	}
	void OnTriggerExit(Collider coll)
	{
		if(!coll.transform.root.GetComponent<SelectableObject>() || coll.transform.root == transform.root) return;
		if (TriggerOpenSystem)
		{
			isOpen = false;
		}
	}

	void Update()
	{
		if (isOpen)
		{
			if (OpenPS > 0)
			{
				OpenPS -= Time.deltaTime / MoveSpeed;
			}
			else
			{
				OpenPS = 0;
			}
		}
		else
		{
			if (OpenPS < 1)
			{
				OpenPS += Time.deltaTime / MoveSpeed;
			}
			else
			{
				OpenPS = 1;
			}
		}

		foreach (DoorInformation obj in DoorParts)
		{
			Quaternion _orot = Quaternion.Euler(obj.OpenRotation.x, obj.OpenRotation.y, obj.OpenRotation.z);
			Quaternion _crot = Quaternion.Euler(obj.CloseRotation.x, obj.CloseRotation.y, obj.CloseRotation.z);

			obj.DoorPart.transform.localPosition = Vector3.Lerp(obj.OpenPosition, obj.ClosePosition, OpenPS);
			obj.DoorPart.transform.localRotation = Quaternion.Lerp(_orot, _crot, OpenPS);
		}
	}
}
[System.Serializable]
public class DoorInformation
{
	public GameObject DoorPart;
	public Vector3 OpenPosition;
	public Vector3 ClosePosition;
	public Vector3 OpenRotation;
	public Vector3 CloseRotation;
}