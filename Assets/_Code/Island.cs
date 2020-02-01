using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EZCameraShake;
using UnityEditor;
using UnityEngine;

public class Island : MonoBehaviour {
    [SerializeField] Transform islandCenter;
    [SerializeField] Transform islandMergePoint;
    [SerializeField] Transform destination;
    [SerializeField] float timeToMergeInSeconds;
    [SerializeField] Transform chunksParent;
    [SerializeField] Material chunkMaterial;
    [SerializeField] CameraShaker shaker;
    List<Transform> chunks;
    List<Rigidbody> rigidbodies;
    Vector3 diff;
    bool mergeCompleted = false;
    

    void Start()
    {
        var islandMergeLocalPoint = transform.InverseTransformPoint(islandMergePoint.position);
        
        diff = destination.position - islandMergePoint.position;

        chunks = new List<Transform>();
        rigidbodies = new List<Rigidbody>();

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
            if (shaker != null)
                shaker.ShakeOnce(10f, 2.4f, 0.2f, 0.5f);
        }

        transform.position = pos;
    }

    public void StartFragmenting() {
        StartCoroutine(FallingCor());
    }

    IEnumerator FallingCor() {
        for (var i = 0; i < 4; i++) {
            rigidbodies[i].isKinematic = false;
            yield return new WaitForSeconds(0.2f);
        }

        for (var i = 4; i < rigidbodies.Count; i++) {
            var r = rigidbodies[i];
            var outDir = (r.GetComponent<MeshCollider>().bounds.center - islandCenter.position).normalized / 5f;

            var pos = r.transform.position;
            float elapsedTime = 0f;
            float waitTime = 0.1f;
            while (elapsedTime < waitTime)
            {
                r.transform.position = Vector3.Lerp(pos, pos+outDir+(Vector3.down/6f), (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (shaker != null)
                shaker.ShakeOnce(1.86f, 0.57f, 0.1f, 0.3f);
            
            yield return new WaitForSeconds(0.25f);

            r.isKinematic = false;
            
            yield return new WaitForSeconds(0.5f);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Collect Refs")]
    public void CollectRefs() {
        Undo.RecordObject(this, "refs");
    }
#endif
}
