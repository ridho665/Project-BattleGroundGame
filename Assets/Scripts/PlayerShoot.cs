using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public bool isAiming;
    [SerializeField] public GameObject rifleModel;
    public bool hasWeapon;

    [SerializeField] public GameObject aimingCam;

    private PlayerManager _playerManager;

    private void Awake() 
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    private void Update() 
    {
        AimShot();
        ShootInput();
    }

    public void OnGettingWeapon()
    {
        hasWeapon = true;
        rifleModel.SetActive(true);
        _playerManager.playerAnimation.animator.SetInteger(_playerManager.playerAnimation.WEAPON_STATE_ANIM_PARAM, 1);
    }

    void AimShot()
    {
        if (!hasWeapon) return;

        if (Input.GetMouseButtonDown(1))
        {
            _playerManager.playerAnimation.animator.SetBool(_playerManager.playerAnimation.IS_AIMING_ANIM_PARAM, true);
            _playerManager.playerAnimation.animator.CrossFade(_playerManager.playerAnimation.RIFLE_AIM_IDLE_ANIM, 0.2f);
            isAiming = true;
            aimingCam.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            _playerManager.playerAnimation.animator.SetBool(_playerManager.playerAnimation.IS_AIMING_ANIM_PARAM, false);
            isAiming = false;
            aimingCam.SetActive(false);
        }
    }

    void ShootInput()
    {
        if (!isAiming) return;

        if (Input.GetMouseButtonDown(0))
        {
            _playerManager.playerAnimation.animator.SetBool(_playerManager.playerAnimation.IS_AIMING_ANIM_PARAM, true);
            _playerManager.playerAnimation.animator.CrossFade(_playerManager.playerAnimation.FIRING_RIFLE_ANIM, 0.2f);
        }
        if (Input.GetMouseButtonUp(0))
        {
            _playerManager.playerAnimation.animator.SetBool(_playerManager.playerAnimation.IS_AIMING_ANIM_PARAM, false);
        }
    }

}
