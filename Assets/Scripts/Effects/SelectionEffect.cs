using UnityEngine;
using System.Collections;

public class SelectionEffect : MonoBehaviour
{
	public bool Diactive;
	private Vector3 vec;

	// Use this for initialization
	void Start()
	{
		vec = transform.rotation.eulerAngles;
	}

	// Update is called once per frame
	void Update()
	{
		if(!Diactive)
		transform.rotation = Quaternion.Euler(vec);
	}
}