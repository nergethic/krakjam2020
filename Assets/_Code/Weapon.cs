using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] BoxCollider collisionCollider;
    [SerializeField] MeshRenderer weaponReloadFX;
    [SerializeField] Material weaponReloadMat;
    public LineRenderer laser;
    public bool isAttached;
    Material mat;

    int reloadProgressShaderID;
    
    private void Start() {
        weaponReloadFX.material = weaponReloadMat;
        mat = weaponReloadFX.material;
        reloadProgressShaderID = Shader.PropertyToID("_ReloadLevel");
        mat.SetFloat(reloadProgressShaderID, 0f);
        
        laser.useWorldSpace = true;
        laser.SetPosition(0, transform.position);
        laser.enabled = false;
    }

    public void SetReloadLevel(float value) {
        mat.SetFloat(reloadProgressShaderID, value);
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
