using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
	public Camera itsMinimapCamera;
	public GameObject itsMainCamera;

	public Vector3 LeftUpPoint;
	public Vector3 RightUpPoint;
	public Vector3 LeftDownPoint;
	public Vector3 RightDownPoint;

	public LineRenderer MinimapLine;
	
	public float minMinimapScale = 200;
	public float maxMinimapScale = 1000;

	private float zoomPos;

	// Use this for initialization
	void Start()
	{

	}
	private float ScrollWheel
	{
		get
		{
			float X;
			float Y;
			X = Screen.width / 100;
			Y = Screen.height / 100;
			
			if (Input.mousePosition.y < Y * 25 & Input.mousePosition.y > Y * 0 & Input.mousePosition.x < X * 14 &
			    Input.mousePosition.x > X * 0)
			{
				if (Input.GetKey(KeyCode.LeftAlt))
				{
					return Input.GetAxis("Mouse ScrollWheel");
				}
				return 0;
			}
			return 0;
		}
	}
	// Update is called once per frame
	void Update()
	{
		float X;
		float Y;
		X = Screen.width / 100;
		Y = Screen.height / 100;
		
		if (Input.mousePosition.y < Y * 25 & Input.mousePosition.y > Y * 0 & Input.mousePosition.x < X * 14 & Input.mousePosition.x > X * 0)
		{
			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				Ray ray = itsMinimapCamera.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit))
				{
					itsMainCamera.transform.position = new Vector3(hit.point.x, itsMainCamera.transform.position.y, hit.point.z);
				}
			}
			zoomPos += ScrollWheel * Time.deltaTime * -100;
			zoomPos = Mathf.Clamp01(zoomPos);
				
			float targetHeight = Mathf.Lerp(minMinimapScale, maxMinimapScale, zoomPos);
			float difference = 0;

			itsMinimapCamera.orthographicSize = Mathf.Lerp(itsMinimapCamera.orthographicSize, 
				targetHeight + difference, Time.deltaTime * 5f);
		}
		
		Plane ground = new Plane(Vector3.up, Vector3.zero);
		
		Camera _mc = Camera.main;
		Ray tlray1 = _mc.ScreenPointToRay(new Vector3(0, 0, 0));
		if (ground.Raycast(tlray1, out float position))
		{
			LeftUpPoint = tlray1.GetPoint(position);
		}
		Ray tlray2 = _mc.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
		if (ground.Raycast(tlray2, out float position2))
		{
			RightUpPoint = tlray2.GetPoint(position2);
		}
		Ray tlray3 = _mc.ScreenPointToRay(new Vector3(0, Screen.height, 0));
		if (ground.Raycast(tlray3, out float position3))
		{
			LeftDownPoint = tlray3.GetPoint(position3);
		}
		Ray tlray4 = _mc.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));
		if (ground.Raycast(tlray4, out float position4))
		{
			RightDownPoint = tlray4.GetPoint(position4);
		}

		MinimapLine.SetPosition(0, LeftDownPoint);
		MinimapLine.SetPosition(1, LeftUpPoint);
		MinimapLine.SetPosition(2, RightUpPoint);
		MinimapLine.SetPosition(3, RightDownPoint);

		/*if (_cms.LocalMinimap.activeSelf)
		{
			MinimapLine.startWidth = 3;
			MinimapLine.endWidth = 3;
		}
		else
		{
			MinimapLine.startWidth = 20;
			MinimapLine.endWidth = 20;
		}*/
	}
}