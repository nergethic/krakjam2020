using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicCamera : MonoBehaviour {
    public float startPointLookDuration;
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float backOffset;
    [SerializeField] float upOffset;
    [SerializeField] float rightOffset;
    [SerializeField] AnimationCurve curve;
    [SerializeField] AnimationCurve deltaAngleToSpeedMultiplier;
    [SerializeField] AnimationCurve deltaTranslationToSpeedMultiplier;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform startLookAtTarget;
    [SerializeField] Transform player;
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform playerHead;
    [SerializeField] GroundImpacter groudImpacter;
    new Transform transform;
    bool playerReached;
    bool groundHit;
    bool playerIsDead;
    float angle;
    bool weaponCameraModeEnabled;

    public void ToggleCameraMode() {
//        movementSpeed = 30f;
//        rotationSpeed = 30f;
//        backOffset = 2.22f;
//        upOffset = 2.46f;
//        rightOffset = 1.98f;
        if (weaponCameraModeEnabled){
            weaponCameraModeEnabled = false;
            backOffset = 4.5f;
            upOffset = 2.5f;
            rightOffset = 0f;
            return;
        }
        weaponCameraModeEnabled = true;
        backOffset = 1.1f;
        upOffset = 1.6f;
        rightOffset = -0.1f;
    }

    void Awake() {
        transform = gameObject.transform;
        groudImpacter.OnGroundHit += HandleGroundHit;
        angle = Vector3.Angle(player.forward, Vector3.forward);
    }

    void HandleGroundHit() {
        groundHit = true;
    }

    public void EnableWastedPositioning() {
        playerIsDead = true;
        movementSpeed /= 7f;
        rotationSpeed /= 7f;
    }

    IEnumerator Start() {
        transform.position = startPoint.position;
        transform.LookAt (startLookAtTarget);
        while (!groundHit)
            yield return null;
        playerController.HandleGroundHit();
        yield return new WaitForSeconds (startPointLookDuration);
        playerController.UnlockInput();
        var progress = 0f;
        while (progress < 1) {
            var targetPosition = GetCameraTargetPosition();
            progress += Time.deltaTime * curve.Evaluate ((targetPosition - transform.position).magnitude);
            transform.position = Vector3.Lerp (transform.position, GetCameraTargetPosition(), progress);
            var dir = (playerHead.position - targetPosition).normalized;
            var lookRotation = Quaternion.LookRotation (dir);
            transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, progress);
            yield return null;
        }

        playerReached = true;
    }

    void Update() {
        if(!playerReached)
            return;
        var xStick = playerController.rightStick.x;
        if (xStick > 0.2f || xStick < -0.2f){
            angle += rotationSpeed * xStick / 30f;
        }

        var targetPosition = GetCameraTargetPosition();
        var dir = playerHead.position - targetPosition;
        var movSpeedMultiplier = deltaTranslationToSpeedMultiplier.Evaluate (dir.magnitude);
        transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * movementSpeed * movSpeedMultiplier);

        var lookRotation = Quaternion.LookRotation (dir.normalized);
        var speedMultiplier = deltaAngleToSpeedMultiplier.Evaluate (Quaternion.Angle (transform.rotation, lookRotation));
        transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * speedMultiplier);
        
        
    }

    Vector3 GetCameraTargetPosition() {
        if (playerIsDead)
            return player.position + Vector3.up * 10f;
        var backDir = -Vector3.forward;
        var cameraDir = Quaternion.AngleAxis(angle, Vector3.up) * backDir;
        var cameraOffset = weaponCameraModeEnabled ? -player.forward : cameraDir * backOffset;
        return player.position + cameraOffset + player.up * upOffset + player.right * rightOffset;
    }
}
