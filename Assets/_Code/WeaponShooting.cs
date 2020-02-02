using System;
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
                ResetLaserFx(weapon.laser);
                CastRaycast();
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
       yield return new WaitForSeconds(0.25f);
       r.enabled = false;
   }

    void CastRaycast() {
       var ray=new Ray(camera.position,camera.forward);
       RaycastHit hit;
     
       if (Physics.Raycast(ray, out hit, 500,layerMask))
       {
           // TODO: Add shooting Shader method
           Debug.Log(hit.collider.gameObject.name);
           
           if (hit.transform.gameObject.tag.Equals(enemyPlayerTag))
           {
               PlayerArmour playerArmour = hit.transform.gameObject.GetComponent<PlayerArmour>();
               if (playerArmour != null) {
                   // TODO play sound
                   playerArmour.HandleHit();
               }
           }
       }
    }
}
