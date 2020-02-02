using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Code.Robot_Parts;
using UnityEditor;
using UnityEngine;
using BodyPart = _Code.Robot_Parts.BodyPart;
using Random = System.Random;


public class PlayerArmour : MonoBehaviour {
    Action<bool> OnHit;
    Vector3 SCALE_INCREASE_STEP = new Vector3(0.1f,0.1f,0.1f);
    [SerializeField] PlayerController controller;
    [SerializeField] RobotBody robotBody;
    [SerializeField] float distanceWhenParent = 0.05f;
    [SerializeField] float flyTime = 1;
    [SerializeField] ArmourPart[] armorParts;
    private List<ArmourAndArmourPlaceholder> structList = new List<ArmourAndArmourPlaceholder>();
    private string armourTag = "Armour";
    private Coroutine cor;
    private Random rnd = new Random();
    bool dead;
    private void OnTriggerEnter(Collider other)
    {
        CheckIsTriggerEnterWithArmour(other);
    }

    private void Update()
    {
        if(!dead)
            CheckIsArmourCloseToArmourPlace();
    }

    [ContextMenu("Hit")]
    public void HandleHit() {
        var parts = GetParts();
        if (!parts.Any() || dead) {
            return;
        }
        var randomParts = parts.OrderBy(x=>rnd.Next()).ToList();
        if (randomParts.Count < 2) {
            dead = true;
            OnHit?.Invoke (true);
            controller?.PlayDeathAnimation();
        }
        else {
            robotBody.body.localScale -= SCALE_INCREASE_STEP;
            controller?.SetAnimationSpeed(1/robotBody.body.localScale.x);
        }

        OnHit?.Invoke (false);
        controller?.PlayHitAnimation();
        if (!randomParts.Any())
            return;
        var firstRandomPart = randomParts[0];
        LerpArmourAwayAndBack(randomParts);
        StartCoroutine(EnablePhysicsAfterSomeTime(firstRandomPart));
        randomParts.Remove (firstRandomPart);
    }

    #region Armour animation on hit

    Coroutine partLerpCor;
    void LerpArmourAwayAndBack(IEnumerable<ArmourPart> parts) {
        RestorePartsLocalPos (parts);
        if(partLerpCor != null)
            StopCoroutine (partLerpCor);
        partLerpCor = StartCoroutine (LerpArmourAwayAndBackCor (parts));
    }

    IEnumerator LerpArmourAwayAndBackCor(IEnumerable<ArmourPart> parts) {
        var progress = 0f;
        while (progress < 1f) {
            foreach (var armourPart in parts) {
                armourPart.transform.position += GetDirectionOut(armourPart.transform) * Time.deltaTime * UnityEngine.Random.Range(0.75f, 1.25f);
            }
            progress += Time.deltaTime*8f;
            yield return null;
        }
        progress = 0f;
        while (progress < 1f) {
            foreach (var armourPart in parts) {
                if(armourPart.transform.parent != null)
                    armourPart.transform.position = Vector3.Lerp (armourPart.transform.position, armourPart.transform.parent.position, progress);
            }
            progress += Time.deltaTime;
            yield return null;
        }

        RestorePartsLocalPos (parts);
    }

    Vector3 GetDirectionOut(Transform part) {
        var dir = part.transform.position - (robotBody.transform.position + Vector3.up*0.5f);
        dir.y = 0;
        return dir;
    }

    void RestorePartsLocalPos(IEnumerable<ArmourPart> parts) {
        foreach (var armourPart in parts) {
            armourPart.transform.localPosition = Vector3.zero;
        }
    }

    IEnumerable<ArmourPart> GetParts() {
        foreach (var armourPart in armorParts) {
            if (DoesPartBelongToThisPlayer (armourPart))
                yield return armourPart;
        }
    }

    bool DoesPartBelongToThisPlayer(ArmourPart part) {
        return part.isAttached && part.player1 == robotBody.player1;
    }
    #endregion
    
