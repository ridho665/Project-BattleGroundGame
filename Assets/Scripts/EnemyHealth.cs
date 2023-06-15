using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private EnemyManager enemyManager;
    [SerializeField] private Collider aliveCollider;
    [SerializeField] private Collider deadCollider;

    private void Awake() 
    {
        enemyManager = GetComponent<EnemyManager>();    
    }

    public override void Dead()
    {
        base.Dead();
        aliveCollider.enabled = false;
        deadCollider.enabled = true;
        enemyManager.enemyController.SetEnemyIsDead();
        enemyManager.enemyAnimation.animator.CrossFade(enemyManager.enemyAnimation.DEAD_ANIM, 0.2f);
    }
}
