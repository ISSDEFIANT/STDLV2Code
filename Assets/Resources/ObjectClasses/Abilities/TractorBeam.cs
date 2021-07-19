using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TractorBeam : Skill
{
    public float Radius;

    private TractorPoint _tp;

    private float curColorAmount;

    public bool onActive;
    
    // Start is called before the first frame update
    void Awake()
    {
        SkillType = skillType.Research;
        SkillTarget = skillTarget.Object;

        energyCost = 0;
        lifeTime = 0;

        _tp = GetComponentInChildren<TractorPoint>();
    }
    public override void onUning()
    {
	    if (!owner.effectManager.TractorBeamEffectActivity() ||
	        owner.effectManager.tractorBeam.SubSystemCurHealth <= 0) return;
        if (gameTarget != null)
		{
			if (gameTarget is ResourceSource)
			{
				ResourceSource res = gameTarget as ResourceSource;
				if (res.spawner != null)
				{
					if(res.spawner.curAsteroids.Any(x => x == res)) res.spawner.curAsteroids.Remove(res);
				}
			}
			if (gameTarget.destroyed || 
			    (gameTarget.effectManager.impulsEngine != null && (gameTarget.effectManager.ImpulseEngineEffectActivity() && gameTarget.effectManager.impulsEngine.efficiency > 0)))
			{
				isUsing = false;
				gameTarget = null;
				return;
			}

			if (gameTarget.PlayerNum != owner.PlayerNum)
			{
				if (gameTarget._hs.Shilds != null && gameTarget._hs.Shilds.Length > 0 && gameTarget._hs.Shilds[0].SubSystemCurHealth > 0)
				{
					isUsing = false;
					gameTarget = null;
					return;
				}
			}
			Vector3 LookVector = (gameTarget.transform.position - _tp.transform.position);

			_tp.transform.rotation = Quaternion.Slerp(_tp.transform.rotation, Quaternion.LookRotation(LookVector), 360);
			
			if (Vector3.Distance(gameObject.transform.position, gameTarget.transform.position) > Radius+owner.ObjectRadius+gameTarget.ObjectRadius)
			{
				isUsing = false;
				gameTarget = null;
				return;
			}
		}

        if (curColorAmount < 1)
        {
	        curColorAmount += Time.deltaTime;
        }

        if (owner.isVisible == STMethods.Visibility.Visible)
        {
	        if (!_tp.WorkingEffect.isPlaying)
	        {
		        if (!onActive)
		        {
			        _tp.OnEffect.Play();
			        onActive = true;
		        }

		        _tp.WorkingEffect.Play();
	        }
	        _tp._lbObject.gameObject.SetActive(true);
        }

        _tp._lbObject.GenerateBeam();
	    _tp._lbObject.gameObject.GetComponent<MeshRenderer>().material
		        .SetColor("_Color", Color.Lerp(_tp.OffColor, _tp.OnColor, curColorAmount));
	    _tp._lbObject.Length = Vector3.Distance(gameObject.transform.position, gameTarget.transform.position)*0.7f; 
	    _tp._lbObject.RadiusBottom = gameTarget.ObjectRadius;

	    if (Vector3.Distance(gameObject.transform.position, gameTarget.transform.position) < Radius - 7)
		{
			gameTarget.rigitBody.AddForce((gameTarget.rigitBody.velocity * (Tspeed+2) * gameTarget.rigitBody.mass) * -1);
		}
		if (Vector3.Distance(gameObject.transform.position, gameTarget.transform.position) > Radius - 7)
		{
			Vector3 LookVector = (gameTarget.transform.position - transform.position);
			gameTarget.rigitBody.AddForce(((LookVector * (Tspeed+1)) * (gameTarget.rigitBody.mass / 2)) * -1);
		}
		if (Vector3.Distance(gameObject.transform.position, gameTarget.transform.position) > Radius - 5)
		{
			Vector3 LookVector = (gameTarget.transform.position - transform.position);
			gameTarget.rigitBody.AddForce(((LookVector * (Tspeed+1)) * (gameTarget.rigitBody.mass)) * -1);
		}
    }

    public override void OnApplyTarget()
    {
        isUsing = true; 
    }

    public override void onClick()
    {
        if (!isUsing)
        {
            PlayerCameraControll player = GameManager.instance.Players[owner.PlayerNum - 1].CameraControll.GetComponent<PlayerCameraControll>();
            player.CameraState = STMethods.PlayerCameraState.OrderSetting;
            player.SetNewCommand = Captain.PlayerCommand.SettingAbilityTarget;
            player.curSelectedAbility = this;
        }
        else
        {
            isUsing = false;
            gameTarget = null;
            onActive = false;
        }
    }
    
    Vector3 lastPosition = Vector3.zero;
    private float Tspeed;
    void FixedUpdate()
    {
        if (gameTarget != null)
        {
            Tspeed = (gameTarget.transform.position - lastPosition).magnitude;
            lastPosition = gameTarget.transform.position;
        }
    }

    public override void Update()
    {
	    if (!isUsing)
	    {
		    if (curColorAmount > 0)
		    {
			    curColorAmount -= Time.deltaTime;
		    }

		    if (owner.isVisible == STMethods.Visibility.Visible)
		    {
			    if (_tp.WorkingEffect.isPlaying)
			    {
				    _tp.OffEffect.Play();
				    _tp.WorkingEffect.Stop();
			    }

			    onActive = false;
			    gameTarget = null;
			    _tp._lbObject.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color",
				    Color.Lerp(_tp.OffColor, _tp.OnColor, curColorAmount));
			    if (_tp._lbObject.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color") == _tp.OffColor)
			    {
				    _tp._lbObject.gameObject.SetActive(false);
			    }
		    }
		    else
		    {
			    if (_tp.WorkingEffect.isPlaying)
			    {
				    _tp.WorkingEffect.Stop();
			    }
			    onActive = false;
			    gameTarget = null;
			    _tp._lbObject.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", _tp.OffColor);
			    _tp._lbObject.gameObject.SetActive(false);
		    }
	    }
	    else
	    {
		    onUning();
	    }
    }
}
