using System;
using OopAndArchitecture.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace OopAndArchitecture.UI {
    /// <summary>
    /// Объект с информацией о применённом бонусе от модуля, который отображается в UI.
    /// </summary>
    public class ShipModulesStatUI : MonoBehaviour {
        [SerializeField] private Text m_text;

        private ISlot m_slot;
        
        
        public void SetSlotModuleDescription(ISlot slot, IModule module) {
            m_slot = slot;
            m_text.text = module.Description;
        }

        public bool Compare(ISlot slot) {
            return slot?.Type == m_slot?.Type && slot?.Index == m_slot?.Index;
        }

        private void Awake() {
            Assert.IsNotNull(m_text, "m_text != null");
        }
    }
}