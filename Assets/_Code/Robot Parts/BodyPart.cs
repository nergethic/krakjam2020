using System;
using UnityEngine;

namespace _Code.Robot_Parts {
    public class BodyPart : MonoBehaviour {
        [SerializeField] BodySide side;
        [SerializeField] BodyType type;
        //ArmorPart placeholder;
        
        bool isOccupied;

        public BodySide Side => side;
        public BodyType Type => type;
        public bool IsOccupied => isOccupied;
        
        //void AttachArmor(ArmorPart armor){}
    
        [Serializable]
        public enum BodyType { Wrist, Glow, Shoulder, Helm, Chest, Boots }
        public enum BodySide { Center, Right, Left} 
    }
}