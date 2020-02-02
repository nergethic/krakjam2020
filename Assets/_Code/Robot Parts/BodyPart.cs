using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace _Code.Robot_Parts {
    public class BodyPart : MonoBehaviour {
        [SerializeField] BodyType type;
        [SerializeField] BodySide side;
        public ArmourPart armourPart;
        
        bool isOccupied;

        public BodySide Side => side;
        public BodyType Type => type;
        public bool IsOccupied => isOccupied;

        public void SetOccupied(bool value, ArmourPart part) {
            if (isOccupied) {
                armourPart.isAttached = value;
                armourPart = null;
            }

            if (value) {
                Assert.IsTrue(part != null);
                armourPart = part;
                armourPart.isAttached = true;
            }
            
            isOccupied = value;
        }

        [Serializable]
        public enum BodyType {
            Wrist = 0, 
            Shoulder = 1, 
            Helm = 2, 
            Chest = 3, 
            UpperLeg = 4,
            Feet = 5,
            Abdomen = 6,
            LowerLeg = 7
        }
        [Serializable]
        public enum BodySide { Center = 0, Right = 2, Left = 3} 
    }
}