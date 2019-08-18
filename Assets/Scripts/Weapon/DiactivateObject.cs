using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiactivateObject : MonoBehaviour {
	public float LifeTime;
	private float timer;
	// Use this for initialization
	void Start () {
		timer = LifeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer >= 0) {
			timer -= Time.deltaTime;
		} else {
			gameObject.SetActive (false);
			transform.position = Vector3.zero;
			timer = LifeTime;
		}
	}
	public void Diactivate(){
		gameObject.SetActive (false);
		transform.position = Vector3.zero;
	}
}
