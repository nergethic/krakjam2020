using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Robot_Parts;
using UnityEngine;



public class CatapultShoot : MonoBehaviour
{  
    [SerializeField] Vector3[] positions = new Vector3[50];
    [SerializeField] Transform rockStartTransform;
    [SerializeField]  float flyTime = 4.0f;
    [SerializeField] private float distanceToGiveDMG = 2;
    [SerializeField] private Transform enemyPlayerTransform;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private bool drawGizmos;
    [SerializeField] private string playerTag;
    [SerializeField] private GameObject Popup;
    [SerializeField] private Transform cameraTransform;
    public Vector3 middlePointOnCurve;
    private KeyCode launchKeycode=KeyCode.E;
    private bool isShooting = false;
    private GameObject rockClone;
    private Vector3 enemyPositionInShootMoment;
    private Coroutine cor;
    private bool canShoot=true;
    private int numPoints = 50;
    private GameObject explosionParticle;
    


    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals(playerTag))
        {
            if (Input.GetKeyDown(launchKeycode) && canShoot)
            {
                canShoot = false;
                isShooting = true;
                if (rockClone != null)
                {
                    rockClone.SetActive(true);
                    rockClone.transform.position = rockStartTransform.position;
                }
                else
                {
                    rockClone = Instantiate(rockPrefab, rockStartTransform.position, rockStartTransform.rotation);
                }

                enemyPositionInShootMoment = enemyPlayerTransform.position;
            }
            Popup.SetActive(true);
            Popup.transform.LookAt(cameraTransform);
        }
        
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(playerTag))
        {
            Popup.SetActive(false);
        }
    }

    void Update()
    {

        if (isShooting)
        {
            LaunchCatapult();
            
        }
    }

    [ContextMenu("cache")]
    void CacheLinearCurve()
    {
        for (int i = 1; i < numPoints+1; i++)
        {
            float t = i / (float) numPoints;
            positions[i - 1] = GetBezierPoint(t, rockStartTransform.position,enemyPositionInShootMoment,middlePointOnCurve);
        }

    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {

                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }
        }
    }

    [ContextMenu("FLY")]
    void LaunchCatapult()
    {
        CacheLinearCurve();
        if(cor !=null)
            StopCoroutine(cor);
        cor = StartCoroutine(FlyCor());

    }

    IEnumerator FlyCor()
    {
        isShooting = false;
        var flyTime = this.flyTime;
        var elapsedTime = 0f;
        while (elapsedTime < flyTime)
        {
            if (elapsedTime < (flyTime * 9) / 10)
            {
                rockClone.transform.position = GetBezierPoint(elapsedTime / flyTime, rockStartTransform.position,
                    enemyPlayerTransform.position, middlePointOnCurve);
                enemyPositionInShootMoment = enemyPlayerTransform.position;
            }
            else
            {
                rockClone.transform.position = GetBezierPoint(elapsedTime / flyTime, rockStartTransform.position,
                    enemyPositionInShootMoment, middlePointOnCurve);
            }
            
            elapsedTime += Time.deltaTime;
        
            yield return null;
           
        }
        CheckDistanceToPlayer();
        PlayeExplosionParticle();
        rockClone.SetActive(false);
        isShooting = false;
        canShoot = true;
    }

    Vector3 GetBezierPoint(float t, Vector3 startPosition,Vector3 middlePointOnCurve, Vector3 endPosition)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * startPosition;
        p += 2 * u * t * endPosition;
        p += tt * middlePointOnCurve;
        return p;
    }

    void CheckDistanceToPlayer()
    {
       float distanceBetweenPlayerAndRock = Vector3.Distance(rockClone.transform.position, enemyPlayerTransform.position);
       if (distanceBetweenPlayerAndRock <= distanceToGiveDMG)
       { 
          DealDMGToPlayer();
       }
    }

    void DealDMGToPlayer()
    {
      
      PlayerArmour playerArmour= enemyPlayerTransform.GetComponent<PlayerArmour>();
      playerArmour.RemoveRandomBodyPart();
    }

    void PlayeExplosionParticle()
    {
       
       GameObject explosionParticle = Instantiate( rockClone.transform.GetChild(0).gameObject,rockClone.transform.position,rockClone.transform.rotation);
        explosionParticle.SetActive(true);
    }
}
