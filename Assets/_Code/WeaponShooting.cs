using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class WeaponShooting : MonoBehaviour
{
 
   [SerializeField] int shootButton;
   [SerializeField] private string enemyPlayerTag;
   [SerializeField] private PlayerArmour playerArmourScript;
   [SerializeField] private PlayerController playerController;
   private void Start()
   {
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
        
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo))
        {
            Debug.DrawRay(transform.position, transform.right,Color.blue);
                
                //Add shooting Shader method
                if (hitInfo.transform.gameObject.tag.Equals(enemyPlayerTag))
                {

                    //Add method from PlayerArmour
                }
            
        }
    }
}
