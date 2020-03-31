using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffectController : MonoBehaviour
{
    private SelectableObject _so;

    public GameObject DestroyedExplosions;

    public LightControlElement impulseEngineCE;

    public LightControlElement warpEngineCE;

    public LightControlElement warpCoreCE;

    // Start is called before the first frame update
    void Start()
    {
        _so = GetComponentInParent<SelectableObject>();

        if (_so.healthSystem)
        {
            if (_so._hs.SubSystems[0].gameObject.GetComponent<ImpulsEngineSS>())
            {
                impulseEngineCE.hasSystem = true;
                impulseEngineCE.SubSystem = _so._hs.SubSystems[0].gameObject.GetComponent<ImpulsEngineSS>();
            }

            if (_so._hs.SubSystems[0].gameObject.GetComponent<WarpEngineSS>())
            {
                warpEngineCE.hasSystem = true;
                warpEngineCE.SubSystem = _so._hs.SubSystems[0].gameObject.GetComponent<WarpEngineSS>();
            }

            if (_so._hs.SubSystems[0].gameObject.GetComponent<WarpCoreSS>())
            {
                warpCoreCE.hasSystem = true;
                warpCoreCE.SubSystem = _so._hs.SubSystems[0].gameObject.GetComponent<WarpCoreSS>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_so.Assimilated)
        {
            Blinking(impulseEngineCE, impulseEngineCE.BorgColors);
            Blinking(warpEngineCE, warpEngineCE.BorgColors);
            Blinking(warpCoreCE, warpCoreCE.BorgColors);
        }
        else
        {
            Blinking(impulseEngineCE, impulseEngineCE.Colors);
            Blinking(warpEngineCE, warpEngineCE.Colors);
            Blinking(warpCoreCE, warpCoreCE.Colors);
        }
        if(_so.destroyed)DestroyedExplosions.SetActive(true);
    }

    private void LateUpdate()
    {
        if (_so.Assimilated)
        {
            FixColors(impulseEngineCE, impulseEngineCE.BorgColors);
            FixColors(warpEngineCE, warpEngineCE.BorgColors);
            FixColors(warpCoreCE, warpCoreCE.BorgColors);
        }
        else
        {
            FixColors(impulseEngineCE, impulseEngineCE.Colors);
            FixColors(warpEngineCE, warpEngineCE.Colors);
            FixColors(warpCoreCE, warpCoreCE.Colors);
        }
    }

    void Blinking(LightControlElement target, LightBlinkColors colors)
    {
        if (target.hasSystem)
        {
            if (target.SubSystem.SubSystemCurHealth <= target.SubSystem.SubSystemMaxHealth / 2)
            {
                if (target.timer > 0)
                {
                    if(_so.destroyed && target.timer > 0.3f) target.timer = Random.Range(0.1f, 0.3f);
                    target.timer -= Time.deltaTime;
                }
                else
                {
                    if (target.SubSystem.SubSystemCurHealth > target.SubSystem.SubSystemMaxHealth / 5)
                    {
                        if (target.Parts.Count > 0)
                            target.Parts[Random.Range(0, target.Parts.Count)].material.color =
                                colors.DamagedColor;
                        if (target.Lights.Count > 0)
                            target.Lights[Random.Range(0, target.Lights.Count)].color =
                                colors.DamagedColor;
                    }
                    else
                    {
                        if (target.Parts.Count > 0)
                            target.Parts[Random.Range(0, target.Parts.Count)].material.color =
                                colors.HeavyDamagedColor;
                        if (target.Lights.Count > 0)
                            target.Lights[Random.Range(0, target.Lights.Count)].color =
                                colors.HeavyDamagedColor;
                    }

                    if (!_so.destroyed)
                    {
                        target.curMaxtimer = Random.Range(0.3f, target.maxTimer);
                        target.timer = target.curMaxtimer;
                    }
                    else
                    {   
                        target.curMaxtimer = Random.Range(0.1f, 0.3f);
                        target.timer = target.curMaxtimer;
                    }
                }
            }
        }
    }

    void FixColors(LightControlElement target, LightBlinkColors colors)
    {
        if(target.timer > target.curMaxtimer-0.05f)return;
        if (target.Parts.Count > 0)
        {
            foreach (Renderer p in target.Parts)
            {
                if (p.material.color != colors.NormalColor)
                    p.material.color = colors.NormalColor;
            }
        }

        if (target.Lights.Count > 0)
        {
            foreach (Light p in target.Lights)
            {
                if (p.color != colors.NormalColor)
                    p.color = colors.NormalColor;
            }
        }
    }
}