using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField] int padIndex = 0;
    public DynamicCamera dynamicCamera;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float rotationSpeed;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    const string MOVEMENT_BLEND = "Speed";
    const string DEATH_TRIGGER_NAME = "Death";
    const float AIM_WEIGHT_CHANGE_SPEED = 5f;
    const string JUMP_BLEND_NAME = "Jump";
    const string JUMP_FORWARD_BOOL_NAME = "JumpForward";
    const string GROUND_HIT_TRIGGER_NAME = "HitGround";
    const string PLAYER_HIT_TRIGGER_NAME = "Hit";
    const float JUMP_COOLDOWN = 1.5f;
    const float MIN_STICK_TILT = 0.05f;
    new Transform transform;
    Gamepad pad;
    float timeOfLastJump = -2f;
    bool equippingGun;
    bool inputBlocked = true;
    bool dead;

    void Awake() {
        transform = gameObject.transform;
        AssignGamepad();
    }

    void AssignGamepad() {
        var pads = Gamepad.all;
        if (!pads.Any() || pads[padIndex] == null) {
            Debug.Log($"Pad by index {padIndex} is not connected");
            return;
        }
        pad = pads[padIndex];
    }

    public void UnlockInput() {
        inputBlocked = false;
    }

    void Update() {
        if (inputBlocked)
            return;
        
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

    Coroutine inputRestoreCor;

    [ContextMenu ("Hit")]
    void xd() {
        PlayHitAnimation (transform);
    }
    public void PlayHitAnimation(Transform otherPlayer) {
        if (dead)
            return;
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
        animator.SetLayerWeight (1, 0);
        inputBlocked = true;
        dead = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
        animator.applyRootMotion = true;
        animator.SetTrigger (DEATH_TRIGGER_NAME);
    }

    Coroutine weightCor;
    IEnumerator UpperBodyWeightChangeCor(float targetWeight) {
        var progress = 0f;
        var weight = animator.GetLayerWeight (1);
        var startWeight = weight;
        while (weight != targetWeight) {
            weight = Mathf.Lerp (startWeight, targetWeight, progress);
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
