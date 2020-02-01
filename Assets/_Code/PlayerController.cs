using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float rotationSpeed;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    const string SPEED_BLEND = "Speed";
    const string JUMP_BLEND_NAME = "Jump";
    const string JUMP_FORWARD_BOOL_NAME = "JumpForward";
    const float JUMP_COOLDOWN = 3f;
    new Transform transform;
    float timeOfLastJump = -3f;

    void Awake() {
        transform = gameObject.transform;
    }
    void Update() {
        var shiftDown = Input.GetKey (KeyCode.LeftShift);
        var forwardKeyPressed = Input.GetKey (KeyCode.W);
        if (forwardKeyPressed) {
            animator.SetFloat(SPEED_BLEND, shiftDown ? 2 : 1);
            transform.position += transform.forward * (shiftDown ? runSpeed : walkSpeed);
            if (Input.GetKey (KeyCode.A))
                transform.Rotate(0, -rotationSpeed, 0);
            else if (Input.GetKey (KeyCode.D))
                transform.Rotate(0, rotationSpeed, 0);
        }
        else if(Input.GetKey (KeyCode.S)){
            animator.SetFloat(SPEED_BLEND, -1);
            transform.position -= transform.forward * walkSpeed/2f;
            if (Input.GetKey (KeyCode.A))
                transform.Rotate(0, rotationSpeed, 0);
            else if (Input.GetKey (KeyCode.D))
                transform.Rotate(0, -rotationSpeed, 0);
        }
        else {
            animator.SetFloat (SPEED_BLEND, 0);
        }

        if (Input.GetKeyDown (KeyCode.Space) && Time.time > timeOfLastJump + JUMP_COOLDOWN) {
            animator.SetTrigger (forwardKeyPressed ? JUMP_FORWARD_BOOL_NAME : JUMP_BLEND_NAME);
            rb.AddForce (Vector3.Lerp (transform.forward, Vector3.up, 0.5f) * jumpForce, ForceMode.Impulse);
            timeOfLastJump = Time.time;
        }
    }
}
