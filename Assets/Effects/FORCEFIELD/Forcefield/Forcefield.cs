using UnityEngine;
using System.Collections;

public class Forcefield : MonoBehaviour
{
	private Renderer ren;
	private Material[] mats;
	private MeshFilter mesh;

	public float Timer;
	public float curTimer;

	public GameObject forceField;

	public float delayTime = 2.0f;
	public bool Shot;

	public bool ClockingEffect;
	
	public float ShieldDeactivationTimer = 2f;

	private float curshielddeacttimer;
	// Use this for initialization
	void Start()
	{
		ren = forceField.gameObject.GetComponent<Renderer>();
		mats = ren.materials;
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
		curshielddeacttimer = ShieldDeactivationTimer;
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
				UpdateMask(Vector3.zero);
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

		if (curshielddeacttimer > 0)
		{
			curshielddeacttimer -= Time.deltaTime;
		}
		else
		{
			if (ren.enabled) ren.enabled = false;
		}
		FadeMask();
	}
}