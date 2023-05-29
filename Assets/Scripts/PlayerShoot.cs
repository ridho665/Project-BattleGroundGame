using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Player State")]
    public bool isAiming;
    public bool hasWeapon;

    [Header("Cameras")]
    [SerializeField] public GameObject aimingCam;
    [SerializeField] private Transform mainCamera;

    [Header("Shooting Logic")]
    [SerializeField] public GameObject rifleModel;
    [SerializeField] private float shootingRange;
    [SerializeField] private float fireRateCd;
    [SerializeField] private float rifleAttackDamage;
    [SerializeField] private bool isAttackCd;

    [Header("Visual Effects")]
    [SerializeField] private GameObject muzzleFlashVfx;
    [SerializeField] private GameObject bulletImpactVfx;
    [SerializeField] private GameObject bloodImpactVfx;

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
            muzzleFlashVfx.SetActive(false);
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
            muzzleFlashVfx.SetActive(false);
            _playerManager.playerAnimation.animator.SetBool(_playerManager.playerAnimation.IS_AIMING_ANIM_PARAM, false);
        }
        if (Input.GetMouseButton(0))
        {
            Shoot();  
        }
    }

    IEnumerator ShootCoroutine()
    {
        isAttackCd = true;
        yield return new WaitForSeconds(fireRateCd); 
        isAttackCd = false;
    }
    void Shoot()
    {
        if (isAttackCd) return;

        muzzleFlashVfx.SetActive(true);
        StartCoroutine(ShootCoroutine());
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hit, shootingRange))
        {
            GameObject effect = null;
            if (hit.collider.GetComponent<Health>())
            {
                hit.collider.GetComponent<Health>().TakeDamage(rifleAttackDamage);
                effect = Instantiate(bloodImpactVfx, hit.point, Quaternion.identity);
            }
            else
            {
                effect = Instantiate(bulletImpactVfx, hit.point, Quaternion.identity);
            } 
            effect.transform.LookAt(transform);
            print(hit.collider.gameObject.name);
        }
    }

}
