using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockExplosionParticle : MonoBehaviour
{
    [SerializeField] private float timeAfterDestroy = 2;
    void Start()
    {
        Destroy(gameObject,timeAfterDestroy);
    }

}
