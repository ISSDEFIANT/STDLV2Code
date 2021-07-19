using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResearchSelectionButton : MonoBehaviour
{
    public Image Selection;
    public Image ProgressBar;
    public Image IsReady;

    public ScienceSubModule sci;

    public string researchName;
    public SciContract target;
    // Start is called before the first frame update
    void Start()
    {
        switch (researchName)
        {
            case "TestResearch1":
                target = new TestResearch1();
                target.curConstructionTime = target.ConstructionTime;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.Players[sci.owner.PlayerNum - 1].ResearchesReady.Any(x => x == target))
        {
            IsReady.enabled = true;
        }
        else
        {
            IsReady.enabled = false;
        }

        if (sci.curSelectedRes != null)
        {
            Selection.enabled = sci.curSelectedRes.GetType() == target.GetType();
        }
        else
        {
            Selection.enabled = false;
        }

        ProgressBar.fillAmount = 1 - target.curConstructionTime/target.ConstructionTime;
    }

    public void OnButtonDown()
    {
        sci.curSelectedRes = target;
    }
}
