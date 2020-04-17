using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningBeam : MonoBehaviour
{
    private float ReloadTimer = 1;
    private ArcReactor_Launcher Beam;
    // Start is called before the first frame update
    void Start()
    {
        Beam = gameObject.GetComponent<ArcReactor_Launcher>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Active(Transform target)
    {
        if (ReloadTimer > 0)
        {
            ReloadTimer -= Time.deltaTime;
        }
        else
        {
            Beam.PhaserFire(target);
            ReloadTimer = 1;
        }
    }
}
