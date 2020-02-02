using System;
using System.Linq;
using _Code.UI;
using Boo.Lang;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Code {
    public class StartMenu : MonoBehaviour {
        [SerializeField] StartGamePopup primaryPopup;
        [SerializeField] StartGamePopup secondaryPopup;
        [SerializeField] Rigidbody[] playerRigidbodies;
        [SerializeField] Component[] dependentComponents;
        Gamepad primaryGamepad;
        Gamepad secondaryGamepad;
        
        public Gamepad PrimaryGamepad => primaryGamepad;
        public Gamepad SecondaryGamepad => secondaryGamepad;
 
        bool primaryPlayerReady;
        bool secondaryPlayerReady;
        bool ready;
        
        const float READY_VIBRATION_TIME = 0.2F;
        
        void Awake() {
            AssignGamepads();
            
            primaryPopup.StartFading();
            secondaryPopup.StartFading();

            foreach (var playerRigidbody in playerRigidbodies) 
                playerRigidbody.isKinematic = true;
            
            foreach (var dependentComponent in dependentComponents) 
                (dependentComponent as IWaitForStart).StartMenu = this;
        }

        void AssignGamepads() {
            var pads = Gamepad.all;
            if (!pads.Any()) {
                Debug.Log("No connected pads");
                return;
            }
            Debug.Log($"{pads.Count} pads connected.");
            if(pads.Count > 0)
                primaryGamepad = pads[0];
            if(pads.Count > 1)
                secondaryGamepad = pads[1];
        }
        
        void Update() {
            if(ready)
                return;
            if (primaryGamepad != null && primaryGamepad.aButton.isPressed) {
                primaryPopup.StopFading();
                primaryPlayerReady = true;
                TriggerPrimaryPadVibration(0.7f, 1f, READY_VIBRATION_TIME);
            }
            if (secondaryGamepad != null && secondaryGamepad.aButton.isPressed) {
                secondaryPopup.StopFading();
                secondaryPlayerReady = true;
                TriggerSecondaryPadVibration(0.7f, 1f, READY_VIBRATION_TIME);
            }

            if (primaryPlayerReady)//  && secondaryPlayerReady)
                Invoke(nameof(SetReady), 1f);
        }

        void SetReady() {
            ready = true;
            primaryPopup.Disable();
            secondaryPopup.Disable();
            foreach (var dependentComponent in dependentComponents) 
                (dependentComponent as IWaitForStart).Ready = true;
            foreach (var playerRigidbody in playerRigidbodies) 
                playerRigidbody.isKinematic = false;
        }


        public void TriggerBothPadsVibrations(float lowFrequency, float highFrequency, float time) {
            TriggerPrimaryPadVibration(lowFrequency, highFrequency, time);
            TriggerSecondaryPadVibration(lowFrequency, highFrequency, time);
        }
        
        public void TriggerPrimaryPadVibration(float lowFrequency, float highFrequency, float time) {
            if(primaryGamepad == null)
                return;
            primaryGamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            Invoke(nameof(StopPrimaryPadVibration), time);
        }
        
        public void TriggerSecondaryPadVibration(float lowFrequency, float highFrequency, float time) {
            if(secondaryGamepad == null)
                return;
            secondaryGamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            Invoke(nameof(StopSecondaryPadVibration), time);
        }
        
        void StopPrimaryPadVibration() {
            primaryGamepad.ResetHaptics();
        }
        
        void StopSecondaryPadVibration() {
            secondaryGamepad.ResetHaptics();
        }

        public void TriggerIslandBreakVibration() {
            
        }
        
#if UNITY_EDITOR
        [ContextMenu("Collect")]
        void Collect() {
            UnityEditor.Undo.RecordObject(this, "Collect");
            var components = FindObjectsOfType<Component>();
            var iWaits = new List<Component>();
            foreach (var component in components) {
                if (component is IWaitForStart)
                    iWaits.Add(component);
            }
            dependentComponents = iWaits.ToArray();

            var rigidbodies = new List<Rigidbody>();
            foreach (var playerController in FindObjectsOfType<PlayerController>()) {
                var rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                    rigidbodies.Add(rb);
            }
            playerRigidbodies = rigidbodies.ToArray();
        }
#endif
    }

    interface IWaitForStart {
        bool Ready { set; }
        StartMenu StartMenu { set; }
    }
}
