using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLights : MonoBehaviour
{
    public SelectableObject Owner;
    
    public GameObject[] Lights;

    public float DeactiveTime = 5;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.root.GetComponent<SelectableObject>()) Owner = transform.root.GetComponent<SelectableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Owner != null)
        {
            if (Owner.isVisible != STMethods.Visibility.Visible)
            {
                foreach (GameObject tar in Lights)
                {
                    tar.SetActive(false);
                }
                return;
            }
        }
        if (DeactiveTime > 0.1f)
        {
            foreach (GameObject tar in Lights)
            {
                tar.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject tar in Lights)
            {
                tar.SetActive(true);
            }
        }

        if (DeactiveTime > 0)
        {
            DeactiveTime -= Time.deltaTime;
        }
        else
        {
            DeactiveTime = 5;
        }
    }
}
