using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Code.Robot_Parts {
    public class RobotBody : MonoBehaviour
    {
       public bool player1;
        [SerializeField] BodyPart[] bodyParts;
        List<BodyPart.BodyType> centerBodyTypes = new List<BodyPart.BodyType> {BodyPart.BodyType.Chest, BodyPart.BodyType.Helm, BodyPart.BodyType.Leg};

        public (bool, BodyPart) GetBodyPart(BodyPart.BodyType type) {
            foreach (var bodyPart in bodyParts) {
                if (bodyPart.Type == type) {
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
            Undo.RecordObject(this, "setup");
            
            var namesToFind = new [] {"lowerArm", "head", "chest", "lowerleg"};
            var parts = new List<BodyPart>();
            foreach (var t in GetComponentsInChildren<Transform>()) {
                AddFromList(namesToFind, t, parts);
            }

            bodyParts = parts.ToArray();
        }

        static void AddFromList(string[] namesToFind, Transform t, List<BodyPart> parts) {
            foreach (var nameToFind in namesToFind) {
                if (t.name.ToLower().Contains(nameToFind.ToLower()) && !t.name.ToLower().Contains("end".ToLower()) && !t.name.ToLower().Contains("target".ToLower()) && t.GetComponent<BodyPart>() == null) {
                    Undo.RecordObject(t, "BodyPart");
                    Debug.Log($"Adding part to {t.name}");
                    parts.Add(t.gameObject.AddComponent<BodyPart>());
                }
            }
        }
#endif
    }
}