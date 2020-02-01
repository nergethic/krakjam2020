using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Island : MonoBehaviour
{
    [SerializeField]  Transform islandMergePoint;
    [SerializeField] Transform destination;
    [SerializeField] float timeToMergeInSeconds;
    [SerializeField] Transform chunksParent;
    [SerializeField] Material chunkMaterial;

    Vector3 diff;
    bool mergeCompleted = false;
    List<Rigidbody> rigidbodies = new List<Rigidbody>();

    void Start()
    {
        var islandMergeLocalPoint = transform.InverseTransformPoint(islandMergePoint.position);
        
        diff = destination.position - islandMergePoint.position;
        var chunks = new List<Transform>();
        
        
        for (var i = 0; i < chunksParent.childCount; i++) {
            var child = chunksParent.GetChild(i);
            chunks.Add(child);
            
            var meshCollider = child.gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;

            var rb = child.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var r = child.GetComponent<MeshRenderer>();
            if (r == null)
                continue;
            
            r.sharedMaterial = chunkMaterial;
        }
        
        chunks = chunks.OrderBy(x => -(islandMergeLocalPoint - x.GetComponent<MeshCollider>().bounds.center).magnitude).ToList();
        foreach (var chunk in chunks) {
            rigidbodies.Add(chunk.GetComponent<Rigidbody>());
        }
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
        StartCoroutine(FallingCor());
    }

    IEnumerator FallingCor() {
        foreach (var r in rigidbodies) {
            r.isKinematic = false;
            yield return new WaitForSeconds(0.6f);
        }
    }
}
