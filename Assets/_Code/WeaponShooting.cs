﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.InputSystem;

public class WeaponShooting : MonoBehaviour { 
    [SerializeField] int shootButton;
    [SerializeField] string enemyPlayerTag;
    [SerializeField] Transform camera;
   
   Coroutine shootFxCor;
   int layerMask;
  // [SerializeField] private PlayerController playerController;
   private void Start()
   {
        layerMask = 1 << LayerMask.NameToLayer("Player"); // only check for collisions with layerX
    //   Transform camera=playerController...
   }

   void Update() {
        CheckInput();
   }

   void CheckInput() {
       if (Mouse.current.leftButton.wasPressedThisFrame) {
            Weapon weapon = gameObject.GetComponentInChildren<Weapon>();
            if (weapon != null) {
                CastRaycast(weapon);
                ResetLaserFx(weapon.laser);
            }
       }
    }

   void ResetLaserFx(LineRenderer r) {
       if (shootFxCor != null)
           StopCoroutine(shootFxCor);

       shootFxCor = StartCoroutine(DisplayLaserFx(r));
   }

   IEnumerator DisplayLaserFx(LineRenderer r) {
       r.enabled = true;
       yield return new WaitForSeconds(0.15f);
       r.enabled = false;
   }

    void CastRaycast(Weapon weapon) {
        var ray = new Ray(weapon.laser.transform.position + transform.forward/10f,weapon.laser.transform.forward);
       weapon.laser.SetPosition(0, weapon.laser.transform.position);
       RaycastHit hit;

       if (Physics.SphereCast(ray, 0.8f, out hit, 500f, layerMask)) {
           Debug.Log(hit.collider.gameObject.name);
           weapon.laser.SetPosition(1, hit.point);
           
           if (hit.transform.gameObject.tag.Equals(enemyPlayerTag))
           {
               PlayerArmour playerArmour = hit.transform.gameObject.GetComponent<PlayerArmour>();
               if (playerArmour != null) {
                   AudioManager.audioManagerInstance.PlaySound("WeaponShot");
                   playerArmour.HandleHit();
               }
           }
       } else {
           weapon.laser.SetPosition(1, weapon.laser.transform.position + weapon.laser.transform.forward * 30f);
       }
    }
}
