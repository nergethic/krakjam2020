﻿using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class PlayerController : MonoBehaviour {
    [SerializeField] int padIndex = 0;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float rotationSpeed;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    const string MOVEMENT_BLEND = "Speed";
    const float AIM_WEIGHT_CHANGE_SPEED = 1f;
    const string JUMP_BLEND_NAME = "Jump";
    const string JUMP_FORWARD_BOOL_NAME = "JumpForward";
    const float JUMP_COOLDOWN = 1.5f;
    const float MIN_STICK_TILT = 0.05f;
    new Transform transform;
    Gamepad pad;
    float timeOfLastJump = -2f;
    bool equippingGun;
    
    void Awake() {
        transform = gameObject.transform;
        AssignGamepad();
    }

    void AssignGamepad() {
        var pads = Gamepad.all;
        if (pads[padIndex] == null) {
            Debug.Log($"Pad by index {padIndex} is not connected");
            return;
        }
        pad = pads[padIndex];
    }

    void Update() {
        bool shiftDown;
        bool forwardKeyPressed;
        bool backwardKeyPressed;
        bool leftKeyPressed;
        bool rightKeyPressed;
        bool jumpPressed;
        
        if(pad == null) {
            shiftDown = Input.GetKey(KeyCode.LeftShift);
            forwardKeyPressed = Input.GetKey(KeyCode.W);
            backwardKeyPressed = Input.GetKey(KeyCode.S);
            leftKeyPressed = Input.GetKey(KeyCode.A);
            rightKeyPressed = Input.GetKey(KeyCode.D);
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
        }
        else {
            shiftDown = pad.rightTrigger.isPressed;
            var leftStick = pad.leftStick.ReadValue();

            forwardKeyPressed = leftStick.y > MIN_STICK_TILT;
            backwardKeyPressed = leftStick.y < -MIN_STICK_TILT;
            rightKeyPressed = leftStick.x > MIN_STICK_TILT;
            leftKeyPressed = leftStick.x < -MIN_STICK_TILT;
            
            jumpPressed = pad.aButton.isPressed;
        }
        
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

        if (jumpPressed && Time.time > timeOfLastJump + JUMP_COOLDOWN) {
            animator.SetTrigger (forwardKeyPressed ? JUMP_FORWARD_BOOL_NAME : JUMP_BLEND_NAME);
            rb.AddForce (Vector3.Lerp (transform.forward, Vector3.up, 0.5f) * jumpForce, ForceMode.Impulse);
            timeOfLastJump = Time.time;
        }

        if (forwardKeyPressed || backwardKeyPressed || equippingGun) {
            if (leftKeyPressed)
                transform.Rotate(0, -rotationSpeed, 0);
            else if (rightKeyPressed)
                transform.Rotate(0, rotationSpeed, 0);
        }
    }
    [ContextMenu("Equip")]
    public void AnimateEquippingGun() {
        if(weightCor != null)
            StopCoroutine (weightCor);
        weightCor = StartCoroutine (UpperBodyWeightChangeCor (1f));
        equippingGun = true;
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

    bool Approximately(float a, float b, float tolerance) {
        return Mathf.Abs(a - b) < tolerance;
    }
}
