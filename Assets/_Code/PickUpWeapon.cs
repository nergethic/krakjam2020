﻿using UnityEngine;

public class PickUpWeapon : MonoBehaviour {
    [SerializeField] Transform weaponPlaceInPlayer;
    [SerializeField] string weaponTag="Weapon";
    [SerializeField] PlayerController playerController;
    [SerializeField] DynamicCamera camera;

    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals(weaponTag)) {
            //Debug.Log("Enter");
         Weapon weaponScript = other.GetComponent<Weapon>();
         if (weaponScript != null && !weaponScript.isAttached) {
             var rb = weaponScript.gameObject.GetComponent<Rigidbody>();
             weaponScript.GetCollisionCollider().isTrigger = true;
             rb.isKinematic = true;
             playerController.AnimateEquippingGun();
             weaponScript.SetAttached(true);
             camera.ChangeCameraMode();
             ParentWeaponToPlayer(weaponScript);
         }
        }
    }

    void ParentWeaponToPlayer(Weapon weapon)
    {
      var weaponTransform= weapon.transform;
      weaponTransform.position = weaponPlaceInPlayer.position;
      weaponTransform.rotation = weaponPlaceInPlayer.rotation;
      weaponTransform.parent = weaponPlaceInPlayer;
        weapon.isAttached = true;
    }
}
