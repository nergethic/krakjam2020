using System.Collections;
using UnityEngine;

public class DynamicCamera : MonoBehaviour {
    [SerializeField] float startPointLookDuration;
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float backOffset;
    [SerializeField] float upOffset;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform startLookAtTarget;
    [SerializeField] Transform player;
    [SerializeField] Transform playerHead;
    new Transform transform;
    bool playerReached;

    void Awake() {
        transform = gameObject.transform;
    }
    IEnumerator Start() {
        transform.position = startPoint.position;
        transform.LookAt (startLookAtTarget);
        yield return new WaitForSeconds (startPointLookDuration);
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
        var targetPosition = GetCameraTargetPosition();
        transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * movementSpeed);
        var dir = (playerHead.position - targetPosition).normalized;
        var lookRotation = Quaternion.LookRotation (dir);
        transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    Vector3 GetCameraTargetPosition() {
        return player.position - player.forward * backOffset + player.up * upOffset;
    }
}
