using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Code.Robot_Parts {
    public class RobotBody : MonoBehaviour {
        [SerializeField] BodyPart[] bodyParts;
        List<BodyPart.BodyType> centerBodyTypes = new List<BodyPart.BodyType> {BodyPart.BodyType.Chest, BodyPart.BodyType.Helm, BodyPart.BodyType.Boots};

        public (bool, BodyPart) GetBodyPart(BodyPart.BodyType type) {
            foreach (var bodyPart in bodyParts) {
                if (bodyPart.Type == type) {
                    if (!bodyPart.IsOccupied)
                        return (true, bodyPart);
                    if (centerBodyTypes.Contains(bodyPart.Type)) {
                        var (found, part) = GetOtherSidePart(bodyPart);
                        if (found)
                            return (true, bodyPart);
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
    }
}