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
        Gamepad primaryGamepad;
        Gamepad secondaryGamepad;

        [SerializeField] Component[] dependentComponents;
 
        bool primaryPlayerReady;
        bool secondaryPlayerReady;
        
        void Awake() {
            AssignGamepads();
            primaryPopup.StartFading();
            secondaryPopup.StartFading();
            foreach (var dependentComponent in dependentComponents) 
                (dependentComponent as IWaitForStart).StartMenu = this;
        }

        void AssignGamepads() {
            var pads = Gamepad.all;
            if (!pads.Any()) {
                Debug.Log($"No connected pads");
                return;
            }
            Debug.Log($"{pads} connected.");
            if(pads.Count > 0)
                primaryGamepad = pads[0];
            if(pads.Count > 1)
                secondaryGamepad = pads[1];
        }
        
        void Update() {
            if (primaryGamepad.aButton.isPressed) {
                primaryPopup.SetActive(false);
                primaryPlayerReady = true;
            }

            if (primaryPlayerReady) {
                foreach (var dependentComponent in dependentComponents) {
                    (dependentComponent as IWaitForStart).Ready = true;
                }
            }
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Collect")]
        void Collect() {
            var components = FindObjectsOfType<Component>();
            var iWaits = new List<Component>();
            foreach (var component in components) {
                if (component is IWaitForStart)
                    iWaits.Add(component);
            }
            UnityEditor.Undo.RecordObject(this, "IWaits");
            dependentComponents = iWaits.ToArray();
        }
        #endif
    }

    interface IWaitForStart {
        bool Ready { set; }
        StartMenu StartMenu { set; }
    }
}
