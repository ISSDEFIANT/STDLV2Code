using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RingSpawnZone : MonoBehaviour
{
    public List<ResourceSource> curAsteroids = new List<ResourceSource>();

    public float minRadius;
    public float maxRadius;

    private SystemCentralObject sco;
    // Start is called before the first frame update
    void Start()
    {
        sco = FindObjectOfType<SystemCentralObject>();
        Instantiate(DataLoader.Instance.ResourcesCache["Asteroid Ring"] as GameObject, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (curAsteroids.Count < 100)
        {
            GameObject newA = new GameObject("AsteroidSource", typeof(RandomAsteroidSource));
            float wallRadius = (maxRadius - minRadius) * 0.5f;
            float ringRadius = wallRadius + minRadius;
            newA.transform.position = GetRandomPositionInTorus(ringRadius, wallRadius);
            curAsteroids.Add(newA.GetComponent<ResourceSource>());
            curAsteroids[curAsteroids.Count-1].rigitBody.AddForce(ForceFor (curAsteroids[curAsteroids.Count-1]), ForceMode.Acceleration);
            curAsteroids[curAsteroids.Count - 1].spawner = this;
            GameManager.instance.UpdateList();
        }

        if (curAsteroids.Count > 0)
        {
            for (int i = 0; i < curAsteroids.Count; i++)
            {
                if (curAsteroids[i].destroyed)
                {
                    curAsteroids.Remove(curAsteroids[i]);
                    if (i > 0) i--;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (curAsteroids.Count > 0)
        {
            for (int i = 0; i < curAsteroids.Count; i++)
            {
                if (Vector3.Distance(transform.position, curAsteroids[i].transform.position) > minRadius + 5)
                {
                    curAsteroids[i].rigitBody.AddForce(ForceFor(curAsteroids[i]), ForceMode.Acceleration);
                }
            }
        }
    }

    public Vector3 ForceFor(ResourceSource asteroid)
    {
        float radius = minRadius;
        float sradius = radius * radius;
        float distance = Vector3.Distance(asteroid.transform.position, transform.position);
        float HPoint = sradius / distance;

        Vector3 lv = (transform.position - asteroid.transform.position).normalized;
        Vector3 h = transform.position + lv * HPoint;

        float hc = Mathf.Sqrt(sradius - ((sradius / distance) * (sradius / distance)));

        Vector3 c = h + Vector3.Cross(lv, new Vector3(0, 1, 0)) * hc;

        return (c.normalized - (sco.GravityForce(asteroid)).normalized);
    }

    Vector3 GetRandomPositionInTorus( float ringRadius, float wallRadius )
    {
        // get a random angle around the ring
        float rndAngle = Random.value * 6.28f; // use radians, saves converting degrees to radians
         
        // determine position
        float cX = Mathf.Sin( rndAngle );
        float cZ = Mathf.Cos( rndAngle );
         
        Vector3 ringPos = new Vector3( cX, 0, cZ );
        ringPos *= ringRadius;
         
        // At any point around the center of the ring
        // a sphere of radius the same as the wallRadius will fit exactly into the torus.
        // Simply get a random point in a sphere of radius wallRadius,
        // then add that to the random center point
        Vector3 sPos = Random.insideUnitSphere * wallRadius;
         
        return ( ringPos + sPos );
    }
}
