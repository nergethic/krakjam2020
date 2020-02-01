using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] float walkSpeed = 1f;
    [SerializeField] float runSpeed = 1.5f;
    [SerializeField] float rotationSpeed = 1.5f;
    [SerializeField] Animator animator;
    const string SPEED_BLEND = "Speed";
    const string JUMP_BOOL_NAME = "Jump";
    const string JUMP_FORWARD_BOOL_NAME = "JumpForward";
    new Transform transform;

    void Awake() {
        transform = gameObject.transform;
    }
    void Update() {
        var shiftDown = Input.GetKey (KeyCode.LeftShift);
        if (Input.GetKey (KeyCode.W)) {
            animator.SetFloat(SPEED_BLEND, shiftDown ? 2 : 1);
            transform.position += transform.forward * (shiftDown ? runSpeed : walkSpeed);
            if (Input.GetKey (KeyCode.A))
                transform.Rotate(0, -rotationSpeed, 0);
            else if (Input.GetKey (KeyCode.D))
                transform.Rotate(0, rotationSpeed, 0);
            
            if (Input.GetKeyDown (KeyCode.Space))
                animator.SetTrigger (JUMP_FORWARD_BOOL_NAME);
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

        if (Input.GetKeyDown (KeyCode.Space)) {
            animator.SetTrigger (JUMP_BOOL_NAME);
        }
    }
}
