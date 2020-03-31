using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiactivateObject : MonoBehaviour {
	/// <summary> Время жизни. </summary>
	public float LifeTime;
	/// <summary> Текущее время жизни. </summary>
	private float timer;
	/// <summary> Первая установка текущего времени. </summary>
	void Start () {
		timer = LifeTime;
	}
	
	/// <summary> Работа таймера до отключения. </summary>
	void Update () {
		if (timer >= 0) {
			timer -= Time.deltaTime;
		} else {
			gameObject.SetActive (false);
			transform.position = Vector3.zero;
			timer = LifeTime;
		}
	}
	/// <summary> Отключение. </summary>
	public void Diactivate(){
		gameObject.SetActive (false);
		transform.position = Vector3.zero;
	}
}
