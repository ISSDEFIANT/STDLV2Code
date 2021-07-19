using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class CameraZoom : MonoBehaviour
{
private float zoomPos = 0; 
public float maxHeight = 10f; //maximal height
public float minHeight = 15f; //minimnal height
public float heightDampening = 5f;
private float scrollWheelZoomingSensitivity;
public float scrollWheelZoomingSensitivityMin = -1f;
public float scrollWheelZoomingSensitivityMax = -25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scrollWheelZoomingSensitivity = Mathf.Lerp(scrollWheelZoomingSensitivityMin,
            scrollWheelZoomingSensitivityMax,
            transform.localPosition.z / maxHeight);

        HeightCalculation();
    }
    
    private float ScrollWheel
    {
        get
        {
            if (Input.GetKey(KeyCode.LeftShift)) return 0;
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                float X;
                float Y;
                X = Screen.width / 100;
                Y = Screen.height / 100;

                if (Input.mousePosition.y < Y * 25 & Input.mousePosition.y > Y * 0 & Input.mousePosition.x < X * 14 &
                    Input.mousePosition.x > X * 0)
                {
                    return 0;
                }
            }
            return Input.GetAxis("Mouse ScrollWheel");
        }
    }
    
    private void HeightCalculation()
    {
        zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
        zoomPos = Mathf.Clamp01(zoomPos);

        float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
        float difference = 0;

        transform.localPosition = Vector3.Lerp(transform.localPosition, 
            new Vector3(transform.localPosition.x, transform.localPosition.y, targetHeight + difference), Time.deltaTime * heightDampening);
    }
}