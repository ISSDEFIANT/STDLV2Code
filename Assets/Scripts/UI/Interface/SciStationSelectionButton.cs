using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SciStationSelectionButton : MonoBehaviour
{
    public Image Icon;
    public Image Selection;
    public Image ProgressBar;

    public ScienceSubModule sci;

    public int index;
    // Start is called before the first frame update
    void Start()
    {
        sci = gameObject.GetComponentInParent<ScienceSubModule>();
    }

    // Update is called once per frame
    void Update()
    {
        Icon.sprite = sci.sciStations[index].system.Owner.ObjectIcon;
        Selection.enabled = sci.curSciStation == sci.sciStations[index];
        if (sci.sciStations[index].Researching)
        {
            ProgressBar.fillAmount = 1 - sci.sciStations[index].curResearch.curConstructionTime/sci.sciStations[index].curResearch.ConstructionTime;
        }
        else
        {
            ProgressBar.fillAmount = 0;
        }
    }

    public void OnButtonDown()
    {
        sci.curSciStation = sci.sciStations[index];
    }
}
