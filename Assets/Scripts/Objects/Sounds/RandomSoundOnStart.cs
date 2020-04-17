using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundOnStart : MonoBehaviour
{
    public List<AudioClip> clips;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.GetComponent<AudioSource>().clip = clips[Random.Range(0, clips.Count)];
        gameObject.GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
