using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnyZoom : MonoBehaviour
{
    public Camera camera;
    
    public float maxHeight = 300f; //maximal height
    public float minHeight = 50f; //minimnal height
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        camera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -10;
    }
}
