using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Code.Robot_Parts {
    public class RobotBody : MonoBehaviour
    {
       public bool player1;
       public Transform body;
        [SerializeField] BodyPart[] bodyParts;
        List<BodyPart.BodyType> centerBodyTypes = new List<BodyPart.BodyType> {BodyPart.BodyType.Chest, BodyPart.BodyType.Helm};

        public (bool, BodyPart) GetBodyPart(ArmourPart part) {
            foreach (var bodyPart in bodyParts) {
                if (bodyPart.Type == part.bodyType && bodyPart.Side == part.side) {
                    if (!bodyPart.IsOccupied)
                        return (true, bodyPart);
                    if (centerBodyTypes.Contains(bodyPart.Type)) {
                        var (found, otherSidePart) = GetOtherSidePart(bodyPart);
                        if (found)
                            return (true, otherSidePart);
                    }
                }
            }

            return (false, null);
        }

        (bool, BodyPart) GetOtherSidePart(BodyPart part) {
            foreach (var bodyPart in bodyParts) 
                if(bodyPart.Type == part.Type && bodyPart.Side != part.Side && !bodyPart.IsOccupied)
                    return (true, bodyPart);
            return (false, null);
        }

        public BodyPart[] GetBodyParts() {
            return bodyParts;
        }
        
#if UNITY_EDITOR

        [ContextMenu("Setup")]
        void SetupPartsInsideRig() {
            bodyParts = GetComponentsInChildren<BodyPart>();
            return;
            Undo.RecordObject(this, "setup");
            
            var namesToFind = new [] {"lowerArm", "chest", "lowerleg", "upperleg", "upperarm", "foot"};
            var parts = new List<BodyPart>();
            foreach (var t in GetComponentsInChildren<Transform>()) {
                //AddFromList(namesToFind, t, parts);
            }

            bodyParts = parts.ToArray();
        }

        static void AddFromList(string[] namesToFind, Transform t, List<BodyPart> parts) {
            foreach (var nameToFind in namesToFind) {
                if (t.name.ToLower().Contains(nameToFind.ToLower()) && !t.name.ToLower().Contains("end".ToLower()) && 
                    !t.name.ToLower().Contains("target".ToLower())) {
                    Undo.RecordObject(t, "BodyPart");
                    var bp = t.GetComponent<BodyPart>();
                    if (bp == null) {
                        Debug.Log ($"Adding part to {t.name}");
                        parts.Add (t.gameObject.AddComponent<BodyPart>());
                    }
                    else {
                        parts.Add (bp);
                    }
                }
            }
        }
#endif
    }
}