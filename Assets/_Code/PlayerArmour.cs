using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmour : MonoBehaviour
{   
    [SerializeField] private Transform armourBodyPlace;
    [SerializeField] private float flyTime = 1;
    private List<GameObject> armourInTriggerList= new List<GameObject>();
    private string armourTag = "Armour";

   float distanceWhenParent=0.05f;
    private Coroutine cor;
    private void OnTriggerEnter(Collider other)
    {
       CheckIsTriggerEnterWithArmour(other);
    }

    private void Update()
    {
        CheckIsArmourCloseToArmourPlace();  
    }

    void CheckIsArmourCloseToArmourPlace()
    {
        if (armourInTriggerList != null)
        {
            for (int i = 0; i < armourInTriggerList.Count; i++)
            {
                //armourBodyPlace trzeba podstawić odpowiedniego z Enuma
                float distanceBetweenArmourAndArmourPlaceholder = Vector3.Distance(armourInTriggerList[i].transform.position, armourBodyPlace.transform.position);
              
                if (distanceBetweenArmourAndArmourPlaceholder <= distanceWhenParent)
                {
                  
                    StopCoroutine(cor);
                    armourInTriggerList[i].transform.parent = armourBodyPlace.gameObject.transform;
                    armourInTriggerList.Remove(armourInTriggerList[i]);
                }
            }
           
        }
    }

    void CheckIsTriggerEnterWithArmour(Collider other)
    {
        if (other.tag.Equals(armourTag))
        {
            armourInTriggerList.Add(other.gameObject);
           
           cor= StartCoroutine(ArmourTravelToPlayer(other.gameObject));
        } 
    }

   IEnumerator ArmourTravelToPlayer(GameObject armour)
   {
       var elapsedTime = 0f;
       while (elapsedTime<flyTime)
       {
           armour.transform.position = Vector3.Lerp(armour.transform.position, armourBodyPlace.position,elapsedTime/flyTime);
           
           
         
           armour.transform.rotation=Quaternion.Lerp(armour.transform.rotation, armourBodyPlace.rotation,elapsedTime/flyTime);
           elapsedTime += Time.deltaTime;
           yield return null;
       }

       
   }
   
}
