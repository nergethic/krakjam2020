using System;
using System.Collections;
using EZCameraShake;
using UnityEngine;

public class GroundImpacter : MonoBehaviour {
    public Action OnGroundHit;
    [SerializeField] CameraShaker[] shakers;
    [SerializeField] Camera leftCamera;
    [SerializeField] Camera leftUICamera;
    [SerializeField] DynamicCamera dynamicCamera;
    [SerializeField] Camera rightCamera;
    [SerializeField] Camera rightUICamera;
    [SerializeField] Island island;
    [SerializeField] Transform groundPoint;
    [SerializeField] Transform player1;

    IEnumerator Start() {
        while (player1.position.y > groundPoint.position.y)
            yield return null;
        OnGroundHit?.Invoke();
        Execute();
        yield return new WaitForSeconds (dynamicCamera.startPointLookDuration);
        leftCamera.rect = new Rect(-0.5f, 0f, 1f, 1f);
        rightCamera.rect = new Rect(0.5f, 0f, 1f, 1f);
        leftUICamera.rect = new Rect(-0.5f, 0f, 1f, 1f);
        rightUICamera.rect = new Rect(0.5f, 0f, 1f, 1f);
    }
    
    void Execute() {
        foreach (var shaker in shakers) {
            shaker.ShakeOnce(10f, 2.4f, 0.2f, 0.5f);
        }
        island.StartFragmenting();
        player1.SetParent(island.transform);
    }
}
