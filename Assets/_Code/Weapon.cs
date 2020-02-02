using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] BoxCollider collisionCollider;
    public LineRenderer laser;
    public bool isAttached;

    private void Start() {
        laser.enabled = false;
    }

    public void SetAttached(bool value) {
        isAttached = value;

        if (!value)
            laser.enabled = false;
    }

    public BoxCollider GetCollisionCollider() {
        return collisionCollider;
    }
}
