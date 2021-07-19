using System.Collections;
using System.Collections.Generic;
using RTS_Cam;
using UnityEngine;

public class GlobalMinimapController : MonoBehaviour
{
    public Camera tarCamera;
    
    public GlobalInterfaceEventSystem playerInterface;
    
    public float minMinimapScale = 750;
    public float maxMinimapScale = 9000;
    
    private float scrollWheelZoomingSensitivityMin = 1f;
    private float scrollWheelZoomingSensitivityMax = 5f;
    
    private float zoomPos = 9000;
    
    private float ScrollWheel
    {
        get
        {
            return Input.GetAxis("Mouse ScrollWheel") * Mathf.Lerp(scrollWheelZoomingSensitivityMax, scrollWheelZoomingSensitivityMin, tarCamera.orthographicSize / maxMinimapScale);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        tarCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInterface == null) return;

        if (playerInterface.globalStatus != GlobalInterfaceEventSystem.globalInterfaceStatus.GlobalMinimap)
        {
            return;
        }
        zoomPos += ScrollWheel * Time.deltaTime * -10;
        zoomPos = Mathf.Clamp01(zoomPos);
				
        float targetHeight = Mathf.Lerp(minMinimapScale, maxMinimapScale, zoomPos);
        float difference = 0;

        tarCamera.orthographicSize = Mathf.Lerp(tarCamera.orthographicSize, 
            targetHeight + difference, Time.deltaTime * 5f);

        MovingMethod();
    }

    public void Init(GlobalInterfaceEventSystem pInterface)
    {
        playerInterface = pInterface;
        playerInterface.player.PlayerCameras.GlobalMinimapCamera = tarCamera;
    }

    private void MovingMethod()
    {
        if (transform.position.z > -(9000 - tarCamera.orthographicSize))
        {
            if ((int) Input.mousePosition.x < 2)
            {
                transform.position -= transform.right * Time.deltaTime * Mathf.Lerp(500, 10000, tarCamera.orthographicSize / maxMinimapScale);
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -(9000 - tarCamera.orthographicSize));
        }
        if (transform.position.z < (9000 - tarCamera.orthographicSize))
        {
            if ((int) Input.mousePosition.x > Screen.width - 2)
            {
                transform.position += transform.right * Time.deltaTime * Mathf.Lerp(500, 10000, tarCamera.orthographicSize / maxMinimapScale);
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, (9000 - tarCamera.orthographicSize));
        }
        
        if (transform.position.x > -(9000 - tarCamera.orthographicSize))
        {
            if ((int) Input.mousePosition.y < 2)
            {
                transform.position -= transform.up * Time.deltaTime * Mathf.Lerp(500, 10000, tarCamera.orthographicSize / maxMinimapScale);
            }
        }
        else
        {
            transform.position = new Vector3(-(9000 - tarCamera.orthographicSize), transform.position.y, transform.position.z);
        }
        if (transform.position.x < (9000 - tarCamera.orthographicSize))
        {
            if ((int) Input.mousePosition.y > Screen.height - 2)
            {
                transform.position += transform.up * Time.deltaTime * Mathf.Lerp(500, 10000, tarCamera.orthographicSize / maxMinimapScale);
            }
        }
        else
        {
            transform.position = new Vector3((9000 - tarCamera.orthographicSize), transform.position.y, transform.position.z);
        }
    }
}