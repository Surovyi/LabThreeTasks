using System;
using OopAndArchitecture.Enums;
using OopAndArchitecture.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace OopAndArchitecture.UI {
    /// <summary>
    /// UI для слота корабля.
    /// Этими объектами наполняется список доступных для апгрейда слотов корабля.
    /// </summary>
    public class SlotUI : MonoBehaviour, ISlot {
        public SlotType Type => m_type;
        public int Index => m_index;
        
        
        [SerializeField] private Text m_description;
        [SerializeField] private Image m_image;
        [SerializeField] private Color m_attachedColor;
        
        
        private Color m_notAttachedColor;
        private SlotType m_type;
        private int m_index;
        private Action<ISlot> m_onClick;

        
        /// <summary>
        /// Инициализация слота его параметрами
        /// </summary>
        /// <param name="type">Тип слота</param>
        /// <param name="slotIndex">Индекс слота</param>
        /// <param name="onClick">Колбек</param>
        public void Init(SlotType type, int slotIndex, Action<ISlot> onClick) {
            const string weapon = "Weapon";
            const string hull = "Hull";
            
            m_type = type;
            m_index = slotIndex;
            m_description.text = $"{(type == SlotType.WEAPON ? weapon : hull)} slot {slotIndex}";
            m_onClick = onClick;
        }

        public void ActivateByColor(bool active) {
            m_image.color = active ? m_attachedColor : m_notAttachedColor;
        }

        public void OnClick() {
            m_onClick?.Invoke(this);
        }


        private void Awake() {
            Assert.IsNotNull(m_description, "m_description != null");
            Assert.IsNotNull(m_image, "m_image != null");
            
            m_notAttachedColor = m_image.color;
        }

        private void OnDestroy() {
            m_onClick = null;
        }
    }
}