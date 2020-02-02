using System;
using System.Collections;
using _Code;
using EZCameraShake;
using UnityEngine;

public class GroundImpacter : MonoBehaviour, IWaitForStart {
    public Action OnGroundHit;
    [SerializeField] CameraShaker[] shakers;
    [SerializeField] Camera leftCamera;
    [SerializeField] DynamicCamera dynamicCamera;
    [SerializeField] Camera rightCamera;
    [SerializeField] Island island;
    [SerializeField] Transform groundPoint;
    [SerializeField] Transform player1;

    public bool Ready { get; set; }
    public StartMenu StartMenu { get; set; }

    IEnumerator Start() {
        while (!Ready)
            yield return null;
        while (player1.position.y > groundPoint.position.y)
            yield return null;
        OnGroundHit?.Invoke();
        Execute();
        yield return new WaitForSeconds (dynamicCamera.startPointLookDuration);
        leftCamera.rect = new Rect(-0.5f, 0f, 1f, 1f);
        rightCamera.rect = new Rect(0.5f, 0f, 1f, 1f);
    }
    
    void Execute() {
        foreach (var shaker in shakers) {
            shaker.ShakeOnce(10f, 2.4f, 0.2f, 0.5f);
        }
        island.StartFragmenting();
    }
}
