using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour {
	public Color32 color;

	public float radius = 1.0f;

	private float oldRadius;

	[Range(3, 256)]
	public int numSegments = 128;

	private LineRenderer lineRenderer;

	void Start ( ) {
		lineRenderer = gameObject.GetComponent<LineRenderer>();
	}

	void Update(){
		if (oldRadius != radius) {
			DoRenderer ();
			oldRadius = radius;
		}
	}

	public void DoRenderer ( ) {
		lineRenderer.material = new Material(Shader.Find("Particles/Additive_Always"));
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		lineRenderer.startWidth = 0.5f;
		lineRenderer.endWidth = 0.5f;
		lineRenderer.positionCount = numSegments + 1;
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0 ; i < numSegments + 1 ; i++) {
			float x = radius * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			lineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}
}