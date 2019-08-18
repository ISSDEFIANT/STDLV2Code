using UnityEngine;
using System.Collections;

public class Forcefield : MonoBehaviour
{

	private Material[] mats;
	private MeshFilter mesh;

	public float Timer;
	public float curTimer;

	public GameObject forceField;

	public float delayTime = 2.0f;
	public bool Shot;
	public Vector3 AttackVector;

	public bool ClockingEffect;
	
	public float ShieldDeactivationTimer;
	// Use this for initialization
	void Start()
	{
		mats = forceField.gameObject.GetComponent<Renderer>().materials;
		mesh = forceField.gameObject.GetComponent<MeshFilter>();
	}

	void UpdateMask(Vector3 hitPoint)
	{
		foreach (Material m in mats)
		{
			Vector4 vTemp = mesh.transform.InverseTransformPoint(hitPoint);
			vTemp.w = 1.0f;

			m.SetVector("_Pos_a", vTemp);
		}
	}

	void FadeMask()
	{
		for (int u = 0; u < mats.Length; u++)
		{
			if (mats[u].shader == Shader.Find("Forcefield/Forcefield"))
			{
				Vector4 oldPos = mats[u].GetVector("_Pos_a");

				if (oldPos.w > 0.005)
				{
					Vector4 NewPos = oldPos;
					NewPos.w = 0.0f;

					Vector4 vTemp = Vector4.Lerp(oldPos, NewPos, Time.deltaTime * delayTime);
					mats[u].SetVector("_Pos_a", vTemp);
				}
			}
		}
	}

	public void OnHit(Vector3 hitPoint)
	{
		UpdateMask(hitPoint);
	}

	//void OnMouseHit()
	//{
	//  if (Input.GetMouseButtonDown(0))
	//{
	//  RaycastHit hit;
	//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	//
	//      MeshCollider col = gameObject.GetComponentInChildren<MeshCollider>();
	//	if (col.Raycast(ray, out hit, 100.0f))            
	//		UpdateMask(PhaserHit.point);
	//}    
	//}

	// Update is called once per frame
	void Update()
	{
		//OnMouseHit();
		if (ClockingEffect)
		{
			if (Shot)
			{
				UpdateMask(AttackVector);
				if (curTimer > 0)
				{
					curTimer -= Time.deltaTime;
				}

				if (curTimer <= 0)
				{
					curTimer = Timer;
					Shot = false;
				}
			}

			FadeMask();
		}
		else
		{
			FadeMask();
			if (Shot)
			{
				UpdateMask(transform.position);
				if (curTimer > 0)
				{
					curTimer -= Time.deltaTime;
				}

				if (curTimer <= 0)
				{
					curTimer = Timer;
					Shot = false;
				}
			}
		}
	}
}