using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColorChanger : MonoBehaviour
{
    public Color BorgColor = Color.green;
    public Color NormalColor = Color.white;
    
    private SelectableObject _so;

    private Renderer mesh = null;
    private Light targetLight = null;
    // Start is called before the first frame update
    void Start()
    {
        _so = GetComponentInParent<SelectableObject>();
        if (gameObject.GetComponent<Renderer>()) mesh = gameObject.GetComponent<Renderer>();
        if (gameObject.GetComponent<Light>()) targetLight = gameObject.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_so.Assimilated)
        {
            if (mesh != null) mesh.material.color = BorgColor;
            if (targetLight != null) targetLight.color = BorgColor;
        }
        else
        {
            if (mesh != null) mesh.material.color = NormalColor;
            if (targetLight != null) targetLight.color = NormalColor;
        }
    }
}
