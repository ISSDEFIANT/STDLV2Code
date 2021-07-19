using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModelController : MonoBehaviour
{
    public LevelInfo[] Levels;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Levels.Length; i++)
        {
            foreach (BuildAnimationScript scr in Levels[i].animScr)
            {
                if (scr.amount == 0)
                {
                    Levels[i].mainObj.SetActive(false);
                }
                else
                {
                    Levels[i].mainObj.SetActive(true);
                }

                if (scr.amount < 0) scr.amount = 0;
                if (scr.amount > 1) scr.amount = 1;
            }
        }
    }

    public void ChangeLevel(float amount, int level)
    {
        for (int i = 0; i < Levels.Length; i++)
        {
            foreach (BuildAnimationScript scr in Levels[i].animScr)
            {
                if (i == level)
                {
                    if (scr.amount < 1)
                    {
                        scr.amount = amount;
                    }
                    else
                    {
                        scr.amount = 1;
                    }
                }
                else
                {
                    if (scr.amount > 0)
                    {
                        scr.amount = 1 - amount;
                    }
                    else
                    {
                        scr.amount = 0;
                    }
                }
            }
        }
    }
}
[System.Serializable]
public class LevelInfo
{
    public GameObject mainObj;
    public BuildAnimationScript[] animScr;
}