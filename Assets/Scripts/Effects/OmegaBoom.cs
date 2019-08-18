using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmegaBoom : MonoBehaviour {
	public float Delay;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Delay > 0) {
			foreach (Transform obj in gameObject.transform) {
				obj.gameObject.SetActive (false);
			}
			Delay -= Time.deltaTime;
		} else {
			foreach (Transform obj in transform) {
				obj.gameObject.SetActive (true);
			}
		}
	}
}
