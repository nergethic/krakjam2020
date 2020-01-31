using UnityEngine;

public class Island : MonoBehaviour
{
    [SerializeField]  Transform islandMergePoint;
    [SerializeField] Transform destination;
    [SerializeField] float timeToMergeInSeconds;

    private Vector3 diff;

    void Start()
    {
        diff = destination.position - islandMergePoint.position;
    }

    // Update is called once per frame
    void Update() {
        var pos = transform.position;

        var currentDiff = destination.position - islandMergePoint.position;
        if (currentDiff.magnitude > 0.01f) {
            pos += diff * Time.deltaTime / timeToMergeInSeconds;
        }

        transform.position = pos;
    }
}
