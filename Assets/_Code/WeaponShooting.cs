using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class WeaponShooting : MonoBehaviour
{
 
   [SerializeField] int shootButton;
   [SerializeField] private string enemyPlayerTag;
  
   [SerializeField] private Transform camera;

   private int layerMask;
  // [SerializeField] private PlayerController playerController;
   private void Start()
   {
        layerMask = 1 << LayerMask.NameToLayer ("Player"); // only check for collisions with layerX
    //   Transform camera=playerController...
   }

   void Update()
    {
        CheckInput();
        
    }

    
    
    void CheckInput()
    {
      
        if (Input.GetMouseButtonDown(shootButton))
        {
            Transform[] childrenCount;
            childrenCount = gameObject.GetComponentsInChildren<Transform>();
            
            if (childrenCount.Length == 2)
            {
                Weapon weapon = gameObject.GetComponentInChildren<Weapon>();
                weapon.isAttached = true;
                CastRaycast();
            }
        }
    }

    void CastRaycast()
    {
       var ray=new Ray(camera.position,camera.forward);
       RaycastHit hit;
     
       if (Physics.Raycast(ray, out hit, 500,layerMask))
       {
           //Add shooting Shader method
           Debug.Log(hit.collider.gameObject.name);
           
           if (hit.transform.gameObject.tag.Equals(enemyPlayerTag))
           {
               PlayerArmour playerArmour = hit.transform.gameObject.GetComponent<PlayerArmour>();
               playerArmour.HandleHit();
           }
       }
        
     
            
                
              
              
            
        
    }
}
