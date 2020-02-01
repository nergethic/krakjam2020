﻿using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public DynamicCamera dynamicCamera;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float rotationSpeed;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    const string MOVEMENT_BLEND = "Speed";
    const string DEATH_TRIGGER_NAME = "Death";
    const float AIM_WEIGHT_CHANGE_SPEED = 1f;
    const string JUMP_BLEND_NAME = "Jump";
    const string JUMP_FORWARD_BOOL_NAME = "JumpForward";
    const string GROUND_HIT_TRIGGER_NAME = "HitGround";
    const string PLAYER_HIT_TRIGGER_NAME = "Hit";
    const float JUMP_COOLDOWN = 1.5f;
    new Transform transform;
    float timeOfLastJump = -2f;
    bool equippingGun;
    bool inputBlocked;

    void Awake() {
        transform = gameObject.transform;
    }
    void Update() {
        if (inputBlocked)
            return;
        var shiftDown = Input.GetKey (KeyCode.LeftShift);
        var forwardKeyPressed = Input.GetKey (KeyCode.W);
        var backwardKeyPressed = Input.GetKey (KeyCode.S);
        if (forwardKeyPressed) {
            animator.SetFloat(MOVEMENT_BLEND, shiftDown ? 2 : 1);
            transform.position += transform.forward * (shiftDown ? runSpeed : walkSpeed);
        }
        else if(backwardKeyPressed){
            animator.SetFloat(MOVEMENT_BLEND, -1);
            transform.position -= transform.forward * walkSpeed/2f;
        }
        else {
            animator.SetFloat (MOVEMENT_BLEND, 0);
        }

        if (Input.GetKeyDown (KeyCode.Space) && Time.time > timeOfLastJump + JUMP_COOLDOWN) {
            animator.SetTrigger (forwardKeyPressed ? JUMP_FORWARD_BOOL_NAME : JUMP_BLEND_NAME);
            rb.AddForce (Vector3.Lerp (transform.forward, Vector3.up, 0.5f) * jumpForce, ForceMode.Impulse);
            timeOfLastJump = Time.time;
        }

        if (forwardKeyPressed || backwardKeyPressed || equippingGun) {
            if (Input.GetKey (KeyCode.A))
                transform.Rotate(0, -rotationSpeed, 0);
            else if (Input.GetKey (KeyCode.D))
                transform.Rotate(0, rotationSpeed, 0);
        }
    }

    Coroutine inputRestoreCor;
    public void PlayHitAnimation(Transform otherPlayer) {
        inputBlocked = true;
        animator.SetTrigger (PLAYER_HIT_TRIGGER_NAME);
        var dir = transform.position - otherPlayer.position;
        var lookRotation = Quaternion.LookRotation (dir);
        transform.rotation = lookRotation;
        if(inputRestoreCor != null)
            StopCoroutine (inputRestoreCor);
        inputRestoreCor = StartCoroutine (RestoreInput());
    }

    IEnumerator RestoreInput() {
        yield return new WaitForSeconds (3f);
        inputBlocked = false;
    }

    public void HandleGroundHit() {
        animator.SetTrigger (GROUND_HIT_TRIGGER_NAME);
    }
    [ContextMenu("Equip")]
    public void AnimateEquippingGun() {
        if(weightCor != null)
            StopCoroutine (weightCor);
        weightCor = StartCoroutine (UpperBodyWeightChangeCor (1f));
        equippingGun = true;
    }

    [ContextMenu("Die")]
    public void PlayDeathAnimation() {
        animator.SetTrigger (DEATH_TRIGGER_NAME);
    }

    Coroutine weightCor;
    IEnumerator UpperBodyWeightChangeCor(float targetWeight) {
        var progress = 0f;
        var weight = animator.GetLayerWeight (1);
        var startWeight = weight;
        while (weight != targetWeight) {
            weight = Mathf.Lerp (weight, targetWeight, progress);
            animator.SetLayerWeight (1, weight);
            progress += Time.deltaTime * AIM_WEIGHT_CHANGE_SPEED;
            yield return null;
        }
    }
    [ContextMenu("Stop equipping")]
    public void StopEquippingGunAnimation() {
        if(weightCor != null)
            StopCoroutine (weightCor);
        weightCor = StartCoroutine (UpperBodyWeightChangeCor (0f));
        equippingGun = false;
    }
}
