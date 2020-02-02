using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Code;
using EZCameraShake;
using UnityEditor;
using UnityEngine;

public class Island : MonoBehaviour, IWaitForStart {
    public bool isFragmenting;
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
    
    public bool Ready { get; set; }
    public StartMenu StartMenu { get; set; }

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

        Ready = true;
    }

    void Update() {
        if (!Ready || mergeCompleted || !isFragmenting) {
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

        int count = 0;
        var bundle = new List<Rigidbody>();
        float bundleTime = 0f;
        for (var i = 4; i < rigidbodies.Count; i++) {
            var r = rigidbodies[i];
            var outDir = (r.GetComponent<MeshCollider>().bounds.center - islandCenter.position).normalized / 5f;

            var pos = r.transform.position;
            float elapsedTime = 0f;
            float waitTime = 0.09f;
            while (elapsedTime < waitTime)
            {
                r.transform.position = Vector3.Lerp(pos, pos+outDir+(Vector3.down/3f), (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                bundleTime += Time.deltaTime;
                yield return null;
            }

            bundle.Add(r);
            
            if (bundleTime > 2f) {
                foreach (var brb in bundle) {
                    brb.isKinematic = false;
                    brb.AddForce(outDir*10f, ForceMode.Impulse);
                    yield return new WaitForSeconds(0.06f);
                }
                if (shaker != null)
                    shaker.ShakeOnce(1.9f, 0.57f, 0.1f, 0.3f);
                yield return new WaitForSeconds(0.8f);
                StartMenu.TriggerBothPadsVibrations(0.9f, 1, 0.3f);
                bundleTime = 0f;
                bundle.Clear();
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Collect Refs")]
    public void CollectRefs() {
        Undo.RecordObject(this, "refs");
    }
#endif
}
