using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public Sprite Icon = null;
    
    public SelectableObject owner;
    
    public float energyCost;
    public enum skillType
    {
        Battle,
        Research,
        Passive
    }
    public enum skillTarget
    {
        Self,
        SelfPlayer,
        PlayerThroughObject,
        Object,
        ObjectsInRange,
        PositionInSpace
    }

    public skillType SkillType;
    public skillTarget SkillTarget;
    
    public SelectableObject gameTarget;
    public PlayerCameraControll playerTarget;
    public float Range;
    public Vector3 positionInSpace;
    
    public float lifeTime;
    public float curLifeTime;
    
    public float cooldown;
    public float curCooldown;

    public bool hasEffectAfterUsing;
    public ObjectEffect effectAfterUsing;

    public bool isUsing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isUsing)
        {
            if (lifeTime > 0)
            {
                if (curLifeTime > 0)
                {
                    curLifeTime -= Time.deltaTime;
                    onUning();
                }
                else
                {
                    curLifeTime = lifeTime;
                    isUsing = false;
                }
            }
            else if (lifeTime == 0)
            {
                onUning();
                isUsing = false;
            }
            else if (lifeTime == -1)
            {
                onUning();
                if (energyCost > 0)
                {
                    if (owner.healthSystem)
                    {
                        if (owner._hs.curEnergy < energyCost)
                        {
                            isUsing = false;
                            return;
                        }
                        owner._hs.curEnergy -= Time.deltaTime * energyCost;
                    }
                }
            }
        }

        if (curCooldown > 0)
        {
            curCooldown -= Time.deltaTime;
        }
    }

    public virtual void onClick()
    {
        if (curCooldown <= 0)
        {
            if (energyCost > 0)
            {
                if (owner.healthSystem)
                {
                    if (owner._hs.curEnergy < energyCost)
                    {
                        return;
                    }
                    owner._hs.curEnergy -= energyCost;
                }
            }
            isUsing = true;
            curCooldown = cooldown;
        }
    }

    public virtual void OnApplyTarget()
    {
        if (curCooldown <= 0)
        {
            if (energyCost > 0)
            {
                if (owner.healthSystem)
                {
                    if (owner._hs.curEnergy < energyCost)
                    {
                        return;
                    }
                    owner._hs.curEnergy -= energyCost;
                }
            }
            isUsing = true;
            curCooldown = cooldown;
        }
    }

    public virtual void onUning() {}
}
