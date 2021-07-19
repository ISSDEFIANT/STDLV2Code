using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaserSheaf : Skill
{
    public PhaserSheafController Sheaf;
    // Start is called before the first frame update
    void Awake()
    {
        Sheaf = gameObject.GetComponentInChildren<PhaserSheafController>();
        
        SkillType = skillType.Battle;
        SkillTarget = skillTarget.Object;

        cooldown = 80;
        energyCost = 100;
        lifeTime = 0;
    }
    public override void Update()
    {
        base.Update();
    }
    public override void onUning()
    {
        if (gameTarget == null) return;
        if (owner.isVisible == STMethods.Visibility.Visible)
        {
            Sheaf.StartCoroutine("AttackSeqience", gameTarget.transform);
        }

        if (gameTarget.healthSystem)
        {
            if (gameTarget._hs.Shilds != null && gameTarget._hs.Shilds.Length > 0 && gameTarget._hs.Shilds[0].SubSystemCurHealth > 0)
            {
                gameTarget._hs.Shilds[0].SubSystemCurHealth -= 300;
                return;
            }
            switch (owner.captain.Gunner.Aiming)
            {
                case STMethods.AttackType.NormalAttack:
                    gameTarget._hs.curHull -= 300;
                    break;
                case STMethods.AttackType.PrimaryWeaponSystemAttack:
                    if (gameTarget.effectManager.primaryWeapon == null) return;
                    gameTarget.effectManager.primaryWeapon.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.primaryWeapon.ChangeEfficiency();
                    break;
                case STMethods.AttackType.SecondaryWeaponSystemAttack:
                    if (gameTarget.effectManager.secondaryWeapon == null) return;
                    gameTarget.effectManager.secondaryWeapon.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.secondaryWeapon.ChangeEfficiency();
                    break;
                case STMethods.AttackType.ImpulseSystemAttack:
                    if (gameTarget.effectManager.impulsEngine == null) return;
                    gameTarget.effectManager.impulsEngine.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.impulsEngine.ChangeEfficiency();
                    break;
                case STMethods.AttackType.WarpEngingSystemAttack:
                    if (gameTarget.effectManager.warpEngine == null) return;
                    gameTarget.effectManager.warpEngine.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.warpEngine.ChangeEfficiency();
                    break;
                case STMethods.AttackType.WarpCoreAttack:
                    if (gameTarget.effectManager.warpCore == null) return;
                    gameTarget.effectManager.warpCore.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.warpCore.ChangeEfficiency();
                    break;
                case STMethods.AttackType.LifeSupportSystemAttack:
                    if (gameTarget.effectManager.lifeSupport == null) return;
                    gameTarget.effectManager.lifeSupport.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.lifeSupport.ChangeEfficiency();
                    break;
                case STMethods.AttackType.SensorsSystemAttack:
                    if (gameTarget.effectManager.sensor == null) return;
                    gameTarget.effectManager.sensor.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.sensor.ChangeEfficiency();
                    break;
                case STMethods.AttackType.TractorBeamSystemAttack:
                    if (gameTarget.effectManager.tractorBeam == null) return;
                    gameTarget.effectManager.tractorBeam.SubSystemCurHealth -= 300;
                    gameTarget.effectManager.tractorBeam.ChangeEfficiency();
                    break;
            }
        }
    }
    
    public override void OnApplyTarget()
    {
        base.OnApplyTarget();
    }

    public override void onClick()
    {
        PlayerCameraControll player = GameManager.instance.Players[owner.PlayerNum - 1].CameraControll.GetComponent<PlayerCameraControll>();
        player.CameraState = STMethods.PlayerCameraState.OrderSetting;
        player.SetNewCommand = Captain.PlayerCommand.SettingAbilityTarget;
        player.curSelectedAbility = this;
    }
}