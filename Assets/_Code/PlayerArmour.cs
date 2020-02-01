using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Robot_Parts;
using UnityEngine;

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

public class PlayerArmour : MonoBehaviour
{
    [SerializeField] private RobotBody robotBody;
    [SerializeField] float distanceWhenParent = 0.05f;
    [SerializeField] private float flyTime = 1;
    private List<ArmourAndArmourPlaceholder> structList = new List<ArmourAndArmourPlaceholder>();
    private string armourTag = "Armour";
    private Coroutine cor;
    private void OnTriggerEnter(Collider other)
    {
        CheckIsTriggerEnterWithArmour(other);
    }

    private void Update()
    {
        CheckIsArmourCloseToArmourPlace();
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
                    armourPartTransform.GetComponent<ArmourPart>().isAttached = true;
                    if (structList[i].coroutine != null)
                        StopCoroutine(structList[i].coroutine);
                    structList.Remove(structList[i]);
                }
            }
        }
    }

    void CheckIsTriggerEnterWithArmour(Collider other)
    {
        if (other.tag.Equals(armourTag))
        {
            var go = other.gameObject;
            var armorPart = go.GetComponent<ArmourPart>();
            if (armorPart == null)
                return;
            var (exists, type) = robotBody.GetBodyPart(armorPart.bodyType);

            if (exists)
            {
                type.SetOccupied(true);
                Coroutine cor = StartCoroutine(ArmourTravelToPlayer(other.gameObject, type));
                var structItem = new ArmourAndArmourPlaceholder(type, other.transform, cor);
                structList.Add(structItem);
            }
        }
    }

    IEnumerator ArmourTravelToPlayer(GameObject armour, BodyPart bodyPart)
    {
        var elapsedTime = 0f;
        while (elapsedTime < flyTime)
        {
            armour.transform.position = Vector3.Lerp(armour.transform.position, bodyPart.transform.position,
                elapsedTime / flyTime);
            armour.transform.rotation = Quaternion.Lerp(armour.transform.rotation, bodyPart.transform.rotation,
                elapsedTime / flyTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}