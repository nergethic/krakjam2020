using System;
using System.Linq;
using _Code.UI;
using Boo.Lang;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Code {
    public class StartMenu : MonoBehaviour {
        [SerializeField] GameObject title;
        [SerializeField] GameObject winGroup;
        [SerializeField] StartGamePopup primaryPopup;
        [SerializeField] StartGamePopup secondaryPopup;
        [SerializeField] Rigidbody primaryPlayerRigidbody;
        [SerializeField] Rigidbody secondaryPlayerRigidbody;
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
        const float MIN_BREAK_FREQUENCY = 0.1F;
        const float MAX_BREAK_FREQUENCY = 0.4F;
        
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
            if (primaryGamepad != null && primaryGamepad.aButton.wasPressedThisFrame) {
                primaryPopup.StopFading();
                primaryPlayerReady = true;
                TriggerPrimaryPadVibration(0.7f, 1f, READY_VIBRATION_TIME);
            }
            if (secondaryGamepad != null && secondaryGamepad.aButton.wasPressedThisFrame) {
                secondaryPopup.StopFading();
                secondaryPlayerReady = true;
                TriggerSecondaryPadVibration(0.7f, 1f, READY_VIBRATION_TIME);
            }

            if(!Gamepad.all.Any() && Keyboard.current.spaceKey.wasPressedThisFrame)
                Invoke(nameof(SetReady), 1f);
            else if(Gamepad.all.Any() && Gamepad.all.Count == 1 && primaryGamepad.aButton.wasPressedThisFrame)
                Invoke(nameof(SetReady), 1f);
            if (primaryPlayerReady && secondaryPlayerReady)
                Invoke(nameof(SetReady), 1f);
        }

        void SetReady() {
            ready = true;
            title.SetActive(false);
            primaryPopup.Disable();
            secondaryPopup.Disable();
            foreach (var dependentComponent in dependentComponents) 
                (dependentComponent as IWaitForStart).Ready = true;
            foreach (var playerRigidbody in playerRigidbodies) 
                playerRigidbody.isKinematic = false;
        }

        public void EnableWinOverlay() {
            winGroup.SetActive(true);
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

        public void TriggerIslandBreakVibration(Transform islandPart) {
            PrimaryIslandVibration(islandPart);
            SecondaryIslandVibration(islandPart);
        }

        void PrimaryIslandVibration(Transform islandPart) {
            var magnitude = (primaryPlayerRigidbody.transform.position - islandPart.position).magnitude;
            magnitude = Mathf.Clamp(magnitude, 0, 100);
            var frequency = 1 - magnitude / 100f;
            var low = frequency - 0.2f >= MIN_BREAK_FREQUENCY ? frequency : MIN_BREAK_FREQUENCY;
            var high = frequency + 0.2f >= MAX_BREAK_FREQUENCY ? frequency : MAX_BREAK_FREQUENCY;
            TriggerPrimaryPadVibration(low, high, 0.3f);
        }
        
        void SecondaryIslandVibration(Transform islandPart) {
            var magnitude = (secondaryPlayerRigidbody.transform.position - islandPart.position).magnitude;
            magnitude = Mathf.Clamp(magnitude, 0, 100);
            var frequency = 1 - magnitude / 100f;
            var low = frequency - 0.2f >= MIN_BREAK_FREQUENCY ? frequency : MIN_BREAK_FREQUENCY;
            var high = frequency + 0.2f >= MAX_BREAK_FREQUENCY ? frequency : MAX_BREAK_FREQUENCY;
            TriggerPrimaryPadVibration(low, high, 0.3f);
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
                if (playerController.PadIndex == 0)
                    primaryPlayerRigidbody = rb;
                else if (playerController.PadIndex == 1)
                    secondaryPlayerRigidbody = rb;
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
