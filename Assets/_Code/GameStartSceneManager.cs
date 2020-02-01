using System;
using EZCameraShake;
using UnityEngine;

public class GameStartSceneManager : MonoBehaviour {
    [SerializeField] Transform player;
    [SerializeField] CameraShaker shaker;
    [SerializeField] Island island;

    private bool executed;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (executed)
            return;
        
        if (player.position.y < 1.2f)
            HandleGroundImpact();
    }

    void HandleGroundImpact() {
        shaker.ShakeOnce(10f, 2.4f, 0.2f, 0.5f);
        island.StartFragmenting();
        executed = true;
    }
}
