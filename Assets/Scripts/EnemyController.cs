using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ENEMY_STATE
{
    LOOKING_FOR_WEAPON,
    LOOKING_FOR_ENEMY,
    ATTACKING_ENEMY,
    IS_DEAD
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] private ENEMY_STATE _enemyState;
    [SerializeField] private float movementSpeed;

    private NavMeshAgent navMeshAgent;

    private Health enemyTargetHealth;
    public Transform enemyTarget;
    private Transform weaponLootParent;
    private Transform characterParent;


    [SerializeField] private GameObject rifleModel;
    [SerializeField] private GameObject muzzleFlashVfx;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform shotPoint;
    [SerializeField] private float startShootingRange;
    [SerializeField] private float stopShootingRange;

    [SerializeField] private float shootingCd;
    [SerializeField] private float defaultShootingCd;



    private EnemyManager _enemyManager;

    private void Awake() 
    {
        _enemyManager = GetComponent<EnemyManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = movementSpeed;    
    }

    private void Start() 
    {
        weaponLootParent = GameManager.Instance.weaponLootParent;
        characterParent = GameManager.Instance.characterParent;

        _enemyState = ENEMY_STATE.LOOKING_FOR_WEAPON;    
    }

    private void Update() 
    {
        if (_enemyManager.enemyHealth.isDead) return;
        switch (_enemyState)
        {
            case ENEMY_STATE.LOOKING_FOR_WEAPON:
                try
                {
                    enemyTarget = FindNearestTarget(weaponLootParent).transform;
                    navMeshAgent.destination = enemyTarget.position;    
                }
                catch (System.Exception)
                {
                    print("Target not found");
                }
                //Logic nyari senjata
                break;
            case ENEMY_STATE.LOOKING_FOR_ENEMY:
                try
                {
                    enemyTarget = FindNearestTarget(characterParent).transform;
                    navMeshAgent.destination = enemyTarget.position;
                    if (Vector3.Distance(transform.position, enemyTarget.position) < startShootingRange)
                    {
                        SetAttackingEnemyState();
                    }                   
                }
                catch (System.Exception)
                {
                    print("Target not found");
                }
                //Logic nyari lawan
                break;
            case ENEMY_STATE.ATTACKING_ENEMY:
                Shoot();
                if (Vector3.Distance(transform.position, enemyTarget.position) > stopShootingRange)
                    {
                        SetLookingForEnemyState();
                    } 
                //Sudah mendapatkan lawan terdekat
                //Dan lagi nembakin lawannya
                break;
            case ENEMY_STATE.IS_DEAD:
                //Ketika characternya mati, Kita mau ngapain?
                break;
        }                                                                                                                      
    }

    public void SetLookingForEnemyState()
    {
        
        if (enemyTarget != null)
        {
            if (enemyTarget.TryGetComponent(out Health health))
            {
                enemyTargetHealth = health;
                enemyTargetHealth.onDead.AddListener(TargetIsDead);
            }
        }

        _enemyState = ENEMY_STATE.LOOKING_FOR_ENEMY;
        rifleModel.SetActive(true);
        _enemyManager.enemyAnimation.animator.CrossFade(_enemyManager.enemyAnimation.RIFLE_RUN_ANIM, 0.2f);
        muzzleFlashVfx.SetActive(false);
        navMeshAgent.isStopped = false;
    }

    public void SetAttackingEnemyState()
    {
        if (_enemyManager.enemyHealth.isDead) return;

        enemyTargetHealth = enemyTarget.GetComponent<Health>();
        enemyTargetHealth.onDead.AddListener(TargetIsDead);

        _enemyState = ENEMY_STATE.ATTACKING_ENEMY;
        rifleModel.SetActive(true);
        _enemyManager.enemyAnimation.animator.CrossFade(_enemyManager.enemyAnimation.FIRING_RIFLE_ANIM, 0.2f);
        muzzleFlashVfx.SetActive(true);
        navMeshAgent.isStopped = true;
    }

    public void SetEnemyIsDead()
    {
        // if (_enemyManager.enemyHealth.isDead) return;

        if (enemyTargetHealth != null) enemyTargetHealth.onDead.RemoveListener(TargetIsDead);
        _enemyState = ENEMY_STATE.IS_DEAD;
        rifleModel.SetActive(true);
        muzzleFlashVfx.SetActive(false);
        navMeshAgent.isStopped = true;
    }

    void TargetIsDead()
    {
        enemyTargetHealth.onDead.RemoveListener(TargetIsDead);
        SetLookingForEnemyState();
    }

    GameObject FindNearestTarget(Transform targetParent)
    {
        var distanceNearestTarget = Mathf.Infinity;
        GameObject nearestTarget = null;

        foreach (Transform target in targetParent)
        {
            if (target == transform) continue;
            
            if (target.TryGetComponent(out Health health))
            {
                if (health.isDead) continue;
            }

            var distanceCurrentTarget = (target.transform.position - transform.position).sqrMagnitude;

            if (distanceCurrentTarget < distanceNearestTarget)
            {
                distanceNearestTarget = distanceCurrentTarget;
                nearestTarget = target.gameObject;
            }
        }

        return nearestTarget;
    }

    void Shoot()
    {
        transform.LookAt(enemyTarget);
        shootingCd -= Time.deltaTime;
        if (shootingCd <= 0)
        {
            shootingCd = defaultShootingCd;
            Instantiate(bulletPrefab, shotPoint.position, shotPoint.rotation);
        }
        
    }
}
