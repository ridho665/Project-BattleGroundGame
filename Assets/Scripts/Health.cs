using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Point")]
    [SerializeField] protected float maxHp;
    [SerializeField] private float currentHp;

    public float CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            OnChangeHealth();
            if (currentHp <= 0)
            {
                Dead();
            }
        }
    }

    public bool isDead;

    [HideInInspector] public UnityEvent onDead;

    [Header("SafeZone Logic")]
    public bool isOutsideSafeZone;
    [SerializeField] private float tickTimerDamagePlayerOutsideSafezone;
    [SerializeField] private float defaultTickTimerDamagePlayerOutsideSafezone;


    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp; 
    }

    // Update is called once per frame
    void Update()
    {
        DamagePlayerOutsideSafeZone();
    }

    void DamagePlayerOutsideSafeZone()
    {
        if (isOutsideSafeZone)
        {  
            tickTimerDamagePlayerOutsideSafezone -= Time.deltaTime;
            if (tickTimerDamagePlayerOutsideSafezone <= 0)
            {
                TakeDamage(10);
                tickTimerDamagePlayerOutsideSafezone = defaultTickTimerDamagePlayerOutsideSafezone;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        CurrentHp -= damageAmount;
    }

    public virtual void OnChangeHealth()
    {

    }

    public virtual void Dead()
    {
        print("this character is dead");
        onDead?.Invoke();
        isDead = true;
        GameManager.Instance.DecreaseAliveCharacter(transform);
    }
}
