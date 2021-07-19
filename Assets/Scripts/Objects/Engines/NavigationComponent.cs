using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationComponent : MonoBehaviour
{
    public bool StartPointUnavaible;
    public bool CanMoveEndPoint;
    public bool isNavigating;
    public bool isUsing;
    
    public List<Vector3> positions;

    private Engines engines;
    public SelectableObject hitObj;

    public List<Mobile> FleetList;
    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector3>();
        engines = GetComponent<Engines>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int layerMask = 1 << 9;
        RaycastHit hit;

        if (!engines.Moving)
        {
            isUsing = false;
            StartPointUnavaible = false;
        }
        if (isUsing && !isNavigating)
        {
            if (positions.Count == 1)
            {
                if (Physics.Raycast(transform.position, positions[0] - transform.position, out hit, Vector3.Distance(transform.position, positions[0]), layerMask))
                {
                    if(FleetList != null && FleetList.Any(x => x == hit.transform.root.GetComponent<Mobile>())) return;
                    isNavigating = true;
                }
            }
            if (positions.Count > 1)
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    if (i == 0)
                    {
                        if (Physics.Raycast(transform.position, positions[0] - transform.position, out hit, Vector3.Distance(transform.position, positions[0]), layerMask))
                        {
                            if(FleetList != null && FleetList.Any(x => x == hit.transform.root.GetComponent<Mobile>())) return;
                            Vector3 finishPoint = positions[positions.Count - 1];
                            positions.Clear();
                            positions.Add(finishPoint);
                            isNavigating = true;
                        }
                        continue;
                    }
                    if (Physics.Raycast(positions[i-1], positions[i] - positions[i-1], out hit, Vector3.Distance(positions[i-1], positions[i]), layerMask))
                    {
                        if(FleetList != null && FleetList.Any(x => x == hit.transform.root.GetComponent<Mobile>())) return;
                        positions.RemoveRange(i, positions.Count-(i+1));
                        isNavigating = true;
                    }
                }
            }

            if (positions.Count > 2)
            {
                for (int i = 0; i < positions.Count-1;)
                {
                    if (i == 0)
                    {
                        if (Physics.Raycast(transform.position, positions[1] - transform.position, out hit, Vector3.Distance(transform.position, positions[1]), layerMask))
                        {
                            if (FleetList != null && FleetList.Any(x => x == hit.transform.root.GetComponent<Mobile>()))
                            {
                                positions.RemoveAt(0);
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            positions.RemoveAt(0);
                        }
                    }
                    else
                    {
                        if (!Physics.Raycast(positions[i - 1], positions[i + 1] - positions[i - 1], out hit,
                            Vector3.Distance(positions[i - 1], positions[i + 1]), layerMask))
                        {
                            if (FleetList != null && FleetList.Any(x => x == hit.transform.root.GetComponent<Mobile>()))
                            {
                                positions.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            positions.RemoveAt(i);
                        }
                    }
                }
            }
        }
        
        if (!isNavigating) return;
        if (positions.Count == 1)
        {
            if (Physics.Raycast(transform.position, positions[0] - transform.position, out hit, Vector3.Distance(transform.position, positions[0]), layerMask))
            {
                if (FleetList != null && FleetList.Any(x => x == hit.transform.root.GetComponent<Mobile>()))
                {
                    isNavigating = false;
                    return;
                }
                
                hitObj = hit.transform.root.GetComponent<SelectableObject>();
                if (hit.transform.root == transform || 
                    (!CanMoveEndPoint && (Vector3.Distance(positions[0], hitObj.transform.position) < hitObj.ObjectRadius)))
                {
                    isNavigating = false;
                    return;
                }
                if (Vector3.Distance(hitObj.transform.position, positions[0]) < hitObj.ObjectRadius + engines.Owner.ObjectRadius)
                {
                    if (CanMoveEndPoint)
                    {
                        if (Vector3.Distance(transform.position, hitObj.transform.position) >
                            Vector3.Distance(transform.position, positions[0]))
                        {
                            StartPointUnavaible = true;
                            positions[0] = (positions[0] - transform.position).normalized *
                                           (Vector3.Distance(transform.position, hitObj.transform.position) -
                                            (hitObj.ObjectRadius + (engines.Owner.ObjectRadius * 2)));
                        }
                        else
                        {
                            StartPointUnavaible = true;
                            positions[0] = (positions[0] - transform.position).normalized *
                                           (Vector3.Distance(transform.position, hitObj.transform.position) +
                                            (hitObj.ObjectRadius + (engines.Owner.ObjectRadius * 2)));
                        }
                    }
                }
                addPoint(transform.position, 0);
            }
            else
            {
                isNavigating = false;
            }
        }
        if (positions.Count > 1)
        {
            if (Physics.Raycast(positions[positions.Count-2], positions[positions.Count-1] - positions[positions.Count-2], out hit, Vector3.Distance(positions[positions.Count-2], positions[positions.Count-1]), layerMask))
            {
                if (FleetList != null && FleetList.Any(x => x == hit.transform.root.GetComponent<Mobile>()))
                {
                    isNavigating = false;
                    return;
                }
                hitObj = hit.transform.root.GetComponent<SelectableObject>();
                addPoint(positions[positions.Count-2], positions.Count-1);
            }
            else
            {
                isNavigating = false;
            }
        }
    }

    void addPoint(Vector3 originPoint, int ineration)
    {
        if(hitObj == engines.Owner) return;

        float radius = hitObj.ObjectRadius;
        float sradius = radius * radius;
        float distance = Vector3.Distance(originPoint, hitObj.transform.position);
        float HPoint = sradius / distance;

        Vector3 lv = (hitObj.transform.position - originPoint).normalized;
        Vector3 h = hitObj.transform.position + lv * HPoint;

        float hc = Mathf.Sqrt(sradius - ((sradius / distance) * (sradius / distance)));

        float horizontalAngle;
        float verticalAngle;
        
        Vector3 c1 = h + Vector3.Cross(lv, new Vector3(0, 1, 0)) * hc;
        Vector3 c2 = h + Vector3.Cross(lv, new Vector3(0, -1, 0)) * hc;
        Vector3 c3 = h + Vector3.Cross(lv, new Vector3(0, 0, 1)) * hc;
        Vector3 c4 = h + Vector3.Cross(lv, new Vector3(0, 0, -1)) * hc;

        Vector3 wchor;
        Vector3 wcver;

        if (Vector3.Angle(c1 - originPoint, Vector3.forward) < Vector3.Angle(c2 - originPoint, Vector3.forward))
        {
            wchor = c1;
            horizontalAngle = Vector3.Angle(c1 - originPoint, Vector3.forward);
        }
        else
        {
            wchor = c2;
            horizontalAngle = Vector3.Angle(c2 - originPoint, Vector3.forward);
        }
        if (Vector3.Angle(c3 - originPoint, Vector3.forward) < Vector3.Angle(c4 - originPoint, Vector3.forward))
        {
            wcver = c3;
            verticalAngle = Vector3.Angle(c3 - originPoint, Vector3.forward);
        }
        else
        {
            wcver = c4;
            verticalAngle = Vector3.Angle(c4 - originPoint, Vector3.forward);
        }
        
        Vector3 wcfinal;
        
        if (horizontalAngle < verticalAngle)
        {
            wcfinal = wchor;
        }
        else
        {
            wcfinal = wcver;
        }
        
        Vector3 final = hitObj.transform.position + (wcfinal - hitObj.transform.position).normalized * (hitObj.ObjectRadius + (engines.Owner.ObjectRadius * 2));
        positions.Insert(ineration, final);
    }

    public void Navigate(Vector3 tar, bool canMoveEndPoint = false, List<Mobile> fleet = null)
    {
        CanMoveEndPoint = canMoveEndPoint;
        positions.Clear();
        positions.Add(tar);
        isNavigating = true;
        StartPointUnavaible = false;

        FleetList = fleet;
    }
}