using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
private float zoomPos = -50; 
public float maxHeight = 10f; //maximal height
public float minHeight = 15f; //minimnal height
public float heightDampening = 5f; 
public float scrollWheelZoomingSensitivity = 25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HeightCalculation();
    }
    
    private float ScrollWheel
    {
        get { return Input.GetAxis("Mouse ScrollWheel"); }
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