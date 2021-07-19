using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoiseObject : MonoBehaviour
{
    public SelectableObject owner;

    public GameObject FarNoise;
    public GameObject NearNoise;
    public Renderer[] Model;
    public GameObject SelectionOnject; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(owner.isVisible == STMethods.Visibility.FarNoise) FarNoise.transform.LookAt(Camera.main.transform.position, Vector3.up);
        if(owner.isVisible == STMethods.Visibility.NearNoise && NearNoise == FarNoise) NearNoise.transform.LookAt(Camera.main.transform.position, Vector3.up);
    }

    // Update is called once per frame
    public void UpdateModel()
    {
        if(owner.destroyed) return;
        switch (owner.isVisible)
        {
            case STMethods.Visibility.Invisible:
                SelectionOnject.SetActive(false);
                FarNoise.SetActive(false);
                NearNoise.SetActive(false);
                foreach (Renderer modelpart in Model)
                {
                    modelpart.enabled = false;
                }
                break;
            case STMethods.Visibility.Visible:
                SelectionOnject.SetActive(true);
                FarNoise.SetActive(false);
                NearNoise.SetActive(false);
                foreach (Renderer modelpart in Model)
                {
                    modelpart.enabled = true;
                }
                break;
            case STMethods.Visibility.FarNoise:
                SelectionOnject.SetActive(false);
                FarNoise.SetActive(true);
                NearNoise.SetActive(false);
                foreach (Renderer modelpart in Model)
                {
                    modelpart.enabled = false;
                }
                break;
            case STMethods.Visibility.NearNoise:
                SelectionOnject.SetActive(false);
                FarNoise.SetActive(false);
                NearNoise.SetActive(true);
                foreach (Renderer modelpart in Model)
                {
                    modelpart.enabled = false;
                }
                break;
        }
    }
}
