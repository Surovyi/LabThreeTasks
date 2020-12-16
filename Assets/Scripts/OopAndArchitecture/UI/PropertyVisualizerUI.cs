using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace OopAndArchitecture.UI {
    /// <summary>
    /// Визуализатор для fill type Image компонентов.
    /// NOTE: возможно стоит название более корректное придумать.
    /// </summary>
    public class PropertyVisualizerUI : MonoBehaviour {
        [SerializeField] private Image m_propertyImage;
        [SerializeField] private Text m_propertyText;
        [SerializeField] private string m_startWith;

        private float m_maxValue;
        private float m_currentValue;


        private void Awake() {
            Assert.IsNotNull(m_propertyImage, "m_propertyImage != null");
            Assert.IsNotNull(m_propertyText, "m_propertyText != null");
        }

        /// <summary>
        /// Установить новое значение изменяемого параметра
        /// </summary>
        /// <param name="maxValue">Максимальное</param>
        /// <param name="currentValue">Текущее</param>
        public void SetValue(float maxValue, float currentValue) {
            m_maxValue = maxValue;
            m_currentValue = currentValue;

            m_propertyImage.fillAmount = m_currentValue / m_maxValue;
            m_propertyText.text = m_startWith + m_currentValue.ToString("F1") + "/" + m_maxValue.ToString("F1");
        }
    }
}