    IEnumerator EnablePhysicsAfterSomeTime(ArmourPart part) {
        foreach (var bodySocket in robotBody.GetBodyParts()) {
            if (bodySocket.IsOccupied && bodySocket.armourPart == part) {
                bodySocket.SetOccupied(false, null);
                break;
            }
        }
        part.isAttached = false;
        part.transform.SetParent(null);
        var rb = part.GetComponent<Rigidbody>();

        float elapsedTime = 0f;
        float waitTime = 0.1f;
        var pos = part.transform.position;
        var outDir = (pos + transform.forward).normalized * 0.7f;
        while (elapsedTime < waitTime)
        {
            part.transform.position = Vector3.Lerp(pos, pos+outDir+(Vector3.up/3f), (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        rb.isKinematic = false;
        rb.AddForce (GetDirectionOut(part.transform).normalized*8f, ForceMode.Impulse);
        rb.useGravity = true;
        var child = part.transform.GetChild(0);
        child.gameObject.SetActive(true);
    }

    void CheckIsArmourCloseToArmourPlace()
    {
        if (structList != null)
        {
            for (int i = 0; i < structList.Count; i++)
            {
                float distanceBetweenArmourAndArmourPlaceholder = Vector3.Distance(
                    structList[i].ArmourPartTransform.position, structList[i].bodyPart.transform.position);

                if (distanceBetweenArmourAndArmourPlaceholder <= distanceWhenParent)
                {
                    var armourPartTransform = structList[i].ArmourPartTransform;
                    var bodyPart = structList[i].bodyPart;
                    armourPartTransform.position = bodyPart.transform.position;
                    armourPartTransform.rotation = bodyPart.transform.rotation;
                    armourPartTransform.parent = bodyPart.gameObject.transform;
                    var armourPart = armourPartTransform.GetComponent<ArmourPart>();
                    armourPart.isAttached = true;
                    armourPart.player1 = robotBody.player1;
                    if (structList[i].coroutine != null)
                        StopCoroutine(structList[i].coroutine);
                    structList.Remove(structList[i]);
                }
            }
        }
    }

    void CheckIsTriggerEnterWithArmour(Collider other) {
        if (other.tag.Equals(armourTag)) {
            var go = other.gameObject;
            var armorPart = go.GetComponent<ArmourPart>();
            if (armorPart == null || armorPart.isAttached)
                return;
            var (exists, armorSocket) = robotBody.GetBodyPart(armorPart.bodyType);

            if (exists) {
                armorSocket.SetOccupied(true, armorPart);
                robotBody.body.localScale += SCALE_INCREASE_STEP;
                controller?.SetAnimationSpeed(1/robotBody.body.localScale.x);
                Coroutine cor = StartCoroutine(ArmourTravelToPlayer(other.transform, armorSocket));
                var structItem = new ArmourAndArmourPlaceholder(armorSocket, other.transform, cor);
                structList.Add(structItem);
            }
        }
    }

    IEnumerator ArmourTravelToPlayer(Transform armour, BodyPart bodyPart) {
        var triggerCol = armour.transform.GetChild(0).GetComponentInChildren<BoxCollider>();
        if (triggerCol == null)
            Debug.LogError("no trigger collider on armor part!");

        armour.GetComponentInChildren<Rigidbody>().isKinematic = true;
        triggerCol.gameObject.SetActive(false);
        
        var elapsedTime = 0f;
        while (elapsedTime < flyTime)
        {
            armour.position = Vector3.Lerp(armour.position, bodyPart.transform.position,
                elapsedTime / flyTime);
            armour.rotation = Quaternion.Lerp(armour.rotation, bodyPart.transform.rotation,
                elapsedTime / flyTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("Setup")]
    void SetupPartsInsideRig() {
        Undo.RecordObject(this, "setup");
        armorParts = FindObjectsOfType<ArmourPart>();
    }

    [ContextMenu("RemoveRandomPart")]
    public void RemoveRandomPartDebug() {
        HandleHit();
    }
#endif
}

struct ArmourAndArmourPlaceholder
{
    public Coroutine coroutine;
    public BodyPart bodyPart;
    public Transform ArmourPartTransform;

    public ArmourAndArmourPlaceholder(BodyPart bodyPart, Transform armourPart, Coroutine coroutine)
    {
        this.bodyPart = bodyPart;
        this.ArmourPartTransform = armourPart;
        this.coroutine = coroutine;
    }
}