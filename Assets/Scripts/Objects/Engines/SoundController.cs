using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    public SelectableObject Owner;

    public AudioClip NormalSound;
    public AudioClip BorgSound;

    private AudioSource source;
    private ImpulsEngineSS system;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponentInChildren<ImpulsEngineSS>())
        {
            system = GetComponentInChildren<ImpulsEngineSS>();
        }
        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.clip = NormalSound;
        source.spatialBlend = 1f;
        source.maxDistance = 500;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Owner._hs != null && Owner._hs.MaxCrew > 0 && (int)Owner._hs.curCrew <= 0) || !Owner.effectManager.ImpulseEngineEffectActivity() || system.SubSystemCurHealth < system.SubSystemMaxHealth * 0.125f)
        {
           if(source.isPlaying) source.Stop();
           return;
        }
        if (Owner.Assimilated)
        {
            if (source.clip != BorgSound)
            {
                source.clip = BorgSound;
                source.Play();
            }
        }
        else
        {
            if (source.clip != NormalSound)
            {
                source.clip = NormalSound;
                source.Play();
            } 
        }

        if (Owner.captain != null && Owner.captain.Pilot != null)
        {
            if (Owner.captain.Pilot.engines.Moving)
            {
                float max;
                if (Owner.Assimilated)
                {
                    max = 1.5f;
                }
                else
                {
                    max = 2f;
                }
                if (source.pitch < max)
                {
                    if (Owner.Assimilated)
                    {
                        source.pitch += Time.deltaTime / 2;
                    }
                    else
                    {
                        source.pitch += Time.deltaTime;
                    }
                }
                else
                {
                    source.pitch = max;
                }
            }
            else
            {
                if (source.pitch > 1)
                {
                    source.pitch -= Time.deltaTime/2;
                }
                else
                {
                    source.pitch = 1;
                }
            }
        }
    }
}