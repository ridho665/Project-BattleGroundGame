using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Loot_Item_Type
{
    WEAPON_RIFLE,
    AMMO
}

public class LootItem : MonoBehaviour
{
    public Loot_Item_Type lootItemType;

    private void OnTriggerEnter(Collider other) 
    {
        switch (lootItemType)
        {
            case Loot_Item_Type.WEAPON_RIFLE:
            if (other.GetComponent<CharacterTag>())
                {
                    //Kasih weapon ke other
                    //destroy diri sendiri
                    if (other.TryGetComponent(out PlayerShoot playerShoot))
                    {
                        playerShoot.OnGettingWeapon();
                        
                        Destroy(gameObject);  
                    }
                    if (other.TryGetComponent(out EnemyController enemyController))
                    {
                        enemyController.SetLookingForEnemyState();
                        Destroy(gameObject);  
                    }
                }
                break;
            case Loot_Item_Type.AMMO:
            if (other.GetComponent<PlayerShoot>())
            {
                other.GetComponent<PlayerShoot>().GetAmmo();
                Destroy(gameObject);
            }
            break;    
        }         
    }
}
