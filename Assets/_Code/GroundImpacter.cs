using System;
using System.Collections;
using _Code;
using EZCameraShake;
using UnityEngine;

public class GroundImpacter : MonoBehaviour, IWaitForStart {
    public Action OnGroundHit;
    [SerializeField] CameraShaker[] shakers;
    [SerializeField] Camera leftCamera;
    [SerializeField] Camera leftUICamera;
    [SerializeField] DynamicCamera dynamicCamera;
    [SerializeField] Camera rightCamera;
    [SerializeField] Camera rightUICamera;
    [SerializeField] Transform groundPoint;
    [SerializeField] Island island1;
    [SerializeField] Transform player1;
    [SerializeField] Island island2;
    [SerializeField] Transform player2;

    public bool Ready { get; set; }
    public StartMenu StartMenu { get; set; }

    IEnumerator Start() {
        Ready = true;
        while (!Ready)
            yield return null;
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
        island1.StartFragmenting();
        player1.SetParent(island1.transform);
        
        island2.StartFragmenting();
        player2.SetParent(island2.transform);
    }
}
