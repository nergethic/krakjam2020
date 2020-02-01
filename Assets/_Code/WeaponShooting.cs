using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class WeaponShooting : MonoBehaviour
{
   [SerializeField] int shootButton;
   [SerializeField] private string enemyPlayerTag;
   [SerializeField] private PlayerArmour playerArmour;
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(shootButton))
        {
     CastRaycast();
        }
    }

    void CastRaycast()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.tag.Equals(enemyPlayerTag))
            {
                
                //Add method 
            }
        }
    }
}
