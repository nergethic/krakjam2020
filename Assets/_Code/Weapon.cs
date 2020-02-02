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
        laser.useWorldSpace = true;
        laser.SetPosition(0, transform.position);
        laser.enabled = false;
    }

    private void Update() {
        laser.SetPosition(0, laser.transform.position);
        laser.SetPosition(1, laser.transform.position + laser.transform.forward * 30f);
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
