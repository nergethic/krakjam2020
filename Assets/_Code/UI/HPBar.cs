using UnityEngine;
using UnityEngine.UI;

namespace _Code.UI {
    public class HPBar : MonoBehaviour {
        [SerializeField] Transform fillTransform;
        [SerializeField] Image fillImage;
        [SerializeField] Image backgroundImage;

        public Image FillImage => fillImage;
        public Image BackgroundImage => backgroundImage;
    
        public void SetValue(float value) {
            var currentScale = fillTransform.localScale;
            currentScale.x = value;
            fillTransform.localScale = currentScale;
        }
    }
}
