using UnityEngine;

public class Island : MonoBehaviour
{
    [SerializeField]  Transform islandMergePoint;
    [SerializeField] Transform destination;
    [SerializeField] float timeToMergeInSeconds;
    [SerializeField] Transform chunksParent;
    [SerializeField] Material chunkMaterial;

    private Vector3 diff;
    private bool mergeCompleted = false;
    private Rigidbody[] rigidbodies;

    void Start()
    {
        diff = destination.position - islandMergePoint.position;

        for (var i = 0; i < chunksParent.childCount; i++) {
            var child = chunksParent.GetChild(i);
            var meshCollider = child.gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            
            var rb = child.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var r = child.GetComponent<MeshRenderer>();
            if (r == null)
                continue;
            
            r.sharedMaterial = chunkMaterial;
        }
        
        rigidbodies = chunksParent.GetComponentsInChildren<Rigidbody>();
    }

    void Update() {
        if (mergeCompleted) {
            return;
        }
        
        var pos = transform.position;

        var currentDiff = destination.position - islandMergePoint.position;
        if (currentDiff.magnitude > 0.1f) {
            pos += diff * Time.deltaTime / timeToMergeInSeconds;
        } else {
            mergeCompleted = true;
            StartFragmenting();
        }

        transform.position = pos;
    }

    void StartFragmenting() {
        foreach (var r in rigidbodies) {
            r.isKinematic = false;
        }
    }
}
