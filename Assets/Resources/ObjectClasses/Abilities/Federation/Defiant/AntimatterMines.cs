using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntimatterMines : Skill
{
    public GameObject Mines;
    // Start is called before the first frame update
    void Awake()
    {
        Icon = DataLoader.Instance.ResourcesCache["Defiant/Mines/Icon"] as Sprite;
        
        Mines = DataLoader.Instance.ResourcesCache["Defiant/Mines"] as GameObject;
        
        SkillType = skillType.Battle;
        SkillTarget = skillTarget.PositionInSpace;

        cooldown = 180;
        energyCost = 75;
        lifeTime = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        positionInSpace = transform.position;
    }

    public override void onUning()
    {
        StartCoroutine("SpawnMines");
    }

    IEnumerator SpawnMines()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 rotateVector;
            if ((i % 2) == 0)
            {
                rotateVector = positionInSpace + (transform.rotation * new Vector3(-0.3f, -0.1f, -3));
            }
            else
            {
                rotateVector = positionInSpace + (transform.rotation * new Vector3(0.3f, -0.1f, -3));
            }

            Shell mine = Instantiate(Mines, rotateVector, this.transform.rotation).GetComponent<Shell>();
            mine.PlayerNum = owner.PlayerNum;
            mine.owner = owner.gameObject;
            if ((i % 2) != 0)
            {
                yield return new WaitForSeconds(.5f);
            }
        }
        yield return null;
    }
}
