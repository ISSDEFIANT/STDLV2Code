using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UnitsMarksController : MonoBehaviour
{
    [HideInInspector]
    public GameObject LocalMinimapMark;
    [HideInInspector]
    public GameObject GlobalMinimapMark;

    public List<SpriteRenderer> LocalMarks;
    
    public List<GlobalMinimapMark> GlobalMarks;
    
    private float groupReloadTimer = 0.5f;

    public GlobalMinimapController _gmc;
    public PlayerCameraControll _pcc;

    private GlobalMinimapMark lastHovered;

    #region Filters

    public bool NamesVisibile = true;
    public bool ShipsVisible = true;
    public bool StationsVisible = true;
    public bool OtherStationsVisible = true;
    public bool ObjectsInProgressVisible = true;
    public bool StarbaseVisible = true;
    public bool ConstructionStationsVisible = true;
    public bool DefenceStationsVisible = true;
    public bool MiningStationsVisible = true;
    public bool ScienceStationsVisible = true;
    public bool UnknownVisible = true;
    public bool StarsVisible = true;
    public bool PlanetsVisible = true;
    public bool AsteroidsVisible = true;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        LocalMinimapMark = DataLoader.Instance.ResourcesCache["LocalMinimapMark"] as GameObject;
        GlobalMinimapMark = DataLoader.Instance.ResourcesCache["GlobalMinimapMark"] as GameObject;

        _gmc = FindObjectOfType<GlobalMinimapController>();
        _pcc = gameObject.GetComponent<PlayerCameraControll>();
    }

    List<claster> ObjectGrouping()
    {
        List<SelectableObject> objects = new List<SelectableObject>();
        
        List<claster> localclasters = new List<claster>();
        
        List<int> usedIndexes = new List<int>();

        foreach (SelectableObject objs in GameManager.instance.SelectableObjects)
        {
            objects.Add(objs);
        }

        for (int i = 0; i < _pcc.Fleets.fleets.Length; i++)
        {
            if(_pcc.Fleets.fleets[i] == null || !_pcc.Fleets.fleets[i].Any()) continue;

            List<int> fleetUsedIndexes = new List<int>();
            
            for (int j = 0; j < _pcc.Fleets.fleets[i].Count; j++)
            {
                if(fleetUsedIndexes.Any(x => x == j)) continue;
                
                claster claster = new claster();
                claster.members = new List<SelectableObject>();
                claster.members.Add(_pcc.Fleets.fleets[i][j]);
                if (i == 0)
                {
                    claster.groupName = GameManager.instance.Players[_pcc.PlayerNum - 1].GroupsNames[9];
                }
                else
                {
                    claster.groupName = GameManager.instance.Players[_pcc.PlayerNum - 1].GroupsNames[i-1];
                }
                fleetUsedIndexes.Add(j);
                
                for (int k = 0; k < _pcc.Fleets.fleets[i].Count; k++)
                {
                    if(fleetUsedIndexes.Any(x => x == k)) continue;
                    if (Vector3.Distance(_pcc.Fleets.fleets[i][j].transform.position, _pcc.Fleets.fleets[i][k].transform.position) < Mathf.Lerp(25, 1000, _gmc.tarCamera.orthographicSize / _gmc.maxMinimapScale))
                    {
                        claster.members.Add(_pcc.Fleets.fleets[i][k]);
                        fleetUsedIndexes.Add(k);
                    }
                }
                
                for (int k = 0; k < claster.members.Count; k++)
                {
                    for (int l = 0; l < objects.Count; l++)
                    {
                        if (claster.members[k] == objects[l])
                        {
                            usedIndexes.Add(l);
                        }
                    }
                }
                localclasters.Add(claster);
            }
        }

        for (int i = 0; i < objects.Count; i++)
        {
            if(usedIndexes.Any(x => x == i)) continue;
            claster claster = new claster();
            claster.members = new List<SelectableObject>();
            claster.members.Add(objects[i]);
            usedIndexes.Add(i);
            for (int j = 0; j < objects.Count; j++)
            {
                if(usedIndexes.Any(x => x == j) || objects[i] == objects[j]) continue;
                if (objects[i].PlayerNum == objects[j].PlayerNum && ((objects[i] is Mobile && objects[j] is Mobile) || (objects[i] is Static && objects[j] is Static || objects[i] is Static && objects[j] is ObjectUnderConstruction || objects[i] is ObjectUnderConstruction && objects[j] is Static || objects[i] is ObjectUnderConstruction && objects[j] is ObjectUnderConstruction)) && Vector3.Distance(objects[i].transform.position, objects[j].transform.position) < Mathf.Lerp(25, 1000, _gmc.tarCamera.orthographicSize / _gmc.maxMinimapScale))
                {
                    claster.members.Add(objects[j]);
                    usedIndexes.Add(j);
                }
            }
            localclasters.Add(claster);
        }
        return localclasters;
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        if (GameManager.instance.SelectableObjects.Count > 0)
        {
            LocalMarkMethod();
            GlobalMarkMethod();
        }
        else
        {
            if (LocalMarks.Count > 0)
            {
                foreach (SpriteRenderer sp in LocalMarks)
                {
                    sp.gameObject.SetActive(false);
                }
            }
            if (GlobalMarks.Count > 0)
            {
                foreach (GlobalMinimapMark gmm in GlobalMarks)
                {
                    gmm.gameObject.SetActive(false);
                }
            }
        }
    }

    private void LocalMarkMethod()
    {
        if (LocalMarks.Count > GameManager.instance.SelectableObjects.Count)
        {
            for (int i = GameManager.instance.SelectableObjects.Count; i < LocalMarks.Count; i++)
            {
                LocalMarks[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < GameManager.instance.SelectableObjects.Count; i++)
        {
            if (LocalMarks.Count < i + 1)
            {
                LocalMarks.Add(Instantiate(LocalMinimapMark).GetComponent<SpriteRenderer>());
            }

            SelectableObject tar = GameManager.instance.SelectableObjects[i];
            if (tar.destroyed) return;
            if (tar.isVisible == STMethods.Visibility.Visible)
            {
                if (tar.PlayerNum > 0)
                {
                    LocalMarks[i].color = GameManager.instance.Players[tar.PlayerNum - 1].PlayerColor;
                }
                else
                {
                    LocalMarks[i].color = Color.gray;
                }

                if (tar.ObjectRadius > 10)
                {
                    LocalMarks[i].gameObject.transform.localScale =
                        new Vector3(tar.ObjectRadius, tar.ObjectRadius, tar.ObjectRadius);
                }
                else
                {
                    LocalMarks[i].gameObject.transform.localScale = new Vector3(25, 25, 25);
                }

                LocalMarks[i].gameObject.transform.position = tar.transform.position;
                if (!LocalMarks[i].gameObject.activeSelf) LocalMarks[i].gameObject.SetActive(true);
            }

            if (tar.isVisible == STMethods.Visibility.Invisible)
            {
                if (LocalMarks[i].gameObject.activeSelf) LocalMarks[i].gameObject.SetActive(false);
            }

            if (tar.isVisible == STMethods.Visibility.FarNoise)
            {
                LocalMarks[i].color = Color.gray;
                LocalMarks[i].gameObject.transform.localScale = new Vector3(10, 10, 10);
                LocalMarks[i].gameObject.transform.position = tar.transform.position;
                if (!LocalMarks[i].gameObject.activeSelf) LocalMarks[i].gameObject.SetActive(true);
            }

            if (tar.isVisible == STMethods.Visibility.NearNoise)
            {
                LocalMarks[i].color = Color.gray;
                if (tar.ObjectRadius > 10)
                {
                    LocalMarks[i].gameObject.transform.localScale =
                        new Vector3(tar.ObjectRadius, tar.ObjectRadius, tar.ObjectRadius);
                }
                else
                {
                    LocalMarks[i].gameObject.transform.localScale = new Vector3(10, 10, 10);
                }

                LocalMarks[i].gameObject.transform.position = tar.transform.position;
                if (!LocalMarks[i].gameObject.activeSelf) LocalMarks[i].gameObject.SetActive(true);
            }
        }
    }
    private void GlobalMarkMethod()
    {
        if (_pcc.globalInterface.globalStatus == GlobalInterfaceEventSystem.globalInterfaceStatus.GlobalMinimap)
        {
            Ray ray = _gmc.tarCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layer = 1 << 14;
            if (Physics.Raycast(ray, out hit, 600, layer))
            {
                if (lastHovered == null)
                {
                    lastHovered = hit.transform.GetComponent<GlobalMinimapMark>();
                    lastHovered.Hovered = true;
                }
                if(hit.transform != lastHovered.transform)
                {
                    lastHovered.Hovered = false;
                    lastHovered = hit.transform.GetComponent<GlobalMinimapMark>();
                    lastHovered.Hovered = true;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    _pcc.SelectionList = lastHovered.curClaster;
                }
            }
            else
            {
                if (lastHovered != null)
                {
                    lastHovered.Hovered = false;
                    lastHovered = null;
                }
            }
        }

        List<claster> Clasters = ObjectGrouping();

        if (GlobalMarks.Count > Clasters.Count)
        {
            for (int i = Clasters.Count; i < GlobalMarks.Count; i++)
            {
                GlobalMarks[i].gameObject.SetActive(false);
            }
        }

        if(Clasters.Count <= 0) return;

        for (int i = 0; i < Clasters.Count; i++)
        {
            if (GlobalMarks.Count < i + 1)
            {
                GlobalMarks.Add(Instantiate(GlobalMinimapMark).GetComponent<GlobalMinimapMark>());
                GlobalMarks.Last()._umc = this;
            }

            List<SelectableObject> tar = Clasters[i].members;

            Vector3 position = new Vector3();
            
            if (tar.Count > 0)
            {
                for (int j = 0; j < tar.Count;)
                {
                    if (!ShipsVisible && tar[j] is Mobile)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!StationsVisible && (tar[j] is Static || tar[j] is ObjectUnderConstruction))
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!OtherStationsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.Station)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!ObjectsInProgressVisible && tar[j] is ObjectUnderConstruction)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!StarbaseVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.Starbase)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!ConstructionStationsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.ConstructionStation)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!DefenceStationsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.DefenceStation)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!MiningStationsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.MiningStation)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!ScienceStationsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.SciStation)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!UnknownVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.Unknown)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!StarsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.Star)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!PlanetsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.Planet)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    if (!AsteroidsVisible && tar[j].GlobalMinimapRender == global::GlobalMinimapMark.ShowingStats.Asteroid)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    
                    if (tar[j].destroyed || tar[j].isVisible == STMethods.Visibility.Invisible)
                    {
                        tar.RemoveAt(j);
                        continue;
                    }
                    
                    position.x += tar[j].transform.position.x;
                    position.z += tar[j].transform.position.z;
                    j++;
                }
                position.x /= tar.Count;
                position.z /= tar.Count;
            }
            
            if(tar.Count <= 0)
            {
                GlobalMarks[i].gameObject.SetActive(false);
                continue;
            }

            float scale = Mathf.Lerp(5, 100, _gmc.tarCamera.orthographicSize / _gmc.maxMinimapScale);
            
            GlobalMarks[i].transform.localScale = new Vector3(scale,scale,scale);
            GlobalMarks[i].transform.position = position;
            GlobalMarks[i].gameObject.SetActive(true);
            GlobalMarks[i].UpdateData(Clasters[i].members, NamesVisibile, Clasters[i].groupName);
        }
    }
    
    private class claster
    {
        public List<SelectableObject> members;
        public string groupName;
    }
}