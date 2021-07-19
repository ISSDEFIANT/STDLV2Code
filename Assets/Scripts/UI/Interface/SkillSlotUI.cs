using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    public int slotNum;
    
    public GlobalInterfaceEventSystem playerInterface;

    private Image ownImage;
    private Button ownButton;

    public Image CoolDownIndicator;
    public Image SkillIcon;
    
    // Start is called before the first frame update
    void Start()
    {
        ownImage = GetComponent<Image>();
        ownButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInterface.ObjectSkills != null && playerInterface.ObjectSkills.Count > 0)
        {
            if (slotNum < playerInterface.ObjectSkills.Count)
            {
                ownButton.enabled = true;
                switch (playerInterface.ObjectSkills[slotNum].SkillType)
                {
                    case Skill.skillType.Battle:
                        ownImage.color = new Color(1,0,0,0.8f);
                        break;
                    case Skill.skillType.Research:
                        ownImage.color = new Color(0,0,1,0.8f);
                        break;
                    case Skill.skillType.Passive:
                        ownImage.color = new Color(1,0,1,0.8f);
                        break;
                }
                CoolDownIndicator.fillAmount =
                    1 - (playerInterface.ObjectSkills[slotNum].curCooldown / playerInterface.ObjectSkills[slotNum].cooldown);
                if(playerInterface.ObjectSkills[slotNum].Icon == null) return;
                SkillIcon.sprite = playerInterface.ObjectSkills[slotNum].Icon;
                SkillIcon.enabled = true;
            }
            else
            {
                ownButton.enabled = false;
                if (ownImage.color != Color.clear) ownImage.color = Color.clear;
                if (SkillIcon.color != Color.clear) SkillIcon.color = Color.clear;
                CoolDownIndicator.fillAmount = 0;
                SkillIcon.sprite = null;
                SkillIcon.enabled = false;
            }
        }
        else
        {
            ownButton.enabled = false;
            if (ownImage.color != Color.clear) ownImage.color = Color.clear;
            if (SkillIcon.color != Color.clear) SkillIcon.color = Color.clear;
            CoolDownIndicator.fillAmount = 0;
            SkillIcon.sprite = null;
            SkillIcon.enabled = false;
        }
    }

    public void OnButtonDown()
    {
        playerInterface.ObjectSkills[slotNum].onClick();
    }
}
