using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScienceSubModule : MonoBehaviour
{
    public GlobalInterfaceEventSystem owner;
    
    public GameObject FederationTecButton;
    public GameObject KlingonTecButton;
    public GameObject RomulanTecButton;
    public GameObject CardassianTecButton;
    public GameObject BorgTecButton;
    public GameObject UndineTecButton;

    public List<SciController> sciStations;
    public SciController curSciStation;
    public SciContract curSelectedRes;

    public GameObject SciStationButton;
    public GameObject ButtonContaner;
    public List<GameObject> ButtonsList = new List<GameObject>();

    public GameObject InfoPanel;
    public Image CurIcon;
    public Text CurName;
    public Text CurDescription;
    public Button BeginResearch;
    public Button CanselResearch;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.Players[owner.PlayerNum - 1].fedTecTree)
        {
            FederationTecButton.SetActive(true);
        }
        else
        {
            FederationTecButton.SetActive(false);
        }

        if (curSelectedRes != null)
        {
            InfoPanel.SetActive(true);
            CurIcon.sprite = curSelectedRes.Icon;
            CurName.text = curSelectedRes.Name;
            CurDescription.text = curSelectedRes.Description;
            if (curSciStation != null)
            {
                if (curSciStation.Researching)
                {
                    if (curSciStation.curResearch == curSelectedRes)
                    {
                        CanselResearch.gameObject.SetActive(true);
                        BeginResearch.gameObject.SetActive(false);
                    }
                    else
                    {
                        CanselResearch.gameObject.SetActive(false);
                        BeginResearch.gameObject.SetActive(true);
                        BeginResearch.interactable = false;
                    }
                }
                else
                {
                    if (GameManager.instance.Players[owner.PlayerNum - 1].ResearchesReady.Any(x => x == curSelectedRes))
                    {
                        CanselResearch.gameObject.SetActive(false);
                        BeginResearch.gameObject.SetActive(false);
                        return;   
                    }
                    if (GameManager.instance.Players[owner.PlayerNum - 1].ResearchesInProgress.Any(x => x == curSelectedRes))
                    {
                        CanselResearch.gameObject.SetActive(true);
                        return;
                    }
                    else
                    {
                        CanselResearch.gameObject.SetActive(false);
                    }
                    BeginResearch.gameObject.SetActive(true);
                    BeginResearch.interactable = true;
                }
            }
            else
            {
                if (GameManager.instance.Players[owner.PlayerNum - 1].ResearchesInProgress.Any(x => x == curSelectedRes))
                {
                    CanselResearch.gameObject.SetActive(true);
                }
                else
                {
                    CanselResearch.gameObject.SetActive(false);
                }
                BeginResearch.gameObject.SetActive(false);
            }
        }
        else
        {
            InfoPanel.SetActive(false);
        }
    }

    public void UpdateSciList(bool selectOne = false)
    {
        curSciStation = null;
        curSelectedRes = null;
        sciStations = new List<SciController>();
        foreach (SelectableObject obj in GameManager.instance.SelectableObjects)
        {
            if (obj.PlayerNum == owner.PlayerNum && obj.GetComponent<SciController>())
            {
                sciStations.Add(obj.GetComponent<SciController>());
            }
        }
        for (int i = 0; i < sciStations.Count; i++)
        {
            if (i > ButtonsList.Count - 1)
            {
                SciStationSelectionButton _com = Instantiate(SciStationButton, ButtonContaner.transform).GetComponent<SciStationSelectionButton>();
                _com.index = i;
                _com.sci = this;
                ButtonsList.Add(_com.gameObject);
            }
        }

        if (ButtonsList.Count > sciStations.Count)
        {
            for (int i = 0; i < ButtonsList.Count; i++)
            {
                if (i > sciStations.Count - 1)
                {
                    Destroy(ButtonsList[i]);
                    ButtonsList.RemoveAt(i);
                    i--;
                }
            }
        }

        if (selectOne)
        {
            if (sciStations.Count > 0)
            {
                curSciStation = sciStations[0];
            }
        }
    }

    public void OnResearchBeginButton()
    {
        curSciStation.BeginResearch(curSelectedRes);
    }
    public void OnResearchCanselButton()
    {
        if (curSciStation != null)
        {
            if (curSciStation.curResearch == curSelectedRes)
            {
                curSciStation.CanselResearch();
            }
            else
            {
                foreach (SciController tar in sciStations)
                {
                    if(tar.curResearch == curSelectedRes)tar.CanselResearch();
                }
            }
        }
        else
        {
            foreach (SciController tar in sciStations)
            {
                if(tar.curResearch == curSelectedRes)tar.CanselResearch();
            }
        }
    }
}