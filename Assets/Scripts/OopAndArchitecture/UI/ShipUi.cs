using OopAndArchitecture.Interfaces.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace OopAndArchitecture.UI {
    /// <summary>
    /// Реализация интерфейса для корабля.
    /// </summary>
    public class ShipUi : MonoBehaviour, IShipUi {
        [SerializeField] private Ship.Ship m_ship;
        [SerializeField] private PropertyVisualizerUI m_hp;
        [SerializeField] private PropertyVisualizerUI m_shields;
        [SerializeField] private GameObject m_destroyedMessage;


        private void Start() {
            Assert.IsNotNull(m_ship, "m_ship != null");
            Assert.IsNotNull(m_hp, "m_hp != null");
            Assert.IsNotNull(m_shields, "m_shields != null");
            Assert.IsNotNull(m_destroyedMessage, "m_destroyedMessage != null");
            
            m_ship.AssignShipUi(this);
        }

        public void ChangeHp(float maxValue, float value) {
            m_hp.SetValue(maxValue, value);
        }

        public void ChangeShields(float maxValue, float value) {
            m_shields.SetValue(maxValue, value);
        }

        public void ShowDestroyedShipMessage() {
            m_destroyedMessage.SetActive(true);
        }
    }
}