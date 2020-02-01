using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{


    [SerializeField] private Transform weaponPlaceInPlayer;
    [SerializeField] private string weaponTag="Weapon";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(weaponTag))
        {
         Weapon weaponScript=  other.GetComponent<Weapon>();
         if (!weaponScript.isAttached)
         {
             ParentWeaponToPlayer(weaponScript);
         }
        }
    }

    void ParentWeaponToPlayer(Weapon weapon)
    {
        weapon.transform.parent = weaponPlaceInPlayer.transform;
        weapon.isAttached = true;
    }
}
