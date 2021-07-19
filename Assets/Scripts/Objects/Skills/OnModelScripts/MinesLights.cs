using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MinesLights : MonoBehaviour
{
    public float timer = 3;
    public float activeTime = 1;

    private float primalTimer;
    private float primalActiveTimer;

    private Renderer mesh;
    // Start is called before the first frame update
    void Awake()
    {
        primalTimer = timer;
        primalActiveTimer = activeTime;

        mesh = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (activeTime > 0)
            {
                mesh.material.SetColor("_EmissionColor", Color.white);
                activeTime -= Time.deltaTime;
            }
            else
            {
                mesh.material.SetColor("_EmissionColor", Color.black);
                activeTime = primalActiveTimer;
                timer = primalTimer;
            }
        }
    }
}
