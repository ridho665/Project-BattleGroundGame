using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LootItemType
{
    RIFLE,
    AMMO
}

public class LootItem : MonoBehaviour
{
    public LootItemType lootItemType;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.GetComponent<CharacterTag>())
        {
            if (other.TryGetComponent(out PlayerShoot playerShoot))
            {
                playerShoot.OnGettingWeapon();
                //Kasih weapon ke other
                //destroy diri sendiri
                Destroy(gameObject);  
            }
            
        }
                 
    }
}
