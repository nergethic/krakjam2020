using System;
using UnityEngine;

namespace _Code.Robot_Parts {
    public class BodyPart : MonoBehaviour {
        [SerializeField] BodyType type;
        [SerializeField] BodySide side;
        //ArmorPart placeholder;
        
        bool isOccupied;

        public BodySide Side => side;
        public BodyType Type => type;
        public bool IsOccupied => isOccupied;
        
        //void AttachArmor(ArmorPart armor){}

        [Serializable]
        public enum BodyType { Wrist = 0, Shoulder = 1, Helm = 2, Chest = 3, Leg = 4 }
        [Serializable]
        public enum BodySide { Center = 0, Right = 2, Left = 3} 
    }
}