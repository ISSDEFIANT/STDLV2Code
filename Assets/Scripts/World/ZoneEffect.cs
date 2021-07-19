using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEffect : MonoBehaviour
{
    public enum Effects
    {
        StarRadiation
    }

    public Effects Effect;
    
    public void Awake()
    {
        switch (Effect)
        {
            case Effects.StarRadiation:
                SpriteRenderer mark = Instantiate(DataLoader.Instance.ResourcesCache["LocalMinimapMark"] as GameObject, transform).GetComponent<SpriteRenderer>();
                mark.transform.localScale = new Vector3(gameObject.GetComponent<SphereCollider>().radius*2,gameObject.GetComponent<SphereCollider>().radius*2,gameObject.GetComponent<SphereCollider>().radius*2);
                mark.color = new Color(1, 0, 0, 0.5f);
                
                SpriteRenderer mark2 = Instantiate(DataLoader.Instance.ResourcesCache["LocalMinimapMark"] as GameObject, transform).GetComponent<SpriteRenderer>();
                mark2.transform.localScale = new Vector3(gameObject.GetComponent<SphereCollider>().radius*2,gameObject.GetComponent<SphereCollider>().radius*2,gameObject.GetComponent<SphereCollider>().radius*2);
                mark2.color = new Color(1, 0, 0, 0.5f);
                mark2.gameObject.layer = 14;
                break;
        }    
    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform.root.GetComponent<SelectableObject>())
        {
            other.transform.root.GetComponent<SelectableObject>().effectManager.AddEffect(Effect, transform.position);
        }
    }
}