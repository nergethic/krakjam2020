using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Code.Robot_Parts;
using UnityEditor;
using UnityEngine;
using BodyPart = _Code.Robot_Parts.BodyPart;
using Random = System.Random;


public class PlayerArmour : MonoBehaviour
{
    [SerializeField] RobotBody robotBody;
    [SerializeField] float distanceWhenParent = 0.05f;
    [SerializeField] float flyTime = 1;
    [SerializeField] ArmourPart[] armorParts;
    private List<ArmourAndArmourPlaceholder> structList = new List<ArmourAndArmourPlaceholder>();
    private string armourTag = "Armour";
    private Coroutine cor;
    private Random rnd = new Random();
    private void OnTriggerEnter(Collider other)
    {
        CheckIsTriggerEnterWithArmour(other);
    }

    private void Update()
    {
        CheckIsArmourCloseToArmourPlace();
    }

    public void RemoveRandomBodyPart()
    {
        var randomParts = GetParts().OrderBy(x=>rnd.Next()).ToList();
        for (int i = 0; i < randomParts.Count; i++)
        {
            var randomPart = randomParts[i];
            if (i == 0)
            {
                StartCoroutine(EnablePhysicsAfterSomeTime(randomPart));
            }
            else {
                //LerpArmourAwayAndBack();
            }
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
        rb.AddForce(-transform.forward*8f, ForceMode.Impulse);
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
        RemoveRandomBodyPart();
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