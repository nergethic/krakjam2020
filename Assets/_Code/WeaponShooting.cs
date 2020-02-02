using System;
using System.Collections;
using System.Collections.Generic;
using _Code;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.InputSystem;

public class WeaponShooting : MonoBehaviour, IWaitForStart { 
    [SerializeField] int shootButton;
    [SerializeField] string enemyPlayerTag;
    [SerializeField] Transform camera;
    [SerializeField] PlayerController playerController;
    bool shootingBlocked = false;

    Coroutine shootingBlockedCor;
    Coroutine shootFxCor;
    int layerMask;

    private float waitTime = 0f;
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
       bool isInputPressed = playerController.PadAssigned ? playerController.Pad.yButton.wasPressedThisFrame : Mouse.current.leftButton.wasPressedThisFrame;
       if (isInputPressed) {
           Weapon weapon = gameObject.GetComponentInChildren<Weapon>();
           if (weapon == null || shootingBlocked)
               return;

           if (shootingBlockedCor != null) {
               StopCoroutine(shootingBlockedCor);
               shootingBlockedCor = null;
           }
           shootingBlockedCor = StartCoroutine(BlockShootingForSomeTime(weapon));
           
           
            CastRaycast(weapon);
            ResetLaserFx(weapon.laser);
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
   
   IEnumerator BlockShootingForSomeTime(Weapon weapon) {
       shootingBlocked = true;
       weapon.SetReloadLevel(0f);
       yield return new WaitForSeconds(0.2f);
       weapon.SetReloadLevel(0.2f);
       
       yield return new WaitForSeconds(0.2f);
       weapon.SetReloadLevel(0.4f);
       
       yield return new WaitForSeconds(0.2f);
       weapon.SetReloadLevel(0.6f);
       
       yield return new WaitForSeconds(0.2f);
       weapon.SetReloadLevel(0.8f);
       
       yield return new WaitForSeconds(0.2f);
       weapon.SetReloadLevel(1f);
       
       shootingBlocked = false;
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

    public bool Ready { get; set; }
    public StartMenu StartMenu { get; set; }
}